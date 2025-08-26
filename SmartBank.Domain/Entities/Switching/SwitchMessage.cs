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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? TransactionId { get; set; } // opsiyonel: Approved olursa dolduruyoruz
        public string? ExternalId { get; set; }  // STAN/RRN/AcquirerRef
        public int? CardId { get; set; }         // Onaylandıysa dolu
        public Card? Card { get; set; }
    }
}
