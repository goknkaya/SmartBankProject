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
        public async Task<List<SelectCardDto>> GetAllCardsAsync()
        {
            var cards = await _dbContext.Cards
                .AsNoTracking()
                .Include(c => c.Customer)     // DTO'da CustomerFullName/Number dolsun
                .Where(c => c.IsActive)
                .ToListAsync();

            return _mapper.Map<List<SelectCardDto>>(cards);
        }

        // Id'ye göre kart
        public async Task<SelectCardDto?> GetCardByIdAsync(int id)
        {
            var card = await _dbContext.Cards
                .AsNoTracking()
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (card == null)
                throw new KeyNotFoundException("Kart bulunamadı.");

            return _mapper.Map<SelectCardDto>(card);
        }

        // Yeni kart ekle
        public async Task<bool> CreateCardAsync(CreateCardDto dto)
        {
            // 1) Müşteri kontrolü
            var customerExists = await _dbContext.Customers
                .AnyAsync(c => c.Id == dto.CustomerId && c.IsActive);
            if (!customerExists)
                throw new InvalidOperationException("Belirtilen müşteri bulunamadı veya pasif.");

            // 2) Kart tipi (C/D/P)
            var allowedTypes = new[] { "C", "D", "P" }; // Credit / Debit / Prepaid
            if (string.IsNullOrWhiteSpace(dto.CardType) || dto.CardType.Length != 1 || !allowedTypes.Contains(dto.CardType))
                throw new InvalidOperationException("Kart tipi yalnızca 'C' (Kredi), 'D' (Banka) veya 'P' (Ön ödemeli) olabilir.");

            // 3) Sağlayıcı (V/M/T)
            var allowedProviders = new[] { "V", "M", "T" }; // Visa / Mastercard / Troy
            if (string.IsNullOrWhiteSpace(dto.CardProvider) || dto.CardProvider.Length != 1 || !allowedProviders.Contains(dto.CardProvider))
                throw new InvalidOperationException("Kart sağlayıcısı yalnızca 'V' (Visa), 'M' (Mastercard) veya 'T' (Troy) olabilir.");

            // 4) Aynı müşteri + aynı sağlayıcı kuralı (aktif kart varsa engelle)
            var providerExistsForCustomer = await _dbContext.Cards
                .AnyAsync(c => c.CustomerId == dto.CustomerId &&
                               c.CardProvider == dto.CardProvider &&
                               c.CardStatus == "A" &&
                               c.IsActive);
            if (providerExistsForCustomer)
                throw new InvalidOperationException("Bu müşteri için aynı sağlayıcıya ait aktif bir kart zaten mevcut.");

            // 5) Kart numarası benzersiz mi?
            if (string.IsNullOrWhiteSpace(dto.CardNumber))
                throw new InvalidOperationException("Kart numarası boş olamaz.");
            var cardNumberExists = await _dbContext.Cards
                .AnyAsync(c => c.CardNumber == dto.CardNumber && c.IsActive);
            if (cardNumberExists)
                throw new InvalidOperationException("Bu kart numarası zaten kullanılıyor.");

            // 6) Para birimi
            if (string.IsNullOrWhiteSpace(dto.Currency) || dto.Currency.Length != 3)
                throw new InvalidOperationException("Para birimi 3 haneli olmalıdır (örn: TRY).");

            // 7) Limitler
            if (dto.CardLimit <= 0)
                throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");
            if (dto.DailyLimit <= 0)
                throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit <= 0)
                throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit > dto.DailyLimit)
                throw new InvalidOperationException("İşlem limiti günlük limiti aşamaz.");

            // 8) Statü (A/B/C)
            var allowedStatuses = new[] { "A", "B", "C" }; // Active / Blocked / Cancelled
            if (string.IsNullOrWhiteSpace(dto.CardStatus) || !allowedStatuses.Contains(dto.CardStatus))
                throw new InvalidOperationException("Geçersiz kart durumu. 'A', 'B' veya 'C' olmalıdır.");

            // 9) PIN Hash (SHA-256 hex / 64)
            if (string.IsNullOrWhiteSpace(dto.PinHash) || dto.PinHash.Length != 64)
                throw new InvalidOperationException("PIN Hash 64 karakter olmalıdır (SHA-256 hex).");

            // 10) Map + Ekle
            var card = _mapper.Map<Card>(dto);
            card.IsActive = true;
            card.CreatedAt = DateTime.UtcNow;
            card.UpdatedAt = DateTime.UtcNow;

            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        // Kart güncelleme
        public async Task<bool> UpdateCardAsync(UpdateCardDto dto)
        {
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(c => c.Id == dto.Id && c.IsActive);
            if (card == null)
                throw new InvalidOperationException("Güncellenecek kart bulunamadı veya pasif.");

            // Sağlayıcı (V/M/T) kontrolü
            if (!string.IsNullOrWhiteSpace(dto.CardProvider))
            {
                var allowedProviders = new[] { "V", "M", "T" };
                if (dto.CardProvider.Length != 1 || !allowedProviders.Contains(dto.CardProvider))
                    throw new InvalidOperationException("Kart sağlayıcısı yalnızca 'V', 'M' veya 'T' olabilir.");

                // Aynı müşteri için aynı sağlayıcıda başka aktif kart var mı kontrol et
                var providerExistsForCustomer = await _dbContext.Cards
                    .AnyAsync(c => c.CustomerId == card.CustomerId
                                && c.CardProvider == dto.CardProvider
                                && c.CardStatus == "A"
                                && c.Id != dto.Id); // kendi kartı hariç

                if (providerExistsForCustomer)
                    throw new InvalidOperationException("Bu müşteri için aynı sağlayıcıya ait aktif bir kart zaten mevcut.");

                card.CardProvider = dto.CardProvider;
            }

            // Statü (A/B/C)
            if (!string.IsNullOrWhiteSpace(dto.CardStatus))
            {
                var allowedStatuses = new[] { "A", "B", "C" };
                if (!allowedStatuses.Contains(dto.CardStatus))
                    throw new InvalidOperationException("Geçersiz kart durumu. 'A', 'B' veya 'C' olmalıdır.");
                card.CardStatus = dto.CardStatus;
            }

            // Limitler
            if (dto.CardLimit.HasValue && dto.CardLimit <= 0)
                throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");
            if (dto.DailyLimit.HasValue && dto.DailyLimit <= 0)
                throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit.HasValue && dto.TransactionLimit <= 0)
                throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit.HasValue && dto.DailyLimit.HasValue &&
                dto.TransactionLimit > dto.DailyLimit)
                throw new InvalidOperationException("İşlem limiti günlük limiti aşamaz.");

            if (dto.CardLimit.HasValue) card.CardLimit = dto.CardLimit.Value;
            if (dto.DailyLimit.HasValue) card.DailyLimit = dto.DailyLimit.Value;
            if (dto.TransactionLimit.HasValue) card.TransactionLimit = dto.TransactionLimit.Value;

            // Diğer opsiyoneller
            if (dto.FailedPinAttempts.HasValue && dto.FailedPinAttempts < 0)
                throw new InvalidOperationException("Hatalı PIN deneme sayısı negatif olamaz.");
            if (dto.FailedPinAttempts.HasValue) card.FailedPinAttempts = dto.FailedPinAttempts.Value;

            if (!string.IsNullOrWhiteSpace(dto.CardIssuerBank))
            {
                if (dto.CardIssuerBank.Length > 100)
                    throw new InvalidOperationException("Kartı basan banka adı çok uzun.");
                card.CardIssuerBank = dto.CardIssuerBank;
            }

            if (!string.IsNullOrWhiteSpace(dto.CardStatusChangeReason))
            {
                if (dto.CardStatusChangeReason.Length > 250)
                    throw new InvalidOperationException("Durum değişim nedeni en fazla 250 karakter olabilir.");
                card.CardStatusChangeReason = dto.CardStatusChangeReason;
            }

            if (dto.IsBlocked.HasValue) card.IsBlocked = dto.IsBlocked.Value;
            if (dto.IsVirtual.HasValue) card.IsVirtual = dto.IsVirtual.Value;
            if (dto.LastUsedAt.HasValue) card.LastUsedAt = dto.LastUsedAt.Value;
            if (dto.ParentCardId.HasValue) card.ParentCardId = dto.ParentCardId.Value;

            card.UpdatedAt = DateTime.UtcNow;

            _dbContext.Cards.Update(card);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        // Kart silme (soft delete)
        public async Task<bool> DeleteCardAsync(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
            if (card == null)
                throw new InvalidOperationException("Silinecek kart bulunamadı veya zaten pasif.");

            if (card.CardStatus == "B")
                throw new InvalidOperationException("Bloke edilmiş kartlar silinemez.");

            card.IsActive = false;
            card.UpdatedAt = DateTime.UtcNow;

            _dbContext.Cards.Update(card);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
