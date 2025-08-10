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
            // 1. Kart var mı kontrolü
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == dto.CardId && c.IsActive);
            if (card == null)
                throw new InvalidOperationException("İşlem yapılacak kart bulunamadı veya müşteriye ait değil.");

            // 2. Kart blokeli mi kontrolü
            if (card.IsBlocked)
                throw new InvalidOperationException("Bu kart blokeli olduğundan işlem yapılamaz.");

            // 3. Limit kontrolleri
            if (dto.Amount > card.TransactionLimit)
                throw new InvalidOperationException("İşlem tutarı, tek işlem limitini aşıyor.");

            if (dto.Amount > card.DailyLimit)
                throw new InvalidOperationException("İşlem tutarı, günlük limiti aşıyor.");

            if (dto.Amount > card.CardLimit)
                throw new InvalidOperationException("Yetersiz bakiye.");

            // 4. İşlem oluştur
            var transaction = new Transaction
            {
                CardId = dto.CardId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                Status = "S", //Successfull
                IsReversed = false
            };

            // 5. Bakiyeden düş
            card.CardLimit -= dto.Amount;
            card.UpdatedAt = DateTime.UtcNow;

            _dbContext.Transactions.Add(transaction);
            _dbContext.Cards.Update(card);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<SelectTransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _dbContext.Transactions.ToListAsync();

            return _mapper.Map<List<SelectTransactionDto>>(transactions);
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
            var transactions = await _dbContext.Transactions
                .Where(t=>t.CardId == cardId)
                .ToListAsync();

            return _mapper.Map<List<SelectTransactionDto>>(transactions).ToList();
        }
    }
}
