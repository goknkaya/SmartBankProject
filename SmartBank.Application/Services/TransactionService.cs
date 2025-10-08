using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Transaction;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;
using SmartBank.Application.Services; // SignatureUtil için

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

            // 1) Kart + durum kontrolü
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(c => c.Id == dto.CardId && c.IsActive);

            if (card == null)
                throw new InvalidOperationException("İşlem yapılacak kart bulunamadı veya pasif.");

            if (card.IsBlocked)
                throw new InvalidOperationException("Bu kart blokeli olduğundan işlem yapılamaz.");

            // 2) Günlük harcama (bugün)
            var todayUtc = DateTime.Now.Date;
            var spentToday = await _dbContext.Transactions
                .Where(t => t.CardId == dto.CardId
                            && t.Status == "S"
                            && !t.IsReversed
                            && t.TransactionDate >= todayUtc
                            && t.TransactionDate < todayUtc.AddDays(1))
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;

            // 3) Limit kontrolleri
            if (dto.Amount > card.TransactionLimit)
                throw new InvalidOperationException("İşlem tutarı, tek işlem limitini aşıyor.");

            var willBeToday = spentToday + dto.Amount;
            if (willBeToday > card.DailyLimit)
            {
                var kalan = card.DailyLimit - spentToday;
                throw new InvalidOperationException(
                    $"İşlem tutarı, günlük limiti aşıyor. Kalan günlük limit: {kalan:0.##}");
            }

            if (dto.Amount > card.CardLimit)
                throw new InvalidOperationException("Yetersiz bakiye.");

            // 4) Atomik işlem
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

                // >>> NEW: SignatureHash doldur
                transaction.SignatureHash = SignatureUtil.ComputeFromPan(
                    pan: card.CardNumber,                   // PAN varsa; maskeli bile olsa util son 4’ü normalize ediyor
                    amount: transaction.Amount,
                    currency: transaction.Currency,
                    txDate: transaction.TransactionDate,
                    merchant: transaction.Description
                );

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
            var list = await _dbContext.Transactions
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
                .ToListAsync();

            return list;
        }

        public async Task<SelectTransactionDto?> GetTransactionByIdAsync(int id)
        {
            var t = await _dbContext.Transactions
                .AsNoTracking()
                .Include(x => x.Card)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return null;

            return new SelectTransactionDto
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
            };
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
