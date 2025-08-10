using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Reversal;

namespace SmartBank.Application.DTOs.Validators.Reversal
{
    public class CreateReversalDtoValidator:AbstractValidator<CreateReversalDto>
    {
        private static readonly string[] AllowedSources = new[] { "API", "MANUEL", "BATCH" };

        public CreateReversalDtoValidator()
        {
            RuleFor(x => x.TransactionId)
                .GreaterThan(0).WithMessage("TransactionId geçerli olmalıdır.");

            RuleFor(x => x.ReversedAmount)
                .GreaterThan(0).WithMessage("Reversal tutarı sıfırdan büyük olmalıdır.")
                .Must(a => HasMaxTwoDecimals(a))
                .WithMessage("Reversal tutarı en fazla 2 ondalık haneye sahip olmalıdır.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reversal nedeni zorunludur.")
                .MaximumLength(200).WithMessage("Reversal nedeni en fazla 200 karakter olmalıdır.");

            RuleFor(x => x.PerformedBy)
                .NotEmpty().WithMessage("İşlemi yapan kullanıcı zorunludur.")
                .MaximumLength(100).WithMessage("İşlemi yapan kullanıcı en fazla 100 karakter olmalıdır.");

            RuleFor(x=>x.ReversalSource)
                .NotEmpty().WithMessage("Reversal kaynağı zorunludur.")
                .Must(src => AllowedSources.Contains(src?.Trim().ToUpperInvariant() ?? ""))
                .WithMessage($"Reversal kaynağı yalnızca şu değerlerden biri olabilir: {string.Join(", ", AllowedSources)}");
        }

        private bool HasMaxTwoDecimals(decimal value)
        {
            // 2 ondalık sınırı: 100 ile çarptığında tam sayıysa OK
            return decimal.Round(value, 2) == value;
        }
    }
}
