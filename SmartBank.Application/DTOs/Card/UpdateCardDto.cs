using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Card
{
    public class UpdateCardDto
    {
        public int Id { get; set; }

        // Durumlar
        public string? CardStatus { get; set; }
        public string? CardStatusDescription { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsContactless { get; set; }
        public bool? IsVirtual { get; set; }  // Eksikti, ekliyoruz

        // Limit Bilgileri
        public decimal? CardLimit { get; set; }        // Create'de vardı, update için ekliyoruz
        public decimal? DailyLimit { get; set; }
        public decimal? TransactionLimit { get; set; }

        // Ek Güvenlik
        public int? FailedPinAttempts { get; set; }
        public DateTime? LastUsedAt { get; set; }

        // Kurum & Altyapı
        public string? CardStatusChangeReason { get; set; }
        public string? CardProvider { get; set; }
        public string? CardIssuerBank { get; set; }

        public int? ParentCardId { get; set; }   // Kart ilişkisi güncellenebilir

    }
}
