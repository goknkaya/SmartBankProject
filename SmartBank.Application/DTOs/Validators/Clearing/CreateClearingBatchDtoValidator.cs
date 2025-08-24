// SmartBank.Application/DTOs/Validators/Clearing/CreateClearingBatchDtoValidator.cs
using FluentValidation;
using SmartBank.Application.DTOs.Clearing;
using System.Text.RegularExpressions;

namespace SmartBank.Application.DTOs.Validators.Clearing
{
    public class CreateClearingBatchDtoValidator : AbstractValidator<CreateClearingBatchDto>
    {
        public CreateClearingBatchDtoValidator()
        {
            RuleFor(x => x.SettlementDate)
                .NotEmpty().WithMessage("SettlementDate zorunludur.")
                .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
                .WithMessage("SettlementDate gelecekte olamaz.");

            RuleFor(x => x.Direction)
                .NotEmpty().WithMessage("Direction zorunludur.")
                .Must(d => d == "IN")
                .WithMessage("Direction yalnızca 'IN' olabilir.");

            RuleFor(x => x.FileName)
                .MaximumLength(255).When(x => !string.IsNullOrWhiteSpace(x.FileName))
                .Must(HasAllowedExtension).When(x => !string.IsNullOrWhiteSpace(x.FileName))
                .WithMessage("Sadece .csv, .psv, .txt kabul edilir.");

            RuleFor(x => x.FileHash)
                .Must(IsSha256Hex).When(x => !string.IsNullOrWhiteSpace(x.FileHash))
                .WithMessage("FileHash 64 uzunlukta SHA-256 hex olmalıdır.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }

        static bool HasAllowedExtension(string? fileName)
        {
            var ext = Path.GetExtension(fileName ?? string.Empty)?.ToLowerInvariant();
            return ext is ".csv" or ".psv" or ".txt";
        }

        static bool IsSha256Hex(string? s)
            => !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s, "^[a-fA-F0-9]{64}$");
    }
}
