using FluentValidation;
using SmartBank.Application.DTOs.Chargeback;

namespace SmartBank.Application.Validators.Chargeback;

public class AddEvidenceDtoValidator : AbstractValidator<AddEvidenceDto>
{
    public AddEvidenceDtoValidator()
    {
        // En az biri dolu olmalı
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Note) || !string.IsNullOrWhiteSpace(x.EvidenceUrl))
            .WithMessage("note veya evidenceUrl alanlarından en az biri dolu olmalıdır.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("note en fazla 500 karakter olabilir.");

        RuleFor(x => x.EvidenceUrl)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("evidenceUrl geçerli bir URL olmalıdır.");
    }
}
