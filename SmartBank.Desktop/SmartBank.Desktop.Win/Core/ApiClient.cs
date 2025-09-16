// Core/ApiClient.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartBank.Desktop.Win.Core
{
    /// <summary>
    /// Tek noktadan HTTP çağrıları + login.
    /// </summary>
    public sealed class ApiClient
    {
        // Sonda / olması önemli (relative path'ler birleşsin)
        private const string BaseUrl = "https://localhost:7244/";

        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly HttpClient _http;

        static ApiClient()
        {
            var handler = new HttpClientHandler
            {
#if DEBUG
                // Geliştirmede self-signed sertifika
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
            };

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // =========================
        // AUTH
        // =========================
        public async Task<bool> LoginAsync(string username, string password)
        {
            username = username?.Trim() ?? string.Empty;
            password = password?.Trim() ?? string.Empty;

            var payload = new { username, password };

            using var req = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json")
            };

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            // Login yanıtı JSON: { token, role, expiresAt } (senin API'ye uygun)
            var dto = JsonSerializer.Deserialize<LoginResponseDto>(body, _json)
                      ?? throw new ApiException((int)res.StatusCode, body, "Boş login yanıtı.");

            if (string.IsNullOrWhiteSpace(dto.Token))
                throw new ApiException((int)res.StatusCode, body, "Token boş döndü.");

            // AuthContext'e güvenli şekilde yaz (Set veya SetToken hangisi varsa)
            TrySetAuthContext(dto);

            return true;
        }

        // =========================
        // GENERIC HELPERS
        // =========================
        public async Task<T?> GetAsync<T>(string path, bool withAuth = true)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            if (string.IsNullOrWhiteSpace(body)) return default;
            return JsonSerializer.Deserialize<T>(body, _json);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest data, bool withAuth = true)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(JsonSerializer.Serialize(data, _json), Encoding.UTF8, "application/json")
            };
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            if (string.IsNullOrWhiteSpace(body)) return default;
            return JsonSerializer.Deserialize<TResponse>(body, _json);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest data, bool withAuth = true)
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, path)
            {
                Content = new StringContent(JsonSerializer.Serialize(data, _json), Encoding.UTF8, "application/json")
            };
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            if (string.IsNullOrWhiteSpace(body)) return default;
            return JsonSerializer.Deserialize<TResponse>(body, _json);
        }

        public async Task DeleteAsync(string path, bool withAuth = true)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, path);
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());
        }

        // =========================
        // INTERNALS
        // =========================
        private static void AddAuth(HttpRequestMessage req, bool withAuth)
        {
            if (!withAuth) return;
            if (AuthContext.IsAuthenticated && !string.IsNullOrWhiteSpace(AuthContext.Token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthContext.Token);
            }
        }

        private static void ThrowApi(string body, int status, HttpMethod? method, string? url)
        {
            string message = BuildErrorMessage(body, status, method, url);
            throw new ApiException(status, body ?? string.Empty, message);
        }

        private static string BuildErrorMessage(string body, int status, HttpMethod? method, string? url)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var pd = JsonSerializer.Deserialize<ProblemDetailsDto>(body, _json);
                    if (pd != null)
                    {
                        // FluentValidation error'ları
                        if (pd.Errors is { Count: > 0 })
                        {
                            var lines = new List<string>();
                            foreach (var kv in pd.Errors)
                            {
                                foreach (var msg in kv.Value ?? Array.Empty<string>())
                                    lines.Add($"{kv.Key}: {msg}");
                            }
                            if (lines.Count > 0)
                                return string.Join(Environment.NewLine, lines);
                        }

                        if (!string.IsNullOrWhiteSpace(pd.Title) || !string.IsNullOrWhiteSpace(pd.Detail))
                            return $"{pd.Title} {pd.Detail}".Trim();
                    }
                }
            }
            catch
            {
                // Body JSON değilse sessizce geç
            }

            return $"{method} {url} -> {status}";
        }

        /// <summary>
        /// AuthContext.Set(token, role, expiresAt) ya da AuthContext.SetToken(token, expiresAt)
        /// hangisi varsa onu reflection ile çağırır (compile-time hatayı önler).
        /// </summary>
        private static void TrySetAuthContext(LoginResponseDto dto)
        {
            var t = typeof(AuthContext);

            // Önce Set(token, role, expiresAt?) dene
            var mSet = t.GetMethod("Set",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (mSet != null)
            {
                var pars = mSet.GetParameters();
                if (pars.Length >= 2) // token, role, (optional expiresAt)
                {
                    object? expires = dto.ExpiresAt == default ? null : dto.ExpiresAt;
                    var args = pars.Length switch
                    {
                        3 => new object?[] { dto.Token, dto.Role, expires },
                        2 => new object?[] { dto.Token, dto.Role },
                        _ => null
                    };
                    if (args != null) { mSet.Invoke(null, args); return; }
                }
            }

            // Sonra SetToken(token, expiresAt?) dene
            var mSetToken = t.GetMethod("SetToken",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (mSetToken != null)
            {
                var pars = mSetToken.GetParameters();
                object? expires = dto.ExpiresAt == default ? null : dto.ExpiresAt;
                var args = pars.Length switch
                {
                    2 => new object?[] { dto.Token, expires },
                    1 => new object?[] { dto.Token },
                    _ => null
                };
                if (args != null) { mSetToken.Invoke(null, args); return; }
            }

            // Hiçbiri yoksa minimum: sadece statik alanlar varsa onları set etmeyi dene
            try
            {
                t.GetProperty("Token")?.SetValue(null, dto.Token);
                t.GetProperty("Role")?.SetValue(null, dto.Role);
                if (dto.ExpiresAt != default)
                    t.GetProperty("ExpiresAt")?.SetValue(null, dto.ExpiresAt);
            }
            catch { /* yoksay */ }
        }
    }
    /// <summary>RFC7807 + FluentValidation gövdesi.</summary>
    public sealed class ProblemDetailsDto
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
