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
            // result
            var reasons = new List<string>();
            var desc = (dto.Description ?? "").ToLowerInvariant();

            // ---------- Flags ----------
            var hasCrypto = desc.Contains("crypto") || desc.Contains("kripto");
            var hasSuspicious = desc.Contains("suspicious") || desc.Contains("şüpheli");
            var hasRapidKeyword = desc.Contains("rapid") || desc.Contains("attempt");

            // Amount ratio (limit oranı)
            bool highAmount = false;
            bool veryHighAmount = false;

            if (card.TransactionLimit > 0)
            {
                var ratio = dto.Amount / card.TransactionLimit; // decimal
                veryHighAmount = ratio >= 0.90m;
                highAmount = ratio >= 0.70m;

                if (veryHighAmount) reasons.Add("Tek işlem limitine çok yakın");
                else if (highAmount) reasons.Add("Yüksek tutarlı işlem");
            }

            if (hasCrypto) reasons.Add("Kripto/şüpheli açıklama");
            if (hasSuspicious) reasons.Add("Şüpheli işyeri/açıklama");
            if (hasRapidKeyword) reasons.Add("Hızlı deneme paterni");

            // ---------- Velocity (son 60 sn - SERVER clock) ----------
            var now = DateTime.Now;
            var from = now.AddSeconds(-60);

            // Not: Velocity, server-time TransactionDate ile anlamlı çalışır.
            var countLast60s = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.CardId == dto.CardId
                            && t.TransactionDate >= from
                            && t.TransactionDate <= now)
                .CountAsync();

            var attempts = countLast60s + 1;

            bool velocityReview = attempts >= 3; // 3+ deneme -> review
            bool velocityBlock = attempts >= 6;  // 6+ deneme -> block

            if (velocityReview) reasons.Add("Son 60 sn içinde 3+ deneme (velocity)");
            if (velocityBlock) reasons.Add("Son 60 sn içinde 6+ deneme (yüksek risk)");

            // ---------- Score (info amaçlı) ----------
            var score = 0;
            if (hasCrypto) score += 40;
            if (hasSuspicious) score += 30;
            if (hasRapidKeyword) score += 20;

            if (veryHighAmount) score += 35;
            else if (highAmount) score += 20;

            if (velocityReview) score += 25;
            if (velocityBlock) score += 45;

            // ---------- Decision (ÖNCELİK: velocity > review > approve) ----------
            string decision;
            if (velocityBlock)
                decision = "B";
            else if (velocityReview || hasCrypto || hasSuspicious || highAmount)
                decision = "R";
            else
                decision = "A";

            return new FraudCheckResult
            {
                Score = score,
                Decision = decision,
                Reasons = reasons
            };
        }
    }
}
