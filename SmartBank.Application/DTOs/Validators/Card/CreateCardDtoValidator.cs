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
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId geçerli olmalıdır.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Kart sahibi adı boş olamaz.")
                .MaximumLength(100);

            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .Matches(@"^\d{13,19}$")
                .WithMessage("Kart numarası 13–19 hane ve sadece rakamlardan oluşmalıdır.");

            RuleFor(x => x.ExpiryMonth)
                .NotEmpty()
                .Matches("^(0[1-9]|1[0-2])$")
                .WithMessage("Ay 01–12 arasında olmalıdır.");

            RuleFor(x => x.ExpiryYear)
                .NotEmpty()
                .Matches(@"^(?:\d{2}|\d{4})$")
                .WithMessage("'Expiry Year' 2 ya da 4 hane olmalıdır.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Para birimi 3 haneli olmalı (örn: TRY).");

            RuleFor(x => x.CardStatus)
                .NotEmpty().WithMessage("Kart durumu boş olamaz.")
                .MaximumLength(1);

            RuleFor(x => x.CardType)
            .NotEmpty().WithMessage("Card Type boş olamaz.")
            .Must(ct => new[] { "C", "D", "P" }.Contains(ct))
            .WithMessage("Card Type yalnızca 'C' (Credit), 'D' (Debit) veya 'P' (Prepaid) olabilir.");

            RuleFor(x => x.CardProvider)
                .NotEmpty().WithMessage("Card Provider boş olamaz.")
                .Must(cp => new[] { "V", "M", "T" }.Contains(cp))
                .WithMessage("Card Provider yalnızca 'V' (Visa), 'M' (Mastercard) veya 'T' (Troy) olabilir.");

            RuleFor(x => x.CardLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Kart limiti negatif olamaz.");

            RuleFor(x => x.DailyLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Günlük limit negatif olamaz.");

            RuleFor(x => x.TransactionLimit)
                .GreaterThanOrEqualTo(0).WithMessage("İşlem limiti negatif olamaz.");

            RuleFor(x => x.CardIssuerBank)
                .NotEmpty().WithMessage("Kartı basan banka boş olamaz.")
                .MaximumLength(100).WithMessage("Kartı basan banka en fazla 100 karakter olabilir.");

            // SHA-256 hex 64 karakter
            RuleFor(x => x.PinHash)
                .NotEmpty().WithMessage("PIN Hash boş olamaz.")
                .Length(64).WithMessage("PIN Hash 64 karakter olmalıdır.")
                .Matches("^[0-9a-fA-F]{64}$").WithMessage("PIN Hash SHA-256 (hex) formatında olmalıdır.");
        }
    }
}

