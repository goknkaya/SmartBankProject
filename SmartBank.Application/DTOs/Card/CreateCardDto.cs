using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Card
{
    public class CreateCardDto
    {
        public int CustomerId { get; set; }

        // Kart Bilgileri
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }     // Ayrı tutuluyor
        public string ExpiryYear { get; set; }      // Ayrı tutuluyor
        public string Currency { get; set; } = "TRY";
        public string PinHash { get; set; }

        // Durumlar
        public string CardStatus { get; set; }
        public string CardType { get; set; } // Örn: "Debit" veya "Credit"
        public string CardStatusDescription { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsContactless { get; set; }
        public bool IsBlocked { get; set; }

        // Limit Bilgileri
        public decimal CardLimit { get; set; }
        public decimal DailyLimit { get; set; }
        public decimal TransactionLimit { get; set; }

        // Ek Güvenlik
        public int FailedPinAttempts { get; set; }
        public DateTime? LastUsedAt { get; set; }

        // Kurum & Altyapı
        public string CardProvider { get; set; }
        public string CardIssuerBank { get; set; }
        public string CardStatusChangeReason { get; set; }

        public int? ParentCardId { get; set; }

    }
}
