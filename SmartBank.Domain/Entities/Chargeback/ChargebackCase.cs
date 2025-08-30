using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities.Chargeback
{
    public class ChargebackCase
    {
        public int Id { get; set; }

        [Required] public int TransactionId { get; set; }
        // Snapshot alanları (o anki bilgiyi sabitlemek için)
        [MaxLength(3)] public string Currency { get; set; } = "TRY";
        public decimal TransactionAmount { get; set; }
        [MaxLength(100)] public string? MerchantName { get; set; }

        // Şikâyet verisi
        [MaxLength(20)] public string ReasonCode { get; set; } = "4853"; // örn. Visa "Cardholder Dispute"
        public decimal DisputedAmount { get; set; }

        // Durum makinesi: Open -> (Won|Lost|Cancelled)
        [MaxLength(15)] public string Status { get; set; } = "Open"; // Open/Won/Lost/Cancelled

        // Zamanlar
        public DateTime OpenedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }

        // Opsiyonel: son tarih (n+45 gün vb.)
        public DateTime? ReplyBy { get; set; }

        // Basit alanlar
        [MaxLength(200)] public string? Note { get; set; }

        // İlişkiler
        public Transaction? Transaction { get; set; }
        public ICollection<ChargebackEvent> Events { get; set; } = new List<ChargebackEvent>();
    }
}
