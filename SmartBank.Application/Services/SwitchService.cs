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
            // ---------- 0) Normalize ----------
            var cleanPan = (dto.PAN ?? string.Empty).Replace(" ", "");
            var bin = cleanPan.Length >= 6 ? cleanPan[..6] : cleanPan;
            var ccy = (dto.Currency ?? "TRY").ToUpperInvariant();
            var acquirer = dto.Acquirer ?? "DemoPOS";

            // ---------- 1) BIN → Issuer ----------
            var issuer = await _dbContext.CardBins
                .AsNoTracking()
                .Where(b => b.IsActive && b.Bin == bin)
                .Select(b => b.Issuer)
                .FirstOrDefaultAsync() ?? "Unknown";

            // ---------- 2) SwitchMessage: Pending ----------
            var msg = new SwitchMessage
            {
                PANMasked = MaskPan(cleanPan),
                Bin = bin,
                Amount = dto.Amount,
                Currency = ccy,
                Acquirer = acquirer,
                Issuer = issuer,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,

                // Dış sistem reference varsa buraya koymalısın (STAN/RRN). Örn. dto’ya ExternalId eklersen direkt ata.
                // Demo amaçlı: TxTime varsa onu ExternalId gibi kullanıyoruz (idempotency göstermek için).
                ExternalId = dto.TxnTime.HasValue ? dto.TxnTime.Value.Ticks.ToString() : null
            };

            _dbContext.SwitchMessages.Add(msg);
            await _dbContext.SaveChangesAsync();

            await LogAsync(msg.Id, "Received", "INFO", "Mesaj alındı", payloadIn: ToJson(dto));

            // ---------- 3) Sadece gerçek kartlara izin ----------
            // Tam PAN eşleşmesi (demo: CardNumber direkt tutuluyorsa; prod'da PAN hash kullanılmalı)
            var card = await _dbContext.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IsActive && c.CardNumber.Replace(" ", "") == cleanPan);

            if (issuer == "Unknown" || dto.Amount <= 0 || card == null)
            {
                msg.Status = "Declined";
                await _dbContext.SaveChangesAsync();
                await LogAsync(msg.Id, "Responded", "INFO", "Declined (issuer/amount/card rule)");
                return _mapper.Map<SelectSwitchMessageDto>(msg);
            }

            // Kart bulundu → bağla
            msg.CardId = card.Id;
            await _dbContext.SaveChangesAsync();

            // ---------- 4) Idempotency (Acquirer + ExternalId) ----------
            if (!string.IsNullOrWhiteSpace(msg.ExternalId))
            {
                bool dupApproved = await _dbContext.SwitchMessages.AsNoTracking().AnyAsync(x =>
                    x.Acquirer == msg.Acquirer &&
                    x.ExternalId == msg.ExternalId &&
                    x.Issuer == msg.Issuer &&
                    x.Amount == msg.Amount &&
                    x.Currency == msg.Currency &&
                    x.Status == "Approved");

                if (dupApproved)
                {
                    msg.Status = "Declined";
                    await _dbContext.SaveChangesAsync();
                    await LogAsync(msg.Id, "Responded", "INFO", "Declined (idempotent duplicate)");
                    return _mapper.Map<SelectSwitchMessageDto>(msg);
                }
            }

            // ---------- 5) Duplicate guard (aynı kart + aynı tutar/ccy + ±1 dk) ----------
            var txTime = dto.TxnTime ?? DateTime.UtcNow;
            bool existsTx = await _dbContext.Transactions.AsNoTracking().AnyAsync(t =>
                t.CardId == card.Id &&
                t.Amount == dto.Amount &&
                t.Currency == ccy &&
                t.TransactionDate >= txTime.AddMinutes(-1) &&
                t.TransactionDate <= txTime.AddMinutes(1));

            if (!existsTx)
            {
                var tx = new Transaction
                {
                    CardId = card.Id,
                    Currency = ccy,
                    Amount = dto.Amount,
                    Status = "S",
                    TransactionDate = txTime,
                    Description = dto.MerchantName ?? $"{acquirer}-{issuer}",
                    IsReversed = false,

                    // Transaction’a AcquirerRef alanını eklediysen:
                    AcquirerRef = msg.ExternalId
                };

                _dbContext.Transactions.Add(tx);
                await _dbContext.SaveChangesAsync();

                msg.TransactionId = tx.Id;
                await _dbContext.SaveChangesAsync();
            }

            // ---------- 6) Onayla ----------
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

        public async Task<List<object>> GetLogsAsync(int messageId)
        {
            return await _dbContext.SwitchLogs
                .AsNoTracking()
                .Where(l => l.MessageId == messageId)
                .OrderBy(l => l.Id)
                .Select(l => new
                {
                    l.Stage,
                    l.Level,
                    l.Note,
                    l.CreatedAt,
                    l.PayloadIn,
                    l.PayloadOut
                })
                .Cast<object>()
                .ToListAsync();
        }

        // ------------- helpers -------------
        private async Task LogAsync(int msgId, string stage, string level, string? note, string? payloadIn = null, string? payloadOut = null)
        {
            _dbContext.SwitchLogs.Add(new SwitchLog
            {
                MessageId = msgId,
                Stage = stage,
                Level = level,
                Note = note,
                PayloadIn = payloadIn,
                PayloadOut = payloadOut,
                CreatedAt = DateTime.UtcNow
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