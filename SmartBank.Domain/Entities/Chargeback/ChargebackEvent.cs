using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities.Chargeback
{
    public class ChargebackEvent
    {
        public int Id { get; set; }

        public int CaseId { get; set; }
        public ChargebackCase Case { get; set; } = null!;

        [MaxLength(20)] public string Type { get; set; } = "Opened"; // Opened, EvidenceAdded, Represented, Won, Lost, Cancelled
        [MaxLength(200)] public string? Note { get; set; }
        public string? EvidenceUrl { get; set; } // dosya yolu/link
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
