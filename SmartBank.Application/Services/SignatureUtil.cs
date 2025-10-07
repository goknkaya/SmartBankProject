using System.Security.Cryptography;
using System.Text;

namespace SmartBank.Application.Services
{
    public static class SignatureUtil
    {
        public static string? NormalizeLast4(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return null;
            if (digits.Length > 4) return digits[^4..];
            return digits.PadLeft(4, '0');
        }

        public static string ComputeSignature(string? last4, decimal amount, string currency, DateTime? txDate, string? merchant)
        {
            var l4 = NormalizeLast4(last4) ?? "";
            var ccy = (currency ?? "").Trim().ToUpperInvariant();
            var d = txDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";
            var m = (merchant ?? "").Trim();

            var raw = $"{l4}|{amount:0.####}|{ccy}|{d}|{m}";
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
        }

        // PAN (kart numarası) varsa kolay kullanım
        public static string ComputeFromPan(string? pan, decimal amount, string currency, DateTime txDate, string? merchant)
        {
            var digits = pan?.Replace(" ", "");
            var last4 = NormalizeLast4(digits);
            return ComputeSignature(last4, amount, currency, txDate, merchant);
        }
    }
}
