using FluentValidation;
using SmartBank.Application.DTOs.Clearing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Validators.Clearing
{
    public class SelectClearingBatchDtoValidator : AbstractValidator<SelectClearingBatchDto>
    {
        public SelectClearingBatchDtoValidator()
        {
            RuleFor(x => x.Direction)
                .NotEmpty().WithMessage("Direction boş olamaz.")
                .Must(x => x == "IN" || x == "OUT").WithMessage("Direction IN veya OUT olmalı.");

            RuleFor(x => x.Status)
                .Must(x => x == "N" || x == "P" || x == "E")
                .WithMessage("Status sadece N, P veya E olabilir.");

            RuleFor(x => x.TotalCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SettlementDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("SettlementDate ileri tarih olamaz.");
        }
    }

}
