using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CardId { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(1)]
        public string Status { get; set; } = "S"; // S: Success, F: Failed, R: Review, V: Reversed
        public DateTime TransactionDate { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }
        public bool IsReversed { get; set; }
        public virtual Card? Card { get; set; }

        [MaxLength(64)]
        public string? AcquirerRef { get; set; }

        [MaxLength(64)]
        public string? SignatureHash { get; set; }
        public int FraudScore { get; set; }

        [Required]
        [MaxLength(1)]
        public string FraudDecision { get; set; } = "A"; // A: Approved, R: Review, B: Block
        public DateTime? FraudCheckedAt { get; set; }

    }
}
