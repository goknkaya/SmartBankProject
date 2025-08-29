using FluentValidation;
using SmartBank.Application.DTOs.Chargeback;

public class AddEvidenceDtoValidator : AbstractValidator<AddEvidenceDto>
{
    public AddEvidenceDtoValidator()
    {
        RuleFor(x => x.EvidenceUrl)
    .NotEmpty()
    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _));
    }
}

