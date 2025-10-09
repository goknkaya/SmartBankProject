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
                .Length(11).WithMessage("TCKN 11 haneli olmalıdır.")
                //.Must(IsValidTckn).WithMessage("TCKN geçersiz. TCKN' nin son basamağı tüm rakamların toplamının birler basamağı olmalıdır.");
                .Matches("^[0-9]+$").WithMessage("TCKN sadece rakamlardan oluşmalıdır.");


            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon boş olamaz.")
                .Matches(@"^05\d{9}$").WithMessage("Geçerli bir telefon numarası giriniz.");

            // M/F standardı
            RuleFor(x => x.Gender)
                .Must(g => string.IsNullOrWhiteSpace(g) || g is "M" or "F")
                .WithMessage("Cinsiyet 'M' (Male) veya 'F' (Female) olmalıdır.");

            RuleFor(x => x.AddressLine)
                .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");
        }

        private bool IsValidTckn(string t)
        {
            if (string.IsNullOrWhiteSpace(t) || t.Length != 11 || t[0] == '0' || !t.All(char.IsDigit))
                return false;

            int[] d = t.Select(c => c - '0').ToArray();
            int odd = d[0] + d[2] + d[4] + d[6] + d[8];
            int even = d[1] + d[3] + d[5] + d[7];
            int d10 = ((odd * 7) - even) % 10;
            int d11 = (d.Take(10).Sum()) % 10;
            return d[9] == d10 && d[10] == d11;
        }
    }
}
