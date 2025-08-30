using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBank.Domain.Entities
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(19)]
        public string CardNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string CardHolderName { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [MaxLength(1)]
        public string CardStatus { get; set; }

        [Required]
        [MaxLength(1)]
        public string CardType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsBlocked { get; set; }

        [Required]
        [MaxLength(64)]
        public string PinHash { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "TRY";

        public bool IsVirtual { get; set; }

        public decimal CardLimit { get; set; }

        public decimal DailyLimit { get; set; } = 0;

        public decimal TransactionLimit { get; set; } = 0;

        public int FailedPinAttempts { get; set; } = 0;

        public DateTime? LastUsedAt { get; set; }

        [MaxLength(50)]
        public string CardProvider { get; set; }

        public string CardStatusChangeReason { get; set; }

        public string CardIssuerBank { get; set; }

        public int? ParentCardId { get; set; }

        // Foreign Key
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Navigasyon Property' si
        public Customer Customer { get; set; }
    }
}
