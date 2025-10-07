using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBank.Domain.Entities
{
    public class ClearingRecord
    {
        public int Id { get; set; }

        // ilişki
        public int BatchId { get; set; }
        public ClearingBatch? Batch { get; set; }

        public int LineNumber { get; set; } // Dosyadaki satır no

        // eşleştirme
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }

        public int? CardId { get; set; }
        public Card? Card { get; set; }

        [MaxLength(4)]
        public string? CardLast4 { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "TRY";

        public DateTime? TransactionDate { get; set; }

        [MaxLength(100)]
        public string? MerchantName { get; set; }

        [MaxLength(1)]
        public string MatchStatus { get; set; } = "P"; // P=Pending, M=Matched, N=NotFound, X=Mismatch, E=Error

        [MaxLength(300)]
        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(64)]
        public string? SignatureHash { get; set; }
    }
}
