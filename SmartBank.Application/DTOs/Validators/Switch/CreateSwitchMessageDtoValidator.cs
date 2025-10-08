using FluentValidation;
using SmartBank.Application.DTOs.Switching;

namespace SmartBank.Application.Validators.Switch
{
    public class CreateSwitchMessageDtoValidator : AbstractValidator<CreateSwitchMessageDto>
    {
        public CreateSwitchMessageDtoValidator()
        {
            RuleFor(x => x.PAN)
                .NotEmpty()
                .Must(p => {
                    var digits = new string((p ?? "").Where(char.IsDigit).ToArray());
                    return digits.Length is >= 12 and <= 19;
                })
                .WithMessage("PAN 12–19 haneli rakam olmalı (sadece rakamlar).");

            RuleFor(x => x.Amount).GreaterThan(0);

            RuleFor(x => x.Currency)
                .NotEmpty().Length(3)
                .Must(c => new[] { "TRY", "USD", "EUR" }.Contains(c.ToUpper()))
                .WithMessage("Desteklenen para birimi: TRY, USD, EUR.");

            RuleFor(x => x.Acquirer).NotEmpty().MaximumLength(50);
        }
    }
}
