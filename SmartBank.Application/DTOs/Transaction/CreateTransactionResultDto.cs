using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Transaction
{
    public sealed class CreateTransactionResultDto
    {
        public int TransactionId { get; init; }
        public string Status { get; init; } = "S";          // S: Success, R: Review, B: Blocked
        public int FraudScore { get; init; }
        public string FraudDecision { get; init; } = "A";   // A: Approve, R: Review, B: Block
        public string Message { get; init; } = string.Empty;

    }
}
