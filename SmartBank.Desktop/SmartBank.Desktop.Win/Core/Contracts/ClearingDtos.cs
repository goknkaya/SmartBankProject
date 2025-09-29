using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core.Contracts
{
    // Desktop tarafına (SmartBank.Desktop.Win/DTOs/Clearing)
    public class SelectClearingBatchDto
    {
        public int Id { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public DateTime SettlementDate { get; set; }
        public string Status { get; set; } = "N";
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class SelectClearingRecordDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int LineNumber { get; set; }
        public int? TransactionId { get; set; }
        public int? CardId { get; set; }
        public string? CardLast4 { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public DateTime? TransactionDate { get; set; }
        public string? MerchantName { get; set; }
        public string MatchStatus { get; set; } = "P";
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
