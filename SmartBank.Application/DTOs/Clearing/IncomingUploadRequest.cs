// SmartBank.Application/DTOs/Clearing/IncomingUploadRequest.cs
using Microsoft.AspNetCore.Http;

namespace SmartBank.Application.DTOs.Clearing
{
    // multipart/form-data için tek paket
    public class IncomingUploadRequest
    {
        public IFormFile File { get; set; } = default!;

        // meta
        public string Direction { get; set; } = "IN"; // sadece IN
        public string? FileName { get; set; }
        public string? FileHash { get; set; }
        public DateTime SettlementDate { get; set; }
        public string? Notes { get; set; }
    }
}
