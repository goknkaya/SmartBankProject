using FluentValidation;
using SmartBank.Application.DTOs.Chargeback;

namespace SmartBank.Application.Validators.Chargeback;

public class DecideChargebackDtoValidator : AbstractValidator<DecideChargebackDto>
{
    // Kullanıcı hangi varyantı yazarsa yazsın kabul edilecek küme
    private static readonly string[] Allowed =
    [
        "won","lost","cancelled","canceled",
        "customer_wins","merchant_wins",
        "müşteri_kazandı","işyeri_kazandı" // istersen çıkar
    ];

    public DecideChargebackDtoValidator()
    {
        RuleFor(x => x.Decision)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("decision zorunludur.")
            .Must(d => Allowed.Contains(d.Trim().ToLowerInvariant()))
            .WithMessage("decision geçersiz. Desteklenen değerler: Won/Lost/Cancelled (veya CUSTOMER_WINS, MERCHANT_WINS).");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage("note en fazla 500 karakter olabilir.");
    }
}
