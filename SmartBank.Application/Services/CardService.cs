using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Card;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;
using System.Text.RegularExpressions;

namespace SmartBank.Application.Services
{
    public class CardService : ICardService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public CardService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Tüm kartlar
        public async Task<List<SelectCardDto>> GetAllAsync()
        {
            var cards = await _dbContext.Cards
                .AsNoTracking()
                .Include(c => c.Customer)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return cards.Select(c => new SelectCardDto
            {
                Id = c.Id,

                // Customer
                CustomerFullName = $"{(c.Customer?.FirstName ?? "").Trim()} {(c.Customer?.LastName ?? "").Trim()}".Trim(),

                // Maskeli PAN
                MaskedCardNumber = MaskPan(c.CardNumber),

                // ExpiryDate -> AA / YY
                ExpiryMonth = c.ExpiryDate.ToString("MM"),
                ExpiryYear = c.ExpiryDate.ToString("yy"),

                // Diğer alanlar
                Currency = c.Currency ?? "TRY",
                CardStatus = c.CardStatus ?? "A",

                // Entity’de yok → boş bırak
                CardStatusDescription = "",

                IsVirtual = c.IsVirtual,
                IsBlocked = c.IsBlocked,
                IsContactless = c.IsContactless,

                CardLimit = c.CardLimit,
                DailyLimit = c.DailyLimit,
                TransactionLimit = c.TransactionLimit,
                FailedPinAttempts = c.FailedPinAttempts,
                LastUsedAt = c.LastUsedAt,

                CardProvider = c.CardProvider ?? "V",
                CardIssuerBank = c.CardIssuerBank ?? "",
                CardStatusChangeReason = c.CardStatusChangeReason ?? "",

                ParentCardId = c.ParentCardId
            }).ToList();
        }




        // Id'ye göre kart
        public async Task<SelectCardDto?> GetByIdAsync(int id)
        {
            var card = await _dbContext.Cards
                .AsNoTracking()
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return card is null ? null : _mapper.Map<SelectCardDto>(card);
        }

        // Yeni kart ekle
        public async Task<SelectCardDto> CreateAsync(CreateCardDto dto)
        {
            // (1) Müşteri var mı?
            var customerExists = await _dbContext.Customers.AnyAsync(c => c.Id == dto.CustomerId && c.IsActive);
            if (!customerExists) throw new InvalidOperationException("Belirtilen müşteri bulunamadı veya pasif.");

            // (2) Kart tipi: C/D/P
            var allowedTypes = new[] { "C", "D", "P" };
            if (string.IsNullOrWhiteSpace(dto.CardType) || dto.CardType.Length != 1 || !allowedTypes.Contains(dto.CardType))
                throw new InvalidOperationException("Kart tipi yalnızca 'C' (Kredi), 'D' (Banka) veya 'P' (Ön ödemeli) olabilir.");

            // (3) Sağlayıcı: V/M/T
            var allowedProviders = new[] { "V", "M", "T" };
            if (string.IsNullOrWhiteSpace(dto.CardProvider) || dto.CardProvider.Length != 1 || !allowedProviders.Contains(dto.CardProvider))
                throw new InvalidOperationException("Kart sağlayıcısı yalnızca 'V' (Visa), 'M' (Mastercard) veya 'T' (Troy) olabilir.");

            // (4) Aynı müşteri + aynı sağlayıcı + aktif kart var mı?
            var providerExistsForCustomer = await _dbContext.Cards.AnyAsync(c =>
                c.CustomerId == dto.CustomerId && c.CardProvider == dto.CardProvider &&
                c.CardStatus == "A" && c.IsActive);
            if (providerExistsForCustomer)
                throw new InvalidOperationException("Bu müşteri için aynı sağlayıcıya ait aktif bir kart zaten mevcut.");

            // (5) PAN benzersiz mi?
            if (string.IsNullOrWhiteSpace(dto.CardNumber))
                throw new InvalidOperationException("Kart numarası boş olamaz.");
            var cardNumberExists = await _dbContext.Cards.AnyAsync(c => c.CardNumber == dto.CardNumber && c.IsActive);
            if (cardNumberExists)
                throw new InvalidOperationException("Bu kart numarası zaten kullanılıyor.");

            // (6) Para birimi
            if (string.IsNullOrWhiteSpace(dto.Currency) || dto.Currency.Length != 3)
                throw new InvalidOperationException("Para birimi 3 haneli olmalıdır (örn: TRY).");

            // (7) Limitler
            if (dto.CardLimit <= 0) throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");
            if (dto.DailyLimit <= 0) throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit <= 0) throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit > dto.DailyLimit) throw new InvalidOperationException("İşlem limiti günlük limiti aşamaz.");

            // (8) Statü: YALNIZCA 'A'
            if (string.IsNullOrWhiteSpace(dto.CardStatus) || dto.CardStatus.Trim() != "A")
                throw new InvalidOperationException("Yeni kart sadece 'A' (Aktif) durumda oluşturulabilir.");

            // (9) PIN hash (SHA-256 hex 64)
            if (string.IsNullOrWhiteSpace(dto.PinHash) || dto.PinHash.Length != 64)
                throw new InvalidOperationException("PIN Hash 64 karakter olmalıdır (SHA-256 hex).");

            // (10) Map + persist
            var entity = _mapper.Map<Card>(dto);
            entity.IsActive = true;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            _dbContext.Cards.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Geri Select için müşteriyle tekrar oku
            var created = await _dbContext.Cards
                .AsNoTracking()
                .Include(c => c.Customer)
                .FirstAsync(c => c.Id == entity.Id);

            return _mapper.Map<SelectCardDto>(created);
        }

        // Kart güncelleme (provider, issuer ve diğer create-only alanlar değiştirilemez)
        // Kart güncelleme (provider, issuer, holder name ve flag'ler değiştirilemez)
        public async Task UpdateAsync(int id, UpdateCardDto dto)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
                ?? throw new KeyNotFoundException("Güncellenecek kart bulunamadı veya pasif.");

            // --- Değiştirilemez alanlar ---

            // Provider (V/M/T) GÜNCELLENEMEZ
            if (!string.IsNullOrWhiteSpace(dto.CardProvider) && dto.CardProvider != card.CardProvider)
                throw new InvalidOperationException("Kart sağlayıcısı (provider) güncellenemez.");

            // Issuer (banka) GÜNCELLENEMEZ
            if (!string.IsNullOrWhiteSpace(dto.CardIssuerBank) && dto.CardIssuerBank != card.CardIssuerBank)
                throw new InvalidOperationException("Kartı basan banka (issuer) güncellenemez.");

            // Kart sahibi adı GÜNCELLENEMEZ (başka bir istemci gönderirse)
            var holderProp = dto.GetType().GetProperty("CardHolderName");
            if (holderProp != null)
            {
                var newHolder = holderProp.GetValue(dto) as string;
                if (!string.IsNullOrWhiteSpace(newHolder) && !string.Equals(newHolder, card.CardHolderName, StringComparison.Ordinal))
                    throw new InvalidOperationException("Kart sahibi adı güncellenemez.");
            }

            // Flag'ler GÜNCELLENEMEZ
            if (dto.IsVirtual.HasValue && dto.IsVirtual.Value != card.IsVirtual)
                throw new InvalidOperationException("Sanal (IsVirtual) değeri güncellenemez.");
            if (dto.IsContactless.HasValue && dto.IsContactless.Value != card.IsContactless)
                throw new InvalidOperationException("Temassız (IsContactless) değeri güncellenemez.");
            if (dto.IsBlocked.HasValue && dto.IsBlocked.Value != card.IsBlocked)
                throw new InvalidOperationException("Bloke (IsBlocked) değeri güncellenemez.");

            // (CardType / Currency / Expiry / PAN / Customer Update DTO'da yok; gelse de değiştirmiyoruz)

            // --- İzin verilen güncellemeler ---

            // Statü (A/B/C)
            if (!string.IsNullOrWhiteSpace(dto.CardStatus))
            {
                var allowedStatuses = new[] { "A", "B", "C" };
                if (!allowedStatuses.Contains(dto.CardStatus))
                    throw new InvalidOperationException("Geçersiz kart durumu. 'A', 'B' veya 'C' olmalıdır.");
                card.CardStatus = dto.CardStatus;
            }

            // Limitler
            if (dto.CardLimit.HasValue && dto.CardLimit <= 0) throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");
            if (dto.DailyLimit.HasValue && dto.DailyLimit <= 0) throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit.HasValue && dto.TransactionLimit <= 0) throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit.HasValue && dto.DailyLimit.HasValue && dto.TransactionLimit > dto.DailyLimit)
                throw new InvalidOperationException("İşlem limiti günlük limiti aşamaz.");

            if (dto.CardLimit.HasValue) card.CardLimit = dto.CardLimit.Value;
            if (dto.DailyLimit.HasValue) card.DailyLimit = dto.DailyLimit.Value;
            if (dto.TransactionLimit.HasValue) card.TransactionLimit = dto.TransactionLimit.Value;

            // Diğer opsiyoneller (gönderiliyorsa izin verilecekler)
            if (dto.FailedPinAttempts.HasValue && dto.FailedPinAttempts < 0)
                throw new InvalidOperationException("Hatalı PIN deneme sayısı negatif olamaz.");
            if (dto.FailedPinAttempts.HasValue) card.FailedPinAttempts = dto.FailedPinAttempts.Value;

            if (!string.IsNullOrWhiteSpace(dto.CardStatusChangeReason))
            {
                if (dto.CardStatusChangeReason.Length > 250)
                    throw new InvalidOperationException("Durum değişim nedeni en fazla 250 karakter olabilir.");
                card.CardStatusChangeReason = dto.CardStatusChangeReason;
            }

            // Artık flag set ETMİYORUZ (yasaklandı)
            // if (dto.IsBlocked.HasValue)     card.IsBlocked     = dto.IsBlocked.Value;
            // if (dto.IsVirtual.HasValue)     card.IsVirtual     = dto.IsVirtual.Value;
            // if (dto.IsContactless.HasValue) card.IsContactless = dto.IsContactless.Value;

            if (dto.LastUsedAt.HasValue) card.LastUsedAt = dto.LastUsedAt.Value;
            if (dto.ParentCardId.HasValue) card.ParentCardId = dto.ParentCardId.Value; // istersen bunu da kilitleyebilirsin

            card.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        // Kart silme (soft delete)
        public async Task DeleteAsync(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
                ?? throw new KeyNotFoundException("Silinecek kart bulunamadı veya zaten pasif.");

            if (card.CardStatus == "B")
                throw new InvalidOperationException("Bloke edilmiş kartlar silinemez.");

            card.IsActive = false;
            card.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<CardPanDto?> GetPanAsync(int id)
        {
            var pan = await _dbContext.Cards
                .AsNoTracking()
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => c.CardNumber)
                .FirstOrDefaultAsync();

            return pan is null ? null : new CardPanDto { CardNumber = pan };
        }


        private static string MaskPan(string? pan, char maskChar = '*')
        {
            if (string.IsNullOrWhiteSpace(pan))
                return string.Empty;

            // Sadece rakamlar
            var digits = new string(pan.Where(char.IsDigit).ToArray());
            var len = digits.Length;

            // 6+4 kuralı uygulanamayacak kadar kısaysa maskesiz geri dön (sadece 4'lü gruplandır)
            if (len <= 10)
                return Group4(digits);

            var first6 = digits.Substring(0, Math.Min(6, len));
            var last4 = digits.Substring(len - 4, 4);
            var midLen = Math.Max(0, len - first6.Length - last4.Length);

            var masked = first6 + new string(maskChar, midLen) + last4;
            return Group4(masked);

            static string Group4(string s)
                => string.Join(" ",
                    Enumerable.Range(0, (s.Length + 3) / 4)
                              .Select(i => s.Substring(i * 4, Math.Min(4, s.Length - i * 4))));
        }
    }
}
