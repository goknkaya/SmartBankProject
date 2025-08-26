using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Switching
{
    public class SelectSwitchMessageDto
    {
        public int Id { get; set; }
        public string PANMasked { get; set; } = "";   // PAN gizlenmiş hali
        public string Bin { get; set; } = "";         // İlk 6 hane
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Acquirer { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Status { get; set; } = "Pending";  // Pending/Approved/Declined
        public DateTime CreatedAt { get; set; }
        public int? TransactionId { get; set; }          // İsteğe bağlı transaction bağlantısı
    }
}
