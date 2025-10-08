using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Reversal;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class ReversalService : IReversalService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        // Ayrı sınıf açmadan, lokal sabitler:
        private const string StatusSuccess = "S";
        private const string StatusVoid = "V";

        public ReversalService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // 1) CREATE
        public async Task<bool> CreateReversalAsync(CreateReversalDto dto)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();

            // 1) İşlem var mı?
            var txEntity = await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == dto.TransactionId);

            if (txEntity == null)
                throw new InvalidOperationException("İlgili işlem bulunamadı.");

            if (txEntity.IsReversed)
                throw new InvalidOperationException("Bu işlem zaten geri alınmış.");

            // 2) Validasyonlar
            if (dto.ReversedAmount <= 0)
                throw new InvalidOperationException("Reversal tutarı sıfırdan büyük olmalıdır.");

            // Faz-1: sadece tam reversal
            if (dto.ReversedAmount != txEntity.Amount)
                throw new InvalidOperationException("Şimdilik sadece tam reversal desteklenmektedir.");

            if (string.IsNullOrWhiteSpace(dto.PerformedBy))
                throw new InvalidOperationException("Reversal işlemi yapan kullanıcı zorunludur.");

            if (Math.Round(dto.ReversedAmount, 2) != Math.Round(txEntity.Amount, 2))
                throw new InvalidOperationException("Şimdilik sadece tam reversal desteklenmektedir.");

            // 3) Kart limit iadesi
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == txEntity.CardId);
            bool isCardLimitRestored = false;

            if (card != null && card.IsActive && !card.IsBlocked)
            {
                card.CardLimit += dto.ReversedAmount;   // iade
                card.UpdatedAt = DateTime.Now;
                _dbContext.Cards.Update(card);
                isCardLimitRestored = true;
            }

            // 4) Transaction.IsReversed işaretle (attach + partial update)
            var txToUpdate = new Transaction { Id = txEntity.Id, IsReversed = true };
            _dbContext.Transactions.Attach(txToUpdate);
            _dbContext.Entry(txToUpdate).Property(x => x.IsReversed).IsModified = true;

            // 5) Reversal kaydı
            var reversal = new Reversal
            {
                TransactionId = txEntity.Id,
                Reason = dto.Reason,
                ReversedAmount = dto.ReversedAmount,
                Status = StatusSuccess,
                PerformedBy = dto.PerformedBy,
                ReversalDate = DateTime.Now,
                ReversalSource = dto.ReversalSource,
                IsCardLimitRestored = isCardLimitRestored
            };
            _dbContext.Reversals.Add(reversal);

            // 6) Kaydet & commit
            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }

        // 2) GET ALL
        public async Task<List<SelectReversalDto>> GetAllReversalsAsync()
        {
            var reversals = await _dbContext.Reversals
                .Where(r => !r.IsDeleted)         // soft delete gizle
                .OrderByDescending(r => r.Id)     // en yeniler üstte
                .ToListAsync();

            return _mapper.Map<List<SelectReversalDto>>(reversals);
        }

        // 3) GET BY ID
        public async Task<SelectReversalDto> GetReversalByIdAsync(int id)
        {
            var reversal = await _dbContext.Reversals
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (reversal == null)
                throw new InvalidOperationException("Reversal bulunamadı.");

            return _mapper.Map<SelectReversalDto>(reversal);
        }

        // 4) GET BY TRANSACTION ID
        public async Task<List<SelectReversalDto>> GetReversalsByTransactionIdAsync(int transactionId)
        {
            var all = await _dbContext.Reversals
                .Where(r => r.TransactionId == transactionId && !r.IsDeleted)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            if (!all.Any())
                throw new InvalidOperationException("Bu işlem için hiç reversal bulunamadı.");

            // kullanıcıya sadece başarılı (void olmayan) reversallar
            var successOnly = all.Where(r => r.Status != StatusVoid).ToList();

            if (!successOnly.Any())
                throw new InvalidOperationException("Bu işlem için reversal var ancak iptal edilmiş durumda.");

            return _mapper.Map<List<SelectReversalDto>>(successOnly);
        }

        // 5) VOID
        public async Task<bool> VoidReversalAsync(int id, string performedBy, string reason)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();

            var reversal = await _dbContext.Reversals
                .Include(r => r.Transaction)
                .ThenInclude(t => t.Card)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reversal == null)
                throw new InvalidOperationException("Reversal bulunamadı.");

            if (reversal.Status == StatusVoid)
            {
                await tx.RollbackAsync(); // no-op, açıklık için
                return true; // zaten void
            }

            // Kart limiti iade edilmişse geri düş
            if (reversal.IsCardLimitRestored && reversal.Transaction?.Card != null)
            {
                var card = reversal.Transaction.Card;

                if (card.CardLimit < reversal.ReversedAmount)
                    throw new InvalidOperationException("Kart limitinden düşülecek tutar yetersiz.");

                card.CardLimit -= reversal.ReversedAmount;
                card.UpdatedAt = DateTime.Now;
                _dbContext.Cards.Update(card);

                reversal.IsCardLimitRestored = false;
            }

            // İşlemi reversed=false yap
            if (reversal.Transaction != null)
            {
                reversal.Transaction.IsReversed = false;
                _dbContext.Transactions.Update(reversal.Transaction);
            }

            // Reversal'ı void et
            reversal.Status = StatusVoid;
            reversal.VoidedBy = string.IsNullOrWhiteSpace(performedBy) ? "user" : performedBy.Trim();
            reversal.VoidReason = reason ?? string.Empty;
            reversal.VoidedAt = DateTime.Now;

            _dbContext.Reversals.Update(reversal);

            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }
    }
}
