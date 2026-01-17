using SmartBank.Application.DTOs.Transaction;
using SmartBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface IFraudService
    {
        Task<FraudCheckResult> CheckAsync(Card card, CreateTransactionDto dto, decimal spentToday);
    }

    public sealed class FraudCheckResult
    {
        public int Score { get; set; }
        public string Decision { get; set; } = "A"; // A: Approve, R: Review, B: Block
        public List<string> Reasons { get; set; } = new();
    }
}
