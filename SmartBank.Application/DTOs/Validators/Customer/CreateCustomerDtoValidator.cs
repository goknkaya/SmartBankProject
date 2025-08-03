using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Customer;

namespace SmartBank.Application.DTOs.Validators.Customer
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad boş olamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad boş olamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email giriniz.");

            RuleFor(x => x.TCKN)
                .NotEmpty().WithMessage("TCKN boş olamaz.")
                .Length(11).WithMessage("TCKN 11 haneli olmalıdır.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon boş olamaz.")
                .Matches(@"^05\d{9}$").WithMessage("Geçerli bir telefon numarası giriniz.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Cinsiyet boş olamaz.")
                .Must(g => g == "E" || g == "K").WithMessage("Cinsiyet 'E' (Erkek) veya 'K' (Kadın) olmalıdır.");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Adres boş olamaz.")
                .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir boş olamaz.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Ülke boş olamaz.");
        }
    }
}
