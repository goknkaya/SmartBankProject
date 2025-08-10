using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Reversal
{
    public class CreateReversalDto
    {
        public int TransactionId { get; set; }
        public decimal ReversedAmount { get; set; }
        public string Reason { get; set; }
        public string PerformedBy { get; set; }
        public string ReversalSource { get; set; }
    }
}
