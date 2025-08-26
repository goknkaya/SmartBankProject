using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Switching
{
    public class CreateSwitchMessageDto
    {
        public string PAN { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Acquirer { get; set; } = "DemoPOS";
        public DateTime? TxnTime { get; set; }
        public string? MerchantName { get; set; }
    }
}
