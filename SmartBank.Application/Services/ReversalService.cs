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

        public ReversalService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // 1) CREATE
        public async Task<bool> CreateReversalAsync(CreateReversalDto dto)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.Card)
                .FirstOrDefaultAsync(t => t.Id == dto.TransactionId);

            if (transaction == null)
                throw new InvalidOperationException("İlgili işlem bulunamadı.");

            if (transaction.IsReversed)
                throw new InvalidOperationException("Bu işlem zaten geri alınmış.");

            if (dto.ReversedAmount <= 0)
                throw new InvalidOperationException("Reversal tutarı sıfırdan büyük olmalıdır.");

            // Faz-1: sadece tam reversal
            if (dto.ReversedAmount != transaction.Amount)
                throw new InvalidOperationException("Şimdilik sadece tam reversal desteklenmektedir.");

            if (string.IsNullOrWhiteSpace(dto.PerformedBy))
                throw new InvalidOperationException("Reversal işlemi yapan kullanıcı zorunludur.");

            bool isCardLimitRestored = false;
            var card = transaction.Card;

            if (card != null && card.IsActive && !card.IsBlocked)
            {
                card.CardLimit += dto.ReversedAmount;
                card.UpdatedAt = DateTime.UtcNow;
                _dbContext.Cards.Update(card);
                isCardLimitRestored = true;
            }

            var reversal = new Reversal
            {
                TransactionId = dto.TransactionId,
                Reason = dto.Reason,
                ReversedAmount = dto.ReversedAmount,
                Status = "S",                           // Successful
                PerformedBy = dto.PerformedBy,
                ReversalDate = DateTime.UtcNow,
                ReversalSource = dto.ReversalSource,
                IsCardLimitRestored = isCardLimitRestored
            };

            // Tam reversal olduğu için işaretle
            transaction.IsReversed = true;
            _dbContext.Transactions.Update(transaction);

            _dbContext.Reversals.Add(reversal);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // 2) GET ALL
        public async Task<List<SelectReversalDto>> GetAllReversalsAsync()
        {
            var reversals = await _dbContext.Reversals
                .Where(r => r.Status != "V") // Void olmayanlar
                .ToListAsync();

            return _mapper.Map<List<SelectReversalDto>>(reversals);
        }

        // 3) GET BY ID
        public async Task<SelectReversalDto> GetReversalByIdAsync(int id)
        {
            var reversal = await _dbContext.Reversals
                .FirstOrDefaultAsync(r => r.Id == id && r.Status != "V");

            if (reversal == null)
                throw new InvalidOperationException("Reversal bulunamadı.");

            return _mapper.Map<SelectReversalDto>(reversal);
        }

        // 4) GET BY TRANSACTION ID
        public async Task<List<SelectReversalDto>> GetReversalsByTransactionIdAsync(int transactionId)
        {
            var reversals = await _dbContext.Reversals
                .Where(r => r.TransactionId == transactionId && r.Status != "V")
                .ToListAsync();

            return _mapper.Map<List<SelectReversalDto>>(reversals);
        }

        // 5) VOID (silmek yok; iptal/void)
        public async Task<bool> VoidReversalAsync(int id, string performedBy, string reason)
        {
            var reversal = await _dbContext.Reversals
                .Include(r => r.Transaction)
                .ThenInclude(t => t.Card)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reversal == null)
                throw new InvalidOperationException("Reversal bulunamadı.");

            if (reversal.Status == "V")
                return true; // zaten void

            // Kart limiti iade edilmişse geri düş
            if (reversal.IsCardLimitRestored && reversal.Transaction?.Card != null)
            {
                var card = reversal.Transaction.Card;

                if (card.CardLimit < reversal.ReversedAmount)
                    throw new InvalidOperationException("Kart limitinden düşülecek tutar yetersiz.");

                card.CardLimit -= reversal.ReversedAmount;
                card.UpdatedAt = DateTime.UtcNow;
                _dbContext.Cards.Update(card);

                reversal.IsCardLimitRestored = false;
            }

            reversal.Status = "V";
            reversal.VoidedBy = performedBy;
            reversal.VoidReason = reason;
            reversal.VoidedAt = DateTime.UtcNow;

            _dbContext.Reversals.Update(reversal);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
