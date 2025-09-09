using SmartBank.Application.DTOs.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<List<SelectCustomerDto>> GetAllAsync();
        Task<SelectCustomerDto?> GetByIdAsync(int id);
        Task<SelectCustomerDto> CreateAsync(CreateCustomerDto dto);
        Task UpdateAsync(int id, UpdateCustomerDto dto); 
        Task DeleteAsync(int id);                        
    }
}
