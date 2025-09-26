using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Reversal
{
    public class SelectReversalDto
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public decimal ReversedAmount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string PerformedBy { get; set; }
        public DateTime ReversalDate { get; set; }
        public string ReversalSource { get; set; }
        public bool IsCardLimitRestored { get; set; }

        public string? VoidedBy { get; set; }
        public DateTime? VoidedAt { get; set; }
        public string? VoidReason { get; set; }
    }
}
