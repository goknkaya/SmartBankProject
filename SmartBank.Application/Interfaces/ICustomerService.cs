using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBank.Application.DTOs;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(CustomerDto customer);
        Task<Customer> UpdateCustomerAsync(int id, CustomerDto customerDto);

    }
}
