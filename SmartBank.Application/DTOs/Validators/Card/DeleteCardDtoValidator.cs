using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Card;

namespace SmartBank.Application.DTOs.Validators.Card
{
    public class DeleteCardDtoValidator : AbstractValidator<DeleteCardDto>
    {
        public DeleteCardDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Silinecek kartın ID'si pozitif olmalıdır.");

            RuleFor(x => x.DeleteReason)
                .MaximumLength(250).WithMessage("Silme nedeni en fazla 250 karakter olabilir.");
        }
    }
}
