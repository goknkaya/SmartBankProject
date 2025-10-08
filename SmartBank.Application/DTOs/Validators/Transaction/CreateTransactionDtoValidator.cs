using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Transaction;

namespace SmartBank.Application.DTOs.Validators.Transaction
{
    public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionDtoValidator()
        {
            RuleFor(x => x.CardId).GreaterThan(0).WithMessage("Kart Id geçerli olmalıdır.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("İşlem tutarı sıfırdan büyük olmalıdır.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("Para birimi boş olamaz.").Length(3).WithMessage("Para birimi üç karakter olmalıdır.");
            RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("İşlem tarihi girilmelidir.");
            RuleFor(x => x.Description).MaximumLength(100).WithMessage("Açıklama en fazla 200 karakter olmalıdır.");
        }
    }
}
