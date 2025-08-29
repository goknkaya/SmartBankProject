using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Chargeback
{
    public class CreateChargebackDto
    {
        public int TransactionId { get; set; }
        public string ReasonCode { get; set; } = "4853";
        public decimal DisputedAmount { get; set; }
        public DateTime? ReplyBy { get; set; }
        public string? Note { get; set; }
    }
}
