using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Card;

namespace SmartBank.Application.DTOs.Validators.Card
{
    public class CreateCardDtoValidator : AbstractValidator<CreateCardDto>
    {
        public CreateCardDtoValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("CustomerId geçerli olmalıdır.");

            RuleFor(x => x.CardHolderName).NotEmpty().WithMessage("Kart sahibi adı boş olamaz.").MaximumLength(100);

            RuleFor(x => x.CardNumber).Length(13, 19).WithMessage("Kart numarası 13 ile 19 hane arasında olmalıdır.");

            RuleFor(x => x.ExpiryMonth).NotEmpty().Matches(@"^(0[1-9]|1[0-2])$").WithMessage("Ay 01 ile 12 arasında olmalı.");

            RuleFor(x => x.ExpiryYear).NotEmpty().Length(2).Matches("Yıl 2 haneli olmalı.");

            RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Para birimi 3 haneli olmalı (örn: TRY).");

            RuleFor(x => x.CardStatus).NotEmpty().WithMessage("Kart statüsü boş olamaz.");

            RuleFor(x => x.IsVirtual).NotEmpty().WithMessage("Is Virtual boş olamaz.");

            RuleFor(x => x.IsContactless).NotEmpty().WithMessage("Is Contactless boş olamaz.");

            RuleFor(x => x.IsBlocked).NotEmpty().WithMessage("Is Blocked boş olamaz.");

            RuleFor(x => x.CardLimit).GreaterThanOrEqualTo(0).WithMessage("Kart limiti negatif olamaz.");

            RuleFor(x => x.DailyLimit).GreaterThanOrEqualTo(0).WithMessage("Günlük limit negatif olamaz.");

            RuleFor(x => x.TransactionLimit).GreaterThanOrEqualTo(0).WithMessage("İşlem limiti negatif olamaz.");

            RuleFor(x => x.PinHash).NotEmpty().WithMessage("PIN Hash değeri boş olamaz.").MinimumLength(64).WithMessage("PIN Hash SHA256 ile oluşturulmallı ve 64 karakter olmalıdır.").MaximumLength(64).WithMessage("PIN Hash değeri 64 karakterden uzun olamaz.");

        }
    }
}
