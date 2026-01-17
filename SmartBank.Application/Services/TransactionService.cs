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
        private readonly IFraudService _fraudService;
        public TransactionService(CustomerCoreDbContext dbContext, IMapper mapper, IFraudService fraudService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _fraudService = fraudService;
        }
        public async Task<CreateTransactionResultDto> CreateTransactionAsync(CreateTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // 0) Baz doğrulamalar
            if (dto.Amount <= 0)
                throw new InvalidOperationException("İşlem tutarı sıfırdan büyük olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.Currency) || dto.Currency.Length != 3)
                throw new InvalidOperationException("Para birimi 3 haneli olmalıdır (örn: TRY).");

            // Status / Decision sabitleri (magic string olmasın)
            const string TxApproved = "S";
            const string TxReview = "R";
            const string TxBlocked = "B";

            const string FraudApprove = "A";
            const string FraudReview = "R";
            const string FraudBlock = "B";

            // 1) Kart + durum kontrolü
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(c => c.Id == dto.CardId && c.IsActive);

            if (card == null)
                throw new InvalidOperationException("İşlem yapılacak kart bulunamadı veya pasif.");

            if (card.IsBlocked)
                throw new InvalidOperationException("Bu kart blokeli olduğundan işlem yapılamaz.");

            // 2) Günlük harcama (bugün) - sadece APPROVED işlemler sayılır
            var today = DateTime.Now.Date;
            var spentToday = await _dbContext.Transactions
                .Where(t => t.CardId == dto.CardId
                            && t.Status == TxApproved
                            && !t.IsReversed
                            && t.TransactionDate >= today
                            && t.TransactionDate < today.AddDays(1))
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

            // 4) Fraud kontrolü (DB transaction'a girmeden)
            var fraudResult = await _fraudService.CheckAsync(card, dto, spentToday);

            // 5) Atomik işlem
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
                    IsReversed = false,

                    FraudScore = fraudResult.Score,
                    FraudDecision = fraudResult.Decision,
                    FraudCheckedAt = DateTime.Now
                };

                // Status (fraud kararına göre)
                transaction.Status =
                    fraudResult.Decision == FraudApprove ? TxApproved :
                    fraudResult.Decision == FraudReview ? TxReview :
                    TxBlocked;

                // SignatureHash
                transaction.SignatureHash = SignatureUtil.ComputeFromPan(
                    pan: card.CardNumber,
                    amount: transaction.Amount,
                    currency: transaction.Currency,
                    txDate: transaction.TransactionDate,
                    merchant: transaction.Description
                );

                // Sadece approved ise bakiye düş
                if (fraudResult.Decision == FraudApprove)
                {
                    card.CardLimit -= dto.Amount;
                    card.UpdatedAt = DateTime.Now;
                    _dbContext.Cards.Update(card);
                }

                _dbContext.Transactions.Add(transaction);

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();

                // ✅ Artık bool değil, detaylı sonuç dönüyoruz
                return new CreateTransactionResultDto
                {
                    TransactionId = transaction.Id,
                    Status = transaction.Status,
                    FraudScore = transaction.FraudScore,
                    FraudDecision = transaction.FraudDecision,
                    Message = transaction.Status == TxApproved
                        ? "Onaylandı"
                        : transaction.Status == TxReview
                            ? "İşlem incelemeye alındı"
                            : "İşlem güvenlik nedeniyle engellendi"
                };
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
                .Where(t => t.CardId == cardId)
                .OrderByDescending(t => t.TransactionDate)
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
