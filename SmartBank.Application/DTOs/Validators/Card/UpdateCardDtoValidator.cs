using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Card;

namespace SmartBank.Application.DTOs.Validators.Card
{
    public class UpdateCardDtoValidator : AbstractValidator<UpdateCardDto>
    {
        public UpdateCardDtoValidator()
        {
            // Id
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Kart güncelleme için geçerli bir Id girilmelidir.");

            // CardStatus
            RuleFor(x => x.CardStatus)
                .NotEmpty().WithMessage("Kart durumu boş olamaz.")
                .MaximumLength(1).WithMessage("Kart durumu en fazla 1 karakter olmalıdır.")
                .Must(cs => cs is null || new[] { "A", "B", "C" }.Contains(cs))
                .WithMessage("Kart durumu yalnızca 'A' (Aktif), 'B' (Bloke) veya 'C' (İptal) olabilir.")
                .When(x => x.CardStatus != null);

            // CardStatusDescription
            RuleFor(x => x.CardStatusDescription)
                .MaximumLength(250)
                .WithMessage("Kart durumu açıklaması en fazla 250 karakter olabilir.")
                .When(x => x.CardStatusDescription != null);

            // CardLimit
            RuleFor(x => x.CardLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Kart limiti negatif olamaz.")
                .When(x => x.CardLimit.HasValue);

            // DailyLimit
            RuleFor(x => x.DailyLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Günlük limit negatif olamaz.")
                .When(x => x.DailyLimit.HasValue);

            // TransactionLimit
            RuleFor(x => x.TransactionLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("İşlem limiti negatif olamaz.")
                .When(x => x.TransactionLimit.HasValue);

            // FailedPinAttempts
            RuleFor(x => x.FailedPinAttempts)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Başarısız PIN deneme sayısı negatif olamaz.")
                .When(x => x.FailedPinAttempts.HasValue);

            // CardStatusChangeReason
            RuleFor(x => x.CardStatusChangeReason)
                .MaximumLength(250)
                .WithMessage("Kart durumu değişim sebebi en fazla 250 karakter olabilir.")
                .When(x => x.CardStatusChangeReason != null);

            // CardProvider (Uzunluk + whitelist)
            RuleFor(x => x.CardProvider)
                .MaximumLength(50)
                .WithMessage("Kart sağlayıcı adı en fazla 50 karakter olabilir.")
                .When(x => x.CardProvider != null);

            RuleFor(x => x.CardProvider)
                .Must(cp => cp is null || new[] { "V", "M", "T" }.Contains(cp))
                .WithMessage("Card Provider yalnızca 'V' (Visa), 'M' (Mastercard) veya 'T' (Troy) olabilir.")
                .When(x => x.CardProvider != null);

            // CardIssuerBank
            RuleFor(x => x.CardIssuerBank)
                .MaximumLength(100).WithMessage("Kartı basan banka en fazla 100 karakter olabilir.")
                .When(x => !string.IsNullOrWhiteSpace(x.CardIssuerBank));

            // ParentCardId
            RuleFor(x => x.ParentCardId)
                .GreaterThan(0)
                .WithMessage("Parent Card Id değeri 0'dan büyük olmalıdır.")
                .When(x => x.ParentCardId.HasValue);
        }
    }
}
