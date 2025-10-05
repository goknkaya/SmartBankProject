using System;

namespace SmartBank.Desktop.Win.Core.Contracts
{
    // API'den gelecek case listesi için
    public class SelectChargebackCaseDto
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Status { get; set; } = "Open";
        public string ReasonCode { get; set; } = "";
        public decimal DisputedAmount { get; set; }
        public string Currency { get; set; } = "TRY";
        public decimal TransactionAmount { get; set; }
        public string? MerchantName { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? ReplyBy { get; set; }
        public string? Note { get; set; }
    }

    // Event listesi için
    public class SelectChargebackEventDto
    {
        public string Type { get; set; } = "";
        public string? Note { get; set; }
        public string? EvidenceUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Yeni case açarken kullanılacak istek
    public class CreateChargebackDto
    {
        public int TransactionId { get; set; }
        public string ReasonCode { get; set; } = "4853";
        public decimal DisputedAmount { get; set; }
        public DateTime? ReplyBy { get; set; }
        public string? Note { get; set; }
    }

    // Kanıt eklerken kullanılacak istek
    public class AddEvidenceDto
    {
        public string? EvidenceUrl { get; set; }
        public string? Note { get; set; }
    }

    // Karar verirken kullanılacak istek
    public class DecideChargebackDto
    {
        public string Decision { get; set; } = "Won"; // Won | Lost | Cancelled
        public string? Note { get; set; }
    }
}
