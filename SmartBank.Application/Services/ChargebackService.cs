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

        private static string? Trunc(string? s, int len) => string.IsNullOrEmpty(s) ? s : (s.Length <= len ? s : s.Substring(0, len));

        public ChargebackService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private string NormalizeDecision(string input)
        {
            var key = (input ?? "").Trim().ToUpperInvariant();

            return key switch
            {
                "WON" => "Won",
                "CUSTOMER_WINS" => "Won",
                "MÜŞTERİ_KAZANDI" => "Won",

                "LOST" => "Lost",
                "MERCHANT_WINS" => "Lost",
                "İŞYERİ_KAZANDI" => "Lost",

                "CANCELLED" or "CANCELED" => "Cancelled",

                _ => throw new InvalidOperationException(
                        "Geçersiz decision. Kullanılabilecek değerler: Won, Lost, Cancelled " +
                        "(veya CUSTOMER_WINS / MERCHANT_WINS).")
            };
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
                MerchantName = Trunc(txn.Description, 100),
                ReasonCode = dto.ReasonCode,
                DisputedAmount = dto.DisputedAmount,
                ReplyBy = dto.ReplyBy,
                Note = dto.Note,
                Status = "Open",
                OpenedAt = DateTime.Now
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
            {
                var s = status.Trim().ToUpperInvariant();
                // c.Status NULL olabilir; güvenli karşılaştır
                q = q.Where(c => c.Status != null && c.Status.ToUpper() == s);
                // NOT: İstersen "OPEN" yerine enum da kullanabilirsin (Open/Won/Lost/Cancelled)
            }

            if (txnId.HasValue)
                q = q.Where(c => c.TransactionId == txnId.Value);

            var list = await q.OrderByDescending(c => c.Id).ToListAsync();

            return _mapper.Map<List<SelectChargebackCaseDto>>(list);
        }

        public async Task<SelectChargebackCaseDto?> GetCaseAsync(int caseId)
        {
            var cb = await _dbContext.Set<ChargebackCase>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == caseId);

            return cb == null ? null : _mapper.Map<SelectChargebackCaseDto>(cb);
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

            if (!string.Equals(cb.Status, "Open", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Case zaten kapalı. Sadece 'Open' durumundaki case'ler sonuçlandırılabilir.");

            // 👇 Artık burada direkt çağırıyoruz
            var decision = NormalizeDecision(dto.Decision);

            cb.Status = decision;
            cb.ClosedAt = DateTime.Now;

            _dbContext.Add(new ChargebackEvent { CaseId = cb.Id, Type = decision, Note = dto.Note });
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<SelectChargebackCaseDto>(cb);
        }
    }
}
