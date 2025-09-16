using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core
{
    public static class AuthContext
    {
        public static string? Token { get; private set; }
        public static string? Role { get; private set; }
        public static DateTime? ExpiresAt { get; private set; }

        public static bool IsAuthenticated =>
            !string.IsNullOrWhiteSpace(Token) &&
            ExpiresAt.HasValue &&
            ExpiresAt.Value > DateTime.Now.AddMinutes(-1); // ufak tolerans

        public static void Set(string token, string? role, DateTime? expiresAt)
        {
            Token = token;
            Role = role;
            ExpiresAt = expiresAt;
        }

        public static void Clear()
        {
            Token = null;
            Role = null;
            ExpiresAt = null;
        }
    }
}
