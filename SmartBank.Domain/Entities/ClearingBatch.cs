using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities
{
    public class ClearingBatch
    {
        public int Id { get; set; }

        [MaxLength(3)]
        public string Direction { get; set; } = "IN";   // IN, OUT

        [MaxLength(255)]
        public string FileName { get; set; } = "";

        [MaxLength(64)]
        public string FileHash { get; set; } = "";

        public DateTime SettlementDate { get; set; }

        [MaxLength(1)]
        public string Status { get; set; } = "N"; // N=New, P=Processed, E=Error

        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }

        [MaxLength(250)]
        public string? Notes { get; set; }

        public ICollection<ClearingRecord> Records { get; set; } = new List<ClearingRecord>();
    }
}
