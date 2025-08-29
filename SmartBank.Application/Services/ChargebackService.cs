using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Chargeback;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities.Chargeback;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class ChargebackService : IChargebackService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChargebackService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SelectChargebackCaseDto> OpenAsync(CreateChargebackDto dto)
        {
            var txn = await _dbContext.Transactions
                .AsNoTracking()
                .Include(t => t.Card)
                .FirstOrDefaultAsync(t => t.Id == dto.TransactionId)
                ?? throw new InvalidOperationException("Transaction bulunamadı.");

            // Basit: aynı txn için açık bir case varsa yaratma (idempotency)
            var existOpen = await _dbContext.Set<ChargebackCase>()
                .AnyAsync(c => c.TransactionId == dto.TransactionId && c.Status == "Open");

            if (existOpen)
                throw new InvalidOperationException("Bu işlem için zaten açık bir chargeback mevcut.");

            var cb = new ChargebackCase
            {
                TransactionId = txn.Id,
                Currency = txn.Currency,
                TransactionAmount = txn.Amount,
                MerchantName = txn.Description,
                ReasonCode = dto.ReasonCode,
                DisputedAmount = dto.DisputedAmount,
                ReplyBy = dto.ReplyBy,
                Note = dto.Note,
                Status = "Open",
                OpenedAt = DateTime.UtcNow
            };

            _dbContext.Add(cb);
            await _dbContext.SaveChangesAsync();

            _dbContext.Add(new ChargebackEvent { CaseId = cb.Id, Type = "Opened", Note = dto.Note });
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<SelectChargebackCaseDto>(cb);
        }

        public async Task<List<SelectChargebackCaseDto>> ListAsync(string? status = null, int? txnId = null)
        {
            var q = _dbContext.Set<ChargebackCase>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(c => c.Status == status);

            if (txnId.HasValue)
                q = q.Where(c => c.TransactionId == txnId.Value);

            var list = await q.OrderByDescending(c => c.Id).ToListAsync();

            return _mapper.Map<List<SelectChargebackCaseDto>>(list);
        }

        public async Task<List<SelectChargebackEventDto>> GetEventsAsync(int caseId)
        {
            var evs = await _dbContext.Set<ChargebackEvent>()
                .AsNoTracking()
                .Where(e =>  e.CaseId == caseId)
                .OrderBy(e => e.Id)
                .ToListAsync();

            return _mapper.Map<List<SelectChargebackEventDto>>(evs);
        }

        public async Task<SelectChargebackCaseDto> AddEvidenceAsync(int caseId, AddEvidenceDto dto)
        {
            var cb = await _dbContext.Set<ChargebackCase>().FirstOrDefaultAsync(c => c.Id == caseId)
                ?? throw new InvalidOperationException("Case bulunamadı.");
            if (cb.Status != "Open")
                throw new InvalidOperationException("Sadece Open durumunda kanıt eklenebilir.");

            _dbContext.Add(new ChargebackEvent
            {
                CaseId = cb.Id,
                Type = "EvidenceAdded",
                Note = dto.Note,
                EvidenceUrl = dto.EvidenceUrl
            });
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<SelectChargebackCaseDto>(cb);
        }

        public async Task<SelectChargebackCaseDto> DecideAsync(int caseId, DecideChargebackDto dto)
        {
            var cb = await _dbContext.Set<ChargebackCase>().FirstOrDefaultAsync(c => c.Id == caseId)
                ?? throw new InvalidOperationException("Case bulunamadı.");
            if (cb.Status != "Open")
                throw new InvalidOperationException("Zaten kapalı bir case.");

            var decision = dto.Decision switch
            {
                "Won" => "Won",
                "Lost" => "Lost",
                "Cancelled" => "Cancelled",
                _ => throw new InvalidOperationException("Geçersiz karar.")
            };

            cb.Status = decision;
            cb.ClosedAt = DateTime.UtcNow;

            _dbContext.Add(new ChargebackEvent { CaseId = cb.Id, Type = decision, Note = dto.Note });
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<SelectChargebackCaseDto>(cb);
        }
    }
}
