using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Card
{
    public class SelectCardDto
    {
        [Key]
        public int Id { get; set; }

        public string CustomerFullName { get; set; }

        public string CustomerNumber { get; set; }

        // Kart Bilgileri
        public string MaskedCardNumber { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public string Currency { get; set; }

        // Durumlar
        public string CardStatus { get; set; }

        public string CardStatusDescription { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsContactless { get; set; }

        public bool IsBlocked { get; set; }

        // Limit Bilgileri
        public decimal CardLimit { get; set; }

        public decimal DailyLimit { get; set; }

        public decimal TransactionLimit { get; set; }

        // Ek güvenlik / kontrol
        public int FailedPinAttempts { get; set; }

        public DateTime? LastUsedAt { get; set; }

        // Kurum ve Altyapi
        public string CardProvider { get; set; }

        public string CardIssuerBank { get; set; }

        public string CardStatusChangeReason { get; set; }

        public int? ParentCardId { get; set; }
    }
}
