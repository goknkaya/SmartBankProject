using FluentValidation;
using SmartBank.Application.DTOs.Chargeback;

namespace SmartBank.Application.Validators.Chargeback;

public class CreateChargebackDtoValidator : AbstractValidator<CreateChargebackDto>
{
    public CreateChargebackDtoValidator()
    {
        RuleFor(x => x.TransactionId)
            .GreaterThan(0).WithMessage("transactionId > 0 olmalı.");

        RuleFor(x => x.DisputedAmount)
            .GreaterThan(0).WithMessage("disputedAmount pozitif olmalı.");

        RuleFor(x => x.ReasonCode)
            .NotEmpty().WithMessage("reasonCode zorunludur.")
            .MaximumLength(20).WithMessage("reasonCode en fazla 20 karakter olabilir.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("note en fazla 500 karakter olabilir.");

        RuleFor(x => x.ReplyBy)
            .Must(d => d == null || d > DateTime.Now)
            .WithMessage("replyBy gelecekte bir tarih olmalı.");
    }
}
