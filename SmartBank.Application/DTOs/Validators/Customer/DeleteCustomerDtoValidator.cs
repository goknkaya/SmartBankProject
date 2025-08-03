using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartBank.Application.DTOs.Customer;

namespace SmartBank.Application.DTOs.Validators.Customer
{
    public class DeleteCustomerDtoValidator : AbstractValidator<DeleteCustomerDto>
    {
        public DeleteCustomerDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id boş olamaz.")
                .GreaterThan(0).WithMessage("Id pozitif bir değer olmalıdır.");
        }
    }
}
