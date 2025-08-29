using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Chargeback
{
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
}
