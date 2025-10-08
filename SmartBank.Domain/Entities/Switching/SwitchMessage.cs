using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities.Switching
{
    public class SwitchMessage
    {
        public int Id { get; set; }

        [MaxLength(19)]
        public string PANMasked { get; set; } = "";

        [MaxLength(6)]
        public string Bin { get; set; } = "";
        public decimal Amount { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "TRY";

        [MaxLength(50)]
        public string Acquirer { get; set; } = "";

        [MaxLength(50)]
        public string Issuer { get; set; } = "";

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending/Approved/Declined
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? TransactionId { get; set; }

        [Required]
        [MaxLength(128)]
        public string ExternalId { get; set; } = default!;
        public int? CardId { get; set; }
        public Card? Card { get; set; }
    }
}
