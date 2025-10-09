// SmartBank.Application/Services/ClearingService.cs
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Clearing;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class ClearingService : IClearingService
    {
        private readonly CustomerCoreDbContext _db;
        private readonly IMapper _mapper;

        public ClearingService(CustomerCoreDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // ========== 1) IN - Dosya Yükle & İşle ==========
        public async Task<SelectClearingBatchDto> UploadIncomingAsync(IncomingUploadRequest req)
        {
            if (req.File == null || req.File.Length == 0)
                throw new ApplicationException("Yüklenecek dosya bulunamadı.");

            // Dosya hash'i (aynı dosyayı ikinci kez işleme koruması)
            var hash = await ComputeSha256Async(req.File);
            if (!string.IsNullOrWhiteSpace(req.FileHash) &&
                !hash.Equals(req.FileHash, StringComparison.OrdinalIgnoreCase))
                throw new ApplicationException("FileHash doğrulaması başarısız.");

            var isDup = await _db.ClearingBatches.AnyAsync(b => b.FileHash == hash && b.Direction == "IN");
            if (isDup)
                throw new ApplicationException("Bu dosya daha önce işlenmiş görünüyor.");

            var batch = new ClearingBatch
            {
                Direction = "IN",
                FileName = string.IsNullOrWhiteSpace(req.FileName) ? req.File.FileName : req.FileName!,
                FileHash = hash,
                SettlementDate = req.SettlementDate.Date,
                Status = "N",
                CreatedAt = DateTime.Now,
                Notes = req.Notes
            };

            using var trx = await _db.Database.BeginTransactionAsync();

            _db.ClearingBatches.Add(batch);
            await _db.SaveChangesAsync();

            // CSV → ClearingRecords kayıtları (başlangıçta P/E)
            var records = await ParseAndPersistAsync(req.File, batch.Id);
            batch.TotalCount = records.Count;

            // Satır bazında eşleştir (M/N/E'ye düşür)
            foreach (var r in records)
                await TryMatchAsync(r);

            // ===== ÇAKIŞMA (X) İLK YÜKLEMEDE DE YAKALANSIN =====
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sql &&
                (sql.Number == 2601 || sql.Number == 2627)
            )
            {
                // Çakışma kontrolünü bellekte yap
                var conflicts = (await _db.ClearingRecords
                    .Where(r => r.BatchId == batch.Id && r.TransactionId != null)
                    .ToListAsync())
                    .GroupBy(r => r.TransactionId)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .ToList();

                foreach (var c in conflicts)
                {
                    c.MatchStatus = "X";
                    c.TransactionId = null;
                    c.CardId = null;
                    c.ErrorMessage = "Aynı Transaction bu batch içinde birden fazla satıra bağlanmaya çalıştı.";
                }

                await _db.SaveChangesAsync();
            }

            // Sayıları güvenle DB üzerinden hesapla (X dahil başarısızlara girer)
            var total = await _db.ClearingRecords.CountAsync(r => r.BatchId == batch.Id);
            var matched = await _db.ClearingRecords.CountAsync(r => r.BatchId == batch.Id && r.MatchStatus == "M");

            batch.TotalCount = total;
            batch.SuccessCount = matched;
            batch.FailCount = total - matched; // N + E + X

            batch.Status = "P";
            batch.ProcessedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            await trx.CommitAsync();

            return _mapper.Map<SelectClearingBatchDto>(batch);
        }

        // ========== 2) OUT - Dosya Üret ==========
        public async Task<(SelectClearingBatchDto batch, byte[] fileBytes, string fileName)>
            GenerateOutgoingAsync(DateTime settlementDate)
        {
            var day = settlementDate.Date;

            var txs = await _db.Transactions
                .AsNoTracking()
                .Include(t => t.Card)
                .Where(t => t.Status == "S"
                            && t.TransactionDate >= day
                            && t.TransactionDate < day.AddDays(1))
                .OrderBy(t => t.Id)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("CardLast4;Amount;Currency;TransactionDate;MerchantName;SignatureHash");

            foreach (var t in txs)
            {
                var last4 = GetLast4(t.Card?.CardNumber);
                var sig = string.IsNullOrWhiteSpace(t.SignatureHash)
                    ? ComputeSignature(last4, t.Amount, t.Currency, t.TransactionDate, t.Description)
                    : t.SignatureHash;

                sb.AppendLine($"{last4};{t.Amount};{t.Currency};{t.TransactionDate:yyyy-MM-ddTHH:mm:ss};{t.Description};{sig}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"out_{day:yyyyMMdd}.csv";

            var batch = new ClearingBatch
            {
                Direction = "OUT",
                FileName = fileName,
                FileHash = ComputeSha256(bytes),
                SettlementDate = day,
                TotalCount = txs.Count,
                SuccessCount = txs.Count,
                FailCount = 0,
                Status = "P",
                CreatedAt = DateTime.Now,
                ProcessedAt = DateTime.Now
            };

            _db.ClearingBatches.Add(batch);
            await _db.SaveChangesAsync();

            var dto = _mapper.Map<SelectClearingBatchDto>(batch);
            return (dto, bytes, fileName);
        }

        // ========== 3) Sorgular ==========
        public async Task<SelectClearingBatchDto?> GetBatchAsync(int batchId)
        {
            var batch = await _db.ClearingBatches.FindAsync(batchId);
            return batch == null ? null : _mapper.Map<SelectClearingBatchDto>(batch);
        }

        public async Task<List<SelectClearingBatchDto>> GetBatchesAsync(string? direction = null, DateTime? settlementDate = null)
        {
            var q = _db.ClearingBatches.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(direction))
                q = q.Where(b => b.Direction == direction);

            if (settlementDate.HasValue)
            {
                var d = settlementDate.Value.Date;
                q = q.Where(b => b.SettlementDate == d);
            }

            var list = await q.OrderByDescending(b => b.CreatedAt).ToListAsync();
            return _mapper.Map<List<SelectClearingBatchDto>>(list);
        }

        public async Task<List<SelectClearingRecordDto>> GetRecordsAsync(int batchId, string? matchStatus = null)
        {
            var q = _db.ClearingRecords.AsNoTracking().Where(r => r.BatchId == batchId);
            if (!string.IsNullOrWhiteSpace(matchStatus))
                q = q.Where(r => r.MatchStatus == matchStatus);

            var list = await q.OrderBy(r => r.LineNumber).ToListAsync();
            return _mapper.Map<List<SelectClearingRecordDto>>(list);
        }

        // ========== 4) Retry (N/E/X) ==========
        public async Task<int> RetryUnmatchedAsync(int batchId)
        {
            var recs = await _db.ClearingRecords
                .Where(r => r.BatchId == batchId &&
                           (r.MatchStatus == "N" || r.MatchStatus == "X" || r.MatchStatus == "E"))
                .OrderBy(r => r.LineNumber)
                .ToListAsync();

            var count = 0;
            foreach (var r in recs)
            {
                await TryMatchAsync(r);
                if (r.MatchStatus == "M") count++;
            }

            // eşleştirme güncellemelerini yaz
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627)) // 2601 unique index ihlali, 2627 unique constraint ihlali 
            {
                // Unique index hatası: aynı batch + aynı TransactionId çakışması
                // => İlgili satırları X (conflict) olarak işaretle
                var conflicts = await _db.ClearingRecords
                    .Where(r => r.BatchId == batchId)
                    .GroupBy(r => r.TransactionId)
                    .Where(g => g.Count() > 1 && g.Key != null)
                    .SelectMany(g => g)
                    .ToListAsync();

                foreach (var c in conflicts)
                {
                    c.MatchStatus = "X";
                    c.TransactionId = null;
                    c.ErrorMessage = "Aynı Transaction bu batch içinde birden fazla satıra bağlanmaya çalıştı.";
                }

                await _db.SaveChangesAsync();
            }

            // batch sayıları güncelle
            var batch = await _db.ClearingBatches.FirstOrDefaultAsync(b => b.Id == batchId)
                        ?? throw new ApplicationException($"Batch {batchId} bulunamadı.");

            var m = await _db.ClearingRecords.CountAsync(r => r.BatchId == batchId && r.MatchStatus == "M");
            batch.SuccessCount = m;
            batch.FailCount = batch.TotalCount - m;

            await _db.SaveChangesAsync();
            return count;
        }

        // =========================================================
        // ==================   Helpers   ==========================
        // =========================================================

        // CSV oku → satır oluştur → DB'ye yaz
        private async Task<List<ClearingRecord>> ParseAndPersistAsync(IFormFile file, int batchId)
        {
            var list = new List<ClearingRecord>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            int lineNo = 0;

            // İlk satır header mı?
            string? first = await reader.ReadLineAsync();
            bool hasHeader = first != null &&
                             first.Split(';')[0]
                                  .Contains("CardLast4", StringComparison.OrdinalIgnoreCase);

            if (!hasHeader && first != null)
            {
                lineNo++;
                var rec0 = ParseLine(first, batchId, lineNo);
                _db.ClearingRecords.Add(rec0);
                list.Add(rec0);
            }

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNo++;
                var rec = ParseLine(line, batchId, lineNo);
                _db.ClearingRecords.Add(rec);
                list.Add(rec);
            }

            await _db.SaveChangesAsync(); // satırları (P/E) olarak yaz
            return list;
        }

        private static ClearingRecord ErrorRow(int batchId, int lineNo, string msg) => new()
        {
            BatchId = batchId,
            LineNumber = lineNo,
            MatchStatus = "E",
            ErrorMessage = msg,
            CreatedAt = DateTime.Now
        };

        // "78"→"0078", "123"→"0123", "4"→"0004", "0004"→"0004"; sadece rakamları tut
        private static string? NormalizeLast4(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return null;
            if (digits.Length > 4) return digits[^4..];
            return digits.PadLeft(4, '0');
        }

        private static bool TryParseAmount(string? raw, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            if (decimal.TryParse(raw.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return true;

            if (decimal.TryParse(raw.Trim(), NumberStyles.Number, new CultureInfo("tr-TR"), out value))
                return true;

            var fixedRaw = raw.Replace(',', '.');
            return decimal.TryParse(fixedRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private static bool IsValidCurrency(string? ccy)
        {
            if (string.IsNullOrWhiteSpace(ccy)) return false;
            var c = ccy.Trim().ToUpperInvariant();
            return c.Length == 3;
            // örn. whitelist istersen:
            // return new[] { "TRY", "USD", "EUR" }.Contains(c);
        }

        // Tek satırı parse et + zorunlu alan validasyonu
        private static ClearingRecord ParseLine(string line, int batchId, int lineNo)
        {
            var parts = line.Split(';');
            if (parts.Length < 5)
                return ErrorRow(batchId, lineNo, "Eksik kolon sayısı");

            var last4 = NormalizeLast4(parts[0]);
            if (last4 is null)
                return ErrorRow(batchId, lineNo, "CardLast4 geçersiz/boş");

            if (!TryParseAmount(parts[1], out var amount))
                return ErrorRow(batchId, lineNo, "Amount sayı değil (örn. 1234.56)");

            var currency = (parts[2] ?? "").Trim().ToUpperInvariant();
            if (!IsValidCurrency(currency))
                return ErrorRow(batchId, lineNo, "Currency 3 harfli ISO olmalı");

            DateTime? txDate = null;
            var d = parts[3]?.Trim();
            if (!string.IsNullOrEmpty(d) &&
                DateTime.TryParse(d, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                txDate = dt;

            var merchant = string.IsNullOrWhiteSpace(parts[4]) ? null : parts[4].Trim();

            // 6. kolon imza olabilir (opsiyonel)
            var sigFromFile = (parts.Length >= 6 ? parts[5]?.Trim() : null);

            return new ClearingRecord
            {
                BatchId = batchId,
                LineNumber = lineNo,
                CardLast4 = last4,
                Amount = amount,
                Currency = currency,
                TransactionDate = txDate,
                MerchantName = merchant,
                MatchStatus = "P",
                CreatedAt = DateTime.Now,
                SignatureHash = string.IsNullOrWhiteSpace(sigFromFile)
                    ? ComputeSignature(last4, amount, currency, txDate, merchant)
                    : sigFromFile
            };
        }


        // Eşleştirme: hatalıysa E, bulunamazsa N, bulunursa M
        private async Task TryMatchAsync(ClearingRecord record)
        {
            // 6. kolonda imza gelmişse onu kullan; yoksa yeniden hesapla
            var sig = string.IsNullOrWhiteSpace(record.SignatureHash)
                ? ComputeSignature(record.CardLast4, record.Amount, record.Currency, record.TransactionDate, record.MerchantName)
                : record.SignatureHash;

            // Sadece imzayla eşleştir (tarih/saniye/boşluk hatalarıyla uğraşma)
            var trx = await _db.Transactions.FirstOrDefaultAsync(t =>
                t.Status == "S" && t.SignatureHash == sig);

            if (trx == null)
            {
                record.MatchStatus = "N";
                record.ErrorMessage = "Eşleşen işlem bulunamadı.";
                return;
            }

            var alreadyMatched = await _db.ClearingRecords.AnyAsync(r =>
                r.BatchId == record.BatchId && r.TransactionId == trx.Id &&
                r.Id != record.Id && r.MatchStatus == "M");

            if (alreadyMatched)
            {
                record.MatchStatus = "X";
                record.TransactionId = null;
                record.ErrorMessage = "Aynı işlem başka satırda zaten eşleşmiş.";
            }
            else
            {
                record.TransactionId = trx.Id;
                record.MatchStatus = "M";
                record.ErrorMessage = null;
            }
        }

        private static string GetLast4(string? pan)
        {
            if (string.IsNullOrWhiteSpace(pan)) return "????";
            var clean = pan.Replace(" ", "");
            return clean.Length < 4 ? "????" : clean[^4..];
        }

        private static async Task<string> ComputeSha256Async(IFormFile file)
        {
            using var sha = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hash = await sha.ComputeHashAsync(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string ComputeSha256(byte[] bytes)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string ComputeSignature(string? last4, decimal amount, string currency, DateTime? txDate, string? merchant)
        {
            var l4 = NormalizeLast4(last4) ?? "";
            var ccy = (currency ?? "").Trim().ToUpperInvariant();
            var d = txDate?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";
            var m = (merchant ?? "").Trim();

            var raw = $"{l4}|{amount:0.####}|{ccy}|{d}|{m}";
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
        }

    }
}
