using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Chargeback
{
    public class SelectChargebackEventDto
    {
        public string Type { get; set; } = "";
        public string? Note { get; set; }
        public string? EvidenceUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
