using FluentValidation;
using SmartBank.Application.DTOs.Customer;

namespace SmartBank.Application.DTOs.Validators.Customer
{
    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir müşteri ID’si girilmelidir.");

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email!)
                    .NotEmpty().WithMessage("Email boş olamaz.")
                    .EmailAddress().WithMessage("Geçerli bir email giriniz.");
            });

            When(x => x.PhoneNumber != null, () =>
            {
                RuleFor(x => x.PhoneNumber!)
                    .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
                    .Matches(@"^05\d{9}$").WithMessage("Geçerli bir telefon numarası giriniz.");
            });

            When(x => x.AddressLine != null, () =>
            {
                RuleFor(x => x.AddressLine!)
                    .NotEmpty().WithMessage("Adres boş olamaz.")
                    .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");
            });

            When(x => x.City != null, () =>
            {
                RuleFor(x => x.City!)
                    .NotEmpty().WithMessage("Şehir boş olamaz.");
            });

            When(x => x.Country != null, () =>
            {
                RuleFor(x => x.Country!)
                    .NotEmpty().WithMessage("Ülke boş olamaz.");
            });
        }
    }
}
