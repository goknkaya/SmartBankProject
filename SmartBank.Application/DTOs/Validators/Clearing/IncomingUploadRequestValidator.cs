// SmartBank.Application/DTOs/Validators/Clearing/IncomingUploadRequestValidator.cs
using FluentValidation;
using SmartBank.Application.DTOs.Clearing;

namespace SmartBank.Application.DTOs.Validators.Clearing
{
    public class IncomingUploadRequestValidator : AbstractValidator<IncomingUploadRequest>
    {
        public IncomingUploadRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Dosya zorunludur.")
                .Must(f => f.Length > 0).WithMessage("Dosya boş olamaz.")
                .Must(f => f.ContentType == "text/csv" || f.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                         || f.FileName.EndsWith(".psv", StringComparison.OrdinalIgnoreCase)
                         || f.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Sadece .csv/.psv/.txt kabul edilir.");

            RuleFor(x => x.Direction)
                .Equal("IN").WithMessage("Direction yalnızca 'IN' olabilir.");

            RuleFor(x => x.SettlementDate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1));

            RuleFor(x => x.Notes)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}
