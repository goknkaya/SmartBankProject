using FluentValidation;
using SmartBank.Application.DTOs.Clearing;

public class IncomingUploadRequestValidator : AbstractValidator<IncomingUploadRequest>
{
    public IncomingUploadRequestValidator()
    {
        RuleFor(x => x.File).NotNull().Must(f => f.Length > 0)
            .WithMessage("Dosya boş olamaz.");
        RuleFor(x => x.Direction)
            .Equal("IN");
        RuleFor(x => x.SettlementDate)
            .NotEmpty();
    }
}

