using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Transaction;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;
        public TransactionService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateTransactionAsync(CreateTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // 0) Baz doğrulamalar
            if (dto.Amount <= 0)
                throw new InvalidOperationException("İşlem tutarı sıfırdan büyük olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.Currency) || dto.Currency.Length != 3)
                throw new InvalidOperationException("Para birimi 3 haneli olmalıdır (örn: TRY).");

            // 1) Kart + durum kontrolü (tracking açık olmalı çünkü limiti güncelleyeceğiz)
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(c => c.Id == dto.CardId && c.IsActive);

            if (card == null)
                throw new InvalidOperationException("İşlem yapılacak kart bulunamadı veya pasif.");

            if (card.IsBlocked)
                throw new InvalidOperationException("Bu kart blokeli olduğundan işlem yapılamaz.");

            // 2) Günlük harcama (bugün) — sadece başarılı ve reverse edilmemiş işlemler
            var todayUtc = DateTime.Now.Date;
            var spentToday = await _dbContext.Transactions
                .Where(t => t.CardId == dto.CardId
                            && t.Status == "S"
                            && !t.IsReversed
                            && t.TransactionDate >= todayUtc
                            && t.TransactionDate < todayUtc.AddDays(1))
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;

            // 3) Limit kontrolleri (sıra ÖNEMLİ)
            // 3.1) Tek işlem limiti
            if (dto.Amount > card.TransactionLimit)
                throw new InvalidOperationException("İşlem tutarı, tek işlem limitini aşıyor.");

            // 3.2) Günlük limit (bugün harcanan + yeni işlem)
            var willBeToday = spentToday + dto.Amount;
            if (willBeToday > card.DailyLimit)
            {
                var kalan = card.DailyLimit - spentToday;
                throw new InvalidOperationException(
                    $"İşlem tutarı, günlük limiti aşıyor. Kalan günlük limit: {kalan:0.##}");
            }

            // 3.3) Kart bakiyesi (kalan limit)
            if (dto.Amount > card.CardLimit)
                throw new InvalidOperationException("Yetersiz bakiye.");

            // 4) Atomik işlem (kart limitini düş + transaction kaydını ekle)
            using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    CardId = dto.CardId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    Description = dto.Description,
                    TransactionDate = dto.TransactionDate == default ? DateTime.Now : dto.TransactionDate,
                    Status = "S",
                    IsReversed = false
                };

                card.CardLimit -= dto.Amount;
                card.UpdatedAt = DateTime.Now;

                _dbContext.Transactions.Add(transaction);
                _dbContext.Cards.Update(card);

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SelectTransactionDto>> GetAllTransactionsAsync()
        {
            var q = await _dbContext.Transactions
                .AsNoTracking()
                .Include(t => t.Card)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new SelectTransactionDto
                {
                    Id = t.Id,
                    CardId = t.CardId,
                    Currency = t.Currency,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Status = t.Status,
                    Description = t.Description,
                    Card = t.Card != null
                        ? (t.Card.Id + " - " + MaskPan(t.Card.CardNumber))
                        : ("#" + t.CardId)
                })
                .OrderBy(t => t.Id)
                .ToListAsync();

            return q;
        }

        public async Task<SelectTransactionDto> GetTransactionByIdAsync(int id)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if (transaction == null)
                throw new InvalidOperationException("İşlem bulunamadı.");

            return _mapper.Map<SelectTransactionDto>(transaction);
        }

        public async Task<List<SelectTransactionDto>> GetTransactionByCardIdAsync(int cardId)
        {
            if (cardId < 0)
                throw new ArgumentException("Geçersiz kart Id.");

            var cardExists = await _dbContext.Cards
                                            .AsNoTracking()
                                            .AnyAsync(c => c.Id == cardId && c.IsActive);

            if (!cardExists)
                throw new KeyNotFoundException("Kart bulunamadı veya pasif.");

            var transactions = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t=>t.CardId == cardId)
                .OrderByDescending(t=>t.TransactionDate)
                .ToListAsync();

            return _mapper.Map<List<SelectTransactionDto>>(transactions).ToList();
        }

        private static string MaskPan(string? pan)
        {
            if (string.IsNullOrWhiteSpace(pan)) return "";
            var digits = new string(pan.Where(char.IsDigit).ToArray());
            if (digits.Length <= 10) return digits;    // kısa pan: maskeleme uygulama
            var first6 = digits[..6];
            var last4 = digits[^4..];
            return first6 + new string('*', digits.Length - 10) + last4;
        }
    }
}
