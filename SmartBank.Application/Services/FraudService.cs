using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Transaction;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public sealed class FraudService : IFraudService
    {
        private readonly CustomerCoreDbContext _dbContext;
        public FraudService(CustomerCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FraudCheckResult> CheckAsync(Card card, CreateTransactionDto dto, decimal spentToday)
        {
            var result = new FraudCheckResult();
            var score = 0;

            // RULE 1: Gunluk limite cok yakin + yuksek tutar
            var willBeToday = spentToday + dto.Amount;
            if (willBeToday > card.DailyLimit * 0.9m)
            {
                score += 40;
                result.Reasons.Add("Günlük işlem limiti aşıldı.");
            }

            // RULE 2: Velocity check (son 2 dakikada 3+ deneme)
            var twoMinutesAgo = DateTime.Now.AddMinutes(-2);
            var recentAttempts = await _dbContext.Transactions
                .CountAsync(t => t.CardId == card.Id &&
                t.TransactionDate >= twoMinutesAgo);

            if (recentAttempts > 3)
            {
                score += 30;
                result.Reasons.Add("Kısa sürede çok sayıda işlem denemesi gerçekleşti.");
            }

            // RULE 3: Supheli aciklama (dummy)
            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.ToLower().Contains("test"))
            {
                score += 20;
                result.Reasons.Add("Şüpheli iş yeri bilgisi tespit edildi.");
            }

            // Decision
            result.Score = score;

            result.Decision =
                score >= 80 ? "B" :
                score >= 50 ? "R" :
                "A";

            return result;

        }
    }
}
