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
            // -- 0) Normalize
            var cleanPan = (dto.PAN ?? "").Replace(" ", "");
            var bin = cleanPan.Length >= 6 ? cleanPan[..6] : cleanPan;
            var cur = (dto.Currency ?? "TRY").ToUpperInvariant();
            var txTime = (dto.TxnTime ?? dto.TxnTime ?? DateTime.Now).ToUniversalTime();

            // -- 1) Issuer
            var issuer = await _dbContext.CardBins
                .Where(b => b.IsActive && b.Bin == bin)
                .Select(b => b.Issuer)
                .FirstOrDefaultAsync() ?? "Unknown";

            // -- 2) ExternalId (aynı body -> aynı id)
            var externalId = $"{dto.Acquirer}|{bin}|{dto.Amount:0.00}|{cur}|{txTime:yyyyMMddHHmmss}";

            // -- 3) Idempotency (uygulama katmanı) — insert'ten ÖNCE
            var alreadyExists = await _dbContext.SwitchMessages.AsNoTracking().AnyAsync(x =>
                x.Acquirer == dto.Acquirer &&
                x.ExternalId == externalId
            // .Status == "Approved"  // sadece Approved'a sıkılamak istersen aç
            );

            if (alreadyExists)
            {
                return new SelectSwitchMessageDto
                {
                    Id = 0,
                    PANMasked = MaskPan(cleanPan),
                    Bin = bin,
                    Amount = dto.Amount,
                    Currency = cur,
                    Acquirer = dto.Acquirer,
                    Issuer = issuer,
                    Status = "Declined",
                    CreatedAt = DateTime.Now,
                    TransactionId = null
                };
            }

            // -- 4) Mesajı ekle (DB unique index fallback'i ile)
            var msg = new SwitchMessage
            {
                PANMasked = MaskPan(cleanPan),
                Bin = bin,
                Amount = dto.Amount,
                Currency = dto.Currency.ToUpperInvariant(),
                Acquirer = dto.Acquirer,
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
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
            {
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

            await LogAsync(msg.Id, "Received", "INFO", "Mesaj alındı",
                payloadIn: System.Text.Json.JsonSerializer.Serialize(dto));

            // -- 5) Basit kurallar
            if (issuer == "Unknown" || dto.Amount <= 0)
            {
                msg.Status = "Declined";
                await _dbContext.SaveChangesAsync();
                await LogAsync(msg.Id, "Responded", "INFO", "Declined");
                return _mapper.Map<SelectSwitchMessageDto>(msg);
            }

            // -- 6) Kart ve muhtemel tx duplicate guard
            // (Gerçekte tam PAN ile eşleştir; demo için last4).
            string last4 = cleanPan.Length >= 4 ? cleanPan[^4..] : cleanPan;
            var card = await _dbContext.Cards.AsNoTracking()
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(c => c.CardNumber.Replace(" ", "").EndsWith(last4));

            if (card != null)
                msg.CardId = card.Id;

            if (card != null)
            {
                var existsTx = await _dbContext.Transactions.AsNoTracking().AnyAsync(t =>
                    t.CardId == card.Id &&
                    t.Amount == dto.Amount &&
                    t.Currency == cur &&
                    t.TransactionDate >= txTime.AddMinutes(-1) &&
                    t.TransactionDate <= txTime.AddMinutes(1));

                if (!existsTx)
                {
                    var tx = new Transaction
                    {
                        CardId = card.Id,
                        Currency = cur,
                        Amount = dto.Amount,
                        Status = "S",
                        TransactionDate = txTime,
                        Description = dto.MerchantName ?? $"{dto.Acquirer}-{issuer}",
                        IsReversed = false,
                        AcquirerRef = msg.ExternalId
                    };
                    _dbContext.Transactions.Add(tx);
                    await _dbContext.SaveChangesAsync();
                    msg.TransactionId = tx.Id;
                }
            }

            // -- 7) Son kararı kaydet
            msg.Status = "Approved";
            await _dbContext.SaveChangesAsync();
            await LogAsync(msg.Id, "Responded", "INFO", "Approved",
                payloadOut: "{\"status\":\"Approved\"}");

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