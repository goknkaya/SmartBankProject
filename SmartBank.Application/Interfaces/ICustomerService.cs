using SmartBank.Application.DTOs.Card;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<List<SelectCustomerDto>> GetAllCustomersAsync();
        Task<SelectCustomerDto?> GetCustomerByIdAsync(int id);
        Task<bool> CreateCustomerAsync(CreateCustomerDto dto);
        Task<bool> UpdateCustomerAsync(UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(int id);
    }
}