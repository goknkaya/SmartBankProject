using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Card;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

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

        // Tum kartlar gelir
        public async Task<List<SelectCardDto>> GetAllCardsAsync()
        {
            var cards = await _dbContext.Cards
                .Where(c => c.IsActive)
                .ToListAsync();

            return _mapper.Map<List<SelectCardDto>>(cards);
        }

        //ID ' ye gore kartlar gelir
        public async Task<SelectCardDto?> GetCardByIdAsync(int id)
        {
            var card = await _dbContext.Cards
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (card == null)
                throw new KeyNotFoundException("Kart bulunamadı.");

            return _mapper.Map<SelectCardDto>(card);
        }

        // Yeni kart ekle
        public async Task<bool> CreateCardAsync(CreateCardDto dto)
        {
            // Müşteri gerçekten var mı?
            var isCustomerExists = await _dbContext.Customers.AnyAsync(c => c.Id == dto.CustomerId && c.IsActive);
            if (!isCustomerExists)
                throw new InvalidOperationException("Belirtilen müşteri sistemde bulunamadı.");

            // CardType boş mu?
            if (string.IsNullOrWhiteSpace(dto.CardType))
                throw new InvalidOperationException("Kart tipi boş olamaz.");

            // CardType uzunluğu 1 karakter mi?
            if (dto.CardType.Length > 1)
                throw new InvalidOperationException("Kart tipi en fazla 1 karakter olmalıdır.");

            // Kart numarası kontrolü
            if (string.IsNullOrWhiteSpace(dto.CardNumber))
                throw new InvalidOperationException("Kart numarası boş olamaz.");

            var isCardNumberExists = await _dbContext.Cards
                .AnyAsync(c => c.CardNumber == dto.CardNumber && c.IsActive);

            if (isCardNumberExists)
                throw new InvalidOperationException("Bu kart numarası zaten kullanılıyor.");

            // CardProvider kontrolü
            if (string.IsNullOrWhiteSpace(dto.CardProvider))
                throw new InvalidOperationException("Kart sağlayıcısı (CardProvider) boş olamaz.");

            if (dto.CardProvider.Length > 50)
                throw new InvalidOperationException("Kart sağlayıcısı en fazla 50 karakter olabilir.");

            // Müşteri + Sağlayıcı kontrolü (bir müşteri aynı sağlayıcıdan 2 kart alamaz)
            var isProviderExistsForCustomer = await _dbContext.Cards
                .AnyAsync(c => c.CustomerId == dto.CustomerId && c.CardProvider == dto.CardProvider && c.IsActive);

            if (isProviderExistsForCustomer)
                throw new InvalidOperationException("Bu müşteri için belirtilen sağlayıcıya ait aktif bir kart zaten mevcut.");

            // Pin kontrolü
            if (string.IsNullOrWhiteSpace(dto.PinHash))
                throw new InvalidOperationException("Pin bilgisi boş olamaz.");

            // Kart durumu kontrolü
            var allowedStatuses = new[] { "A", "B", "C" }; // Active, Blocked, Cancelled gibi
            if (!allowedStatuses.Contains(dto.CardStatus))
                throw new InvalidOperationException("Geçersiz kart durumu. (A, B veya C olmalı)");

            // Limit kontrolleri
            if (dto.CardLimit <= 0)
                throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");

            if (dto.TransactionLimit <= 0)
                throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");

            if (dto.DailyLimit <= 0)
                throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");

            if (dto.TransactionLimit > dto.DailyLimit)
                throw new InvalidOperationException("İşlem limiti, günlük limiti aşamaz.");

            if (dto.FailedPinAttempts < 0)
                throw new InvalidOperationException("Hatalı PIN giriş sayısı negatif olamaz.");

            // DTO validasyonları geçti, mapleme yap
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
            // Kart gerçekten var mı?
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == dto.Id && c.IsActive);
            if (card == null)
                throw new InvalidOperationException("Güncellenmek istenen kart bulunamadı.");

            // Kart durumu kontrolü
            if (!string.IsNullOrWhiteSpace(dto.CardStatus))
            {
                var allowedStatuses = new[] { "A", "B", "C" }; // Active, Blocked, Cancelled gibi
                if (!allowedStatuses.Contains(dto.CardStatus))
                    throw new InvalidOperationException("Geçersiz kart durumu. (A, B veya C olmalı)");
                card.CardStatus = dto.CardStatus;
            }

            // Limit kontrolleri
            if (dto.CardLimit.HasValue && dto.CardLimit <= 0)
                throw new InvalidOperationException("Kart limiti sıfırdan büyük olmalıdır.");
            if (dto.TransactionLimit.HasValue && dto.TransactionLimit <= 0)
                throw new InvalidOperationException("İşlem limiti sıfırdan büyük olmalıdır.");
            if (dto.DailyLimit.HasValue && dto.DailyLimit <= 0)
                throw new InvalidOperationException("Günlük limit sıfırdan büyük olmalıdır.");

            if (dto.TransactionLimit.HasValue && dto.DailyLimit.HasValue &&
                dto.TransactionLimit > dto.DailyLimit)
                throw new InvalidOperationException("İşlem limiti, günlük limiti aşamaz.");

            if (dto.CardLimit.HasValue)
                card.CardLimit = dto.CardLimit.Value;
            if (dto.TransactionLimit.HasValue)
                card.TransactionLimit = dto.TransactionLimit.Value;
            if (dto.DailyLimit.HasValue)
                card.DailyLimit = dto.DailyLimit.Value;

            // Failed PIN attempts
            if (dto.FailedPinAttempts.HasValue && dto.FailedPinAttempts < 0)
                throw new InvalidOperationException("Hatalı PIN deneme sayısı negatif olamaz.");
            if (dto.FailedPinAttempts.HasValue)
                card.FailedPinAttempts = dto.FailedPinAttempts.Value;

            // LastUsedAt (opsiyonel)
            if (dto.LastUsedAt.HasValue)
                card.LastUsedAt = dto.LastUsedAt.Value;

            // CardProvider kontrolü
            if (!string.IsNullOrWhiteSpace(dto.CardProvider))
            {
                if (dto.CardProvider.Length > 50)
                    throw new InvalidOperationException("Kart sağlayıcısı en fazla 50 karakter olabilir.");
                card.CardProvider = dto.CardProvider;
            }

            // CardIssuerBank
            if (!string.IsNullOrWhiteSpace(dto.CardIssuerBank))
            {
                if (dto.CardIssuerBank.Length > 50)
                    throw new InvalidOperationException("Kartı basan banka ismi çok uzun.");
                card.CardIssuerBank = dto.CardIssuerBank;
            }

            // CardStatusChangeReason
            if (!string.IsNullOrWhiteSpace(dto.CardStatusChangeReason))
            {
                if (dto.CardStatusChangeReason.Length > 100)
                    throw new InvalidOperationException("Durum değiştirme nedeni çok uzun.");
                card.CardStatusChangeReason = dto.CardStatusChangeReason;
            }

            // ParentCardId güncellemesi
            if (dto.ParentCardId.HasValue)
                card.ParentCardId = dto.ParentCardId;

            // Güncelleme tarihi
            card.UpdatedAt = DateTime.UtcNow;

            _dbContext.Cards.Update(card);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        // Kart silme
        public async Task<bool> DeleteCardAsync(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (card == null)
                throw new InvalidOperationException("Silinmek istenen kart bulunamadı veya zaten pasif.");

            if (card.CardStatus == "B")
                throw new InvalidOperationException("Bloke edilmiş kartlar silinemez.");

            card.IsActive = false;
            card.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
