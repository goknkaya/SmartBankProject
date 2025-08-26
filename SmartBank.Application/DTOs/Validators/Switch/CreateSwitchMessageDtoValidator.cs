using FluentValidation;
using SmartBank.Application.DTOs.Switching;

public class CreateSwitchMessageDtoValidator : AbstractValidator<CreateSwitchMessageDto>
{
    public CreateSwitchMessageDtoValidator()
    {
        RuleFor(x => x.PAN)
            .NotEmpty()
            .Matches(@"^\d{12,19}$").WithMessage("PAN 12-19 haneli rakam olmalı.");
        RuleFor(x => x.Amount)
            .GreaterThan(0);
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("Currency 3 harf olmalı.")
            .Must(c => new[] { "TRY", "USD", "EUR" }.Contains(c.ToUpper()))
            .WithMessage("Desteklenmeyen para birimi.");
        RuleFor(x => x.Acquirer)
            .NotEmpty().MaximumLength(50);
    }
}
