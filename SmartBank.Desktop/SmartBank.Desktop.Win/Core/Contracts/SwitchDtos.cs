using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core.Contracts
{
    public sealed class CreateSwitchMessageDto
    {
        public string PAN { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Acquirer { get; set; } = "DemoPOS";
        public DateTime? TxnTime { get; set; }
        public string? MerchantName { get; set; }
    }

    public sealed class SelectSwitchMessageDto
    {
        public int Id { get; set; }
        public string PANMasked { get; set; } = "";
        public string Bin { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Acquirer { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public int? TransactionId { get; set; }
    }

    public sealed class SwitchLogDto
    {
        public string Stage { get; set; } = "";
        public string Level { get; set; } = "";
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PayloadIn { get; set; }
        public string? PayloadOut { get; set; }
    }
}
