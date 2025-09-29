// Core/ApiClient.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartBank.Desktop.Win.Core
{
    /// <summary>
    /// HTTP çağrıları, login ve ortak hata işleme için tek nokta.
    /// </summary>
    public sealed class ApiClient
    {
        // Relative path birleşimi için sonda '/' olmalı
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
                // Geliştirme ortamında self-signed sertifikayı kabul et
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

        // ---- Dışarıya faydalı birkaç readonly bilgi
        public Uri BaseAddress => _http.BaseAddress!;
        public HttpRequestHeaders DefaultHeaders => _http.DefaultRequestHeaders;

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

            // API'nin döndüğü yapıya göre; sende zaten var
            var dto = JsonSerializer.Deserialize<LoginResponseDto>(body, _json)
                      ?? throw new ApiException((int)res.StatusCode, body, "Boş login yanıtı.");

            if (string.IsNullOrWhiteSpace(dto.Token))
                throw new ApiException((int)res.StatusCode, body, "Token boş döndü.");

            TrySetAuthContext(dto);
            return true;
        }

        // =========================
        // GENERIC HELPERS (JSON)
        // =========================
        public async Task<T?> GetAsync<T>(string path, bool withAuth = true)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            return string.IsNullOrWhiteSpace(body)
                ? default
                : JsonSerializer.Deserialize<T>(body, _json);
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

            return string.IsNullOrWhiteSpace(body)
                ? default
                : JsonSerializer.Deserialize<TResponse>(body, _json);
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

            return string.IsNullOrWhiteSpace(body)
                ? default
                : JsonSerializer.Deserialize<TResponse>(body, _json);
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
        // MULTIPART (dosya upload)
        // =========================

        /// <summary>
        /// Diskteki bir dosyayı multipart/form-data ile gönderir.
        /// </summary>
        public async Task<TResponse?> PostMultipartAsync<TResponse>(
            string path,
            Dictionary<string, string> fields,
            string fileFieldName,
            string filePath,
            bool withAuth = true)
        {
            await using var fs = File.OpenRead(filePath);
            var fileName = Path.GetFileName(filePath);
            return await PostMultipartAsync<TResponse>(path, fields, fileFieldName, fs, fileName, withAuth);
        }

        /// <summary>
        /// Bellekteki bir stream'i (ör. OpenFileDialog'dan) multipart/form-data ile gönderir.
        /// </summary>
        public async Task<TResponse?> PostMultipartAsync<TResponse>(
            string path,
            Dictionary<string, string> fields,
            string fileFieldName,
            Stream fileStream,
            string fileName,
            bool withAuth = true)
        {
            using var form = new MultipartFormDataContent();

            // Text alanları
            if (fields != null)
            {
                foreach (var kv in fields)
                    form.Add(new StringContent(kv.Value ?? ""), kv.Key);
            }

            // Dosya içeriği
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            form.Add(fileContent, fileFieldName, fileName);

            using var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = form };
            AddAuth(req, withAuth);

            using var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                ThrowApi(body, (int)res.StatusCode, req.Method, req.RequestUri?.ToString());

            return string.IsNullOrWhiteSpace(body)
                ? default
                : JsonSerializer.Deserialize<TResponse>(body, _json);
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
                                foreach (var msg in kv.Value ?? Array.Empty<string>())
                                    lines.Add($"{kv.Key}: {msg}");
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
                // Yanıt gövdesi JSON değilse sessiz geç
            }

            return $"{method} {url} -> {status}";
        }

        /// <summary>
        /// AuthContext.Set(...) veya SetToken(...) varyantlarını reflection ile çağırır.
        /// (Projendeki AuthContext şekline uyumlu olması için.)
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

            // Hiçbiri yoksa minimum fallback
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

    /// <summary>
    /// RFC7807 + FluentValidation uyumlu problem detayları.
    /// </summary>
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
