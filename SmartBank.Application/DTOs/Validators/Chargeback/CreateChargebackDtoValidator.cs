using FluentValidation;
using SmartBank.Application.DTOs.Chargeback;

public class CreateChargebackDtoValidator : AbstractValidator<CreateChargebackDto>
{
    public CreateChargebackDtoValidator()
    {
        RuleFor(x => x.TransactionId).GreaterThan(0);
        RuleFor(x => x.ReasonCode).NotEmpty().MaximumLength(10);
    }
}
