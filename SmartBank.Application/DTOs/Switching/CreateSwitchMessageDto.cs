using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Switching
{
    public sealed class CreateSwitchMessageDto
    {
        public string PAN { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Acquirer { get; set; } = "DemoPOS";
        public DateTime? TxnTime { get; set; }
        public string? MerchantName { get; set; }

        // ISO8583 benzeri opsiyoneller
        public string? RRN { get; set; }        // DE37 (6–12 digit)
        public string? STAN { get; set; }       // DE11 (6 digit)
        public string? TerminalId { get; set; } // DE41 (<=16 char)
    }
}

