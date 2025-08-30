using FluentValidation;
using SmartBank.Application.DTOs.Clearing;

namespace SmartBank.Application.Validators.Clearing
{
    public class IncomingUploadRequestValidator : AbstractValidator<IncomingUploadRequest>
    {
        public IncomingUploadRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .Must(f => f.Length > 0)
                .WithMessage("Dosya boş olamaz.");

            // Bu validator "incoming" endpoint'i için; direction IN bekliyoruz.
            RuleFor(x => x.Direction)
                .Equal("IN");

            RuleFor(x => x.SettlementDate)
                .NotEmpty();
        }
    }
}
