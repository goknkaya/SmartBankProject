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
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Kart güncelleme işlemi için geçerli bir ID girilmelidir.");

            When(x => x.CardStatus != null, () =>
            {
                RuleFor(x => x.CardStatus)
                    .NotEmpty().WithMessage("Kart durumu boş olamaz.")
                    .MaximumLength(1).WithMessage("Kart durumu en fazla 1 karakter olmalıdır.");
            });

            When(x => x.CardStatusDescription != null, () =>
            {
                RuleFor(x => x.CardStatusDescription)
                    .MaximumLength(250).WithMessage("Açıklama en fazla 250 karakter olabilir.");
            });

            When(x => x.IsBlocked != null, () =>
            {
                RuleFor(x => x.IsBlocked)
                    .Must(x => x == true || x == false)
                    .WithMessage("Bloke bilgisi sadece true ya da false olmalıdır.");
            });

            When(x => x.IsContactless != null, () =>
            {
                RuleFor(x => x.IsContactless)
                    .Must(x => x == true || x == false)
                    .WithMessage("Temassız bilgisi sadece true ya da false olmalıdır.");
            });

            When(x => x.IsVirtual != null, () =>
            {
                RuleFor(x => x.IsVirtual)
                    .Must(x => x == true || x == false)
                    .WithMessage("Sanal kart bilgisi sadece true ya da false olmalıdır.");
            });

            When(x => x.CardLimit != null, () =>
            {
                RuleFor(x => x.CardLimit)
                    .GreaterThanOrEqualTo(0).WithMessage("Kart limiti negatif olamaz.");
            });

            When(x => x.DailyLimit != null, () =>
            {
                RuleFor(x => x.DailyLimit)
                    .GreaterThanOrEqualTo(0).WithMessage("Günlük limit negatif olamaz.");
            });

            When(x => x.TransactionLimit != null, () =>
            {
                RuleFor(x => x.TransactionLimit)
                    .GreaterThanOrEqualTo(0).WithMessage("İşlem limiti negatif olamaz.");
            });

            When(x => x.FailedPinAttempts != null, () =>
            {
                RuleFor(x => x.FailedPinAttempts)
                    .GreaterThanOrEqualTo(0).WithMessage("Hatalı pin denemesi negatif olamaz.");
            });

            When(x => x.CardStatusChangeReason != null, () =>
            {
                RuleFor(x => x.CardStatusChangeReason)
                    .MaximumLength(250).WithMessage("Durum değişim açıklaması en fazla 250 karakter olabilir.");
            });

            When(x => x.CardProvider != null, () =>
            {
                RuleFor(x => x.CardProvider)
                    .MaximumLength(100).WithMessage("Kart sağlayıcı en fazla 100 karakter olabilir.");
            });

            When(x => x.CardIssuerBank != null, () =>
            {
                RuleFor(x => x.CardIssuerBank)
                    .MaximumLength(100).WithMessage("Kartı basan banka en fazla 100 karakter olabilir.");
            });

            When(x => x.ParentCardId != null, () =>
            {
                RuleFor(x => x.ParentCardId)
                    .GreaterThan(0).WithMessage("Ana kart ID'si pozitif bir sayı olmalıdır.");
            });
        }
    }
}
