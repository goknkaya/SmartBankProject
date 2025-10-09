using System.Globalization;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Switching;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Domain.Entities.Switching;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class SwitchService : ISwitchService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public SwitchService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SelectSwitchMessageDto> ProcessMessageAsync(CreateSwitchMessageDto dto)
        {
            // 0) Normalize
            var cleanPan = (dto.PAN ?? "").Replace(" ", "");
            var bin = cleanPan.Length >= 6 ? cleanPan[..6] : cleanPan;
            var cur = (dto.Currency ?? "TRY").ToUpperInvariant();
            var txTime = (dto.TxnTime ?? DateTime.Now); // local time

            // 1) Issuer
            var issuer = await _dbContext.CardBins
                .Where(b => b.IsActive && b.Bin == bin)
                .Select(b => b.Issuer)
                .FirstOrDefaultAsync() ?? "Unknown";

            // -- 2) ExternalId: RRN/STAN/TID varsa onları kullan; yoksa dakika bazlı fallback
            string externalId;
            if (!string.IsNullOrWhiteSpace(dto.RRN))
            {
                externalId = $"{dto.Acquirer}|RRN:{dto.RRN.Trim()}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.TerminalId) && !string.IsNullOrWhiteSpace(dto.STAN))
            {
                externalId = $"{dto.Acquirer}|TID:{dto.TerminalId.Trim()}|STAN:{dto.STAN.Trim()}";
            }
            else
            {
                // Fallback (UI değişmeden idempotency sağlamak için dakika bazlı anahtar)
                // Aynı dakika içinde aynı PAN-bin + Amount + Currency + Acquirer => tek mesaj
                var minuteKey = (dto.TxnTime ?? DateTime.Now).ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
                externalId = string.Create(CultureInfo.InvariantCulture,
                    $"{dto.Acquirer}|{bin}|{dto.Amount:0.00}|{cur}|{minuteKey}");
            }

            // -- 3) Idempotency: varsa aynı kaydı dön
            var existing = await _dbContext.SwitchMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ExternalId == externalId);

            if (existing != null)
            {
                return _mapper.Map<SelectSwitchMessageDto>(existing);
            }

            // 4) Mesajı ekle
            var msg = new SwitchMessage
            {
                PANMasked = MaskPan(cleanPan),
                Bin = bin,
                Amount = dto.Amount,
                Currency = cur,
                Acquirer = string.IsNullOrWhiteSpace(dto.Acquirer) ? "DemoPOS" : dto.Acquirer.Trim(),
                Issuer = issuer,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                ExternalId = externalId
            };

            try
            {
                _dbContext.SwitchMessages.Add(msg);
                await _dbContext.SaveChangesAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627) // unique violation
            {
                var dup = await _dbContext.SwitchMessages.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ExternalId == externalId);
                if (dup != null)
                    return _mapper.Map<SelectSwitchMessageDto>(dup);

                // çok düşük olasılık fallback
                return new SelectSwitchMessageDto
                {
                    Id = 0,
                    PANMasked = msg.PANMasked,
                    Bin = msg.Bin,
                    Amount = msg.Amount,
                    Currency = msg.Currency,
                    Acquirer = msg.Acquirer,
                    Issuer = msg.Issuer,
                    Status = "Declined",
                    CreatedAt = DateTime.Now,
                    TransactionId = null
                };
            }

            await LogAsync(msg.Id, "Received", "INFO", "Mesaj alındı", payloadIn: ToJson(dto));

            // 5) Basit karar kuralları
            if (issuer == "Unknown" || dto.Amount <= 0)
            {
                msg.Status = "Declined";
                await _dbContext.SaveChangesAsync();
                await LogAsync(msg.Id, "Responded", "INFO", "Declined", payloadOut: "{\"status\":\"Declined\"}");
                return _mapper.Map<SelectSwitchMessageDto>(msg);
            }

            // 6) Kart eşleşmesi ve duplicate tx guard (demo: last4)
            var last4 = cleanPan.Length >= 4 ? cleanPan[^4..] : cleanPan;

            var card = await _dbContext.Cards.AsNoTracking()
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(c => c.CardNumber.Replace(" ", "").EndsWith(last4));

            if (card != null)
                msg.CardId = card.Id;

            if (card != null)
            {
                var existsByRef = await _dbContext.Transactions.AsNoTracking()
                    .AnyAsync(t => t.AcquirerRef == msg.ExternalId && t.CardId == card.Id);

                var existsByWindow = await _dbContext.Transactions.AsNoTracking()
                    .AnyAsync(t =>
                        t.CardId == card.Id &&
                        t.Amount == dto.Amount &&
                        t.Currency == cur &&
                        t.TransactionDate >= txTime.AddMinutes(-1) &&
                        t.TransactionDate <= txTime.AddMinutes(1));

                if (!existsByRef && !existsByWindow)
                {
                    var tx = new Transaction
                    {
                        CardId = card.Id,
                        Currency = cur,
                        Amount = dto.Amount,
                        Status = "S", // Success
                        TransactionDate = txTime,
                        Description = dto.MerchantName ?? $"{msg.Acquirer}-{issuer}",
                        IsReversed = false,
                        AcquirerRef = msg.ExternalId
                    };

                    _dbContext.Transactions.Add(tx);
                    await _dbContext.SaveChangesAsync();

                    msg.TransactionId = tx.Id;
                }
            }

            // 7) Karar & log
            msg.Status = "Approved";
            await _dbContext.SaveChangesAsync();
            await LogAsync(msg.Id, "Responded", "INFO", "Approved", payloadOut: "{\"status\":\"Approved\"}");

            return _mapper.Map<SelectSwitchMessageDto>(msg);
        }

        public async Task<List<SelectSwitchMessageDto>> GetMessagesAsync(string? status = null, string? issuer = null)
        {
            var q = _dbContext.SwitchMessages.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(x => x.Status == status);
            if (!string.IsNullOrWhiteSpace(issuer)) q = q.Where(x => x.Issuer == issuer);

            var list = await q.OrderByDescending(x => x.Id).ToListAsync();
            return _mapper.Map<List<SelectSwitchMessageDto>>(list);
        }

        public async Task<List<SwitchLogDto>> GetLogsAsync(int messageId)
        {
            return await _dbContext.SwitchLogs
                .AsNoTracking()
                .Where(l => l.MessageId == messageId)
                .OrderBy(l => l.Id)
                .Select(l => new SwitchLogDto
                {
                    Stage = l.Stage,
                    Level = l.Level,
                    Note = l.Note,
                    CreatedAt = l.CreatedAt,
                    PayloadIn = l.PayloadIn,
                    PayloadOut = l.PayloadOut
                })
                .ToListAsync();
        }

        // ------------- helpers -------------
        private async Task LogAsync(int msgId, string stage, string level, string? note,
                                    string? payloadIn = null, string? payloadOut = null)
        {
            _dbContext.SwitchLogs.Add(new SwitchLog
            {
                MessageId = msgId,
                Stage = stage,
                Level = level,
                Note = note,
                PayloadIn = payloadIn,
                PayloadOut = payloadOut,
                CreatedAt = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
        }

        private static string MaskPan(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan)) return "";
            var p = pan.Replace(" ", "");
            if (p.Length <= 4) return new string('*', Math.Max(0, p.Length)) + p;
            return new string('*', p.Length - 4) + p[^4..];
        }

        private static string ToJson(object obj) =>
            JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
    }
}
