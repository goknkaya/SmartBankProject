using FluentValidation;
using SmartBank.Application.DTOs.Clearing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Validators.Clearing
{
    public class SelectClearingRecordDtoValidator : AbstractValidator<SelectClearingRecordDto>
    {
        public SelectClearingRecordDtoValidator()
        {
            RuleFor(x => x.BatchId)
                .GreaterThan(0).WithMessage("BatchId geçerli olmalı.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount sıfırdan büyük olmalı.");

            RuleFor(x => x.Currency)
                .Length(3).WithMessage("Currency ISO 4217 standardına uygun 3 harfli olmalı.");

            RuleFor(x => x.MatchStatus)
                .Must(x => new[] { "P", "M", "N", "X", "E" }.Contains(x))
                .WithMessage("Geçersiz MatchStatus.");

            RuleFor(x => x.CardLast4)
                .Length(4).When(x => !string.IsNullOrEmpty(x.CardLast4))
                .WithMessage("CardLast4 varsa 4 haneli olmalı.");
        }
    }

}
