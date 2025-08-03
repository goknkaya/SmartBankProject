using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Card;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public CustomerService(CustomerCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Tum musteriler gelir
        public async Task<List<SelectCustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _dbContext.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return _mapper.Map<List<SelectCustomerDto>>(customers);
        }

        // ID ' ye gore musteriler gelir
        public async Task<SelectCustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (customer == null)
                throw new KeyNotFoundException("Müşteri bulunamadı.");

            return _mapper.Map<SelectCustomerDto>(customer);
        }

        // Yeni musteri ekle
        public async Task<bool> CreateCustomerAsync(CreateCustomerDto dto)
        {
            if (await _dbContext.Customers.AnyAsync(c => c.TCKN == dto.TCKN && c.IsActive))
                throw new InvalidOperationException("Bu TCKN ile zaten kayıtlı bir müşteri var.");

            if (await _dbContext.Customers.AnyAsync(c => c.Email == dto.Email && c.IsActive))
                throw new InvalidOperationException("Bu e-posta adresi ile zaten kayıtlı bir müşteri var.");

            if (await _dbContext.Customers.AnyAsync(c => c.PhoneNumber == dto.PhoneNumber && c.IsActive))
                throw new InvalidOperationException("Bu telefon numarası ile zaten kayıtlı bir müşteri var.");

            var customer = _mapper.Map<Customer>(dto);
            customer.IsActive = true;
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        // Musteri guncelleme
        public async Task<bool> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.Id && c.IsActive);

            if (customer == null)
                throw new InvalidOperationException("Güncellenecek müşteri bulunamadı.");

            // Email başka biri tarafından kullanılıyor mu?
            if (!string.IsNullOrEmpty(dto.Email))
            {
                bool emailExists = await _dbContext.Customers
                    .AnyAsync(c => c.Email == dto.Email && c.Id != dto.Id && c.IsActive);

                if (emailExists)
                    throw new InvalidOperationException("Bu e-posta adresi başka bir müşteri tarafından kullanılıyor.");
            }

            // Telefon başka biri tarafından kullanılıyor mu?
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                bool phoneExists = await _dbContext.Customers
                    .AnyAsync(c => c.PhoneNumber == dto.PhoneNumber && c.Id != dto.Id && c.IsActive);

                if (phoneExists)
                    throw new InvalidOperationException("Bu telefon numarası başka bir müşteri tarafından kullanılıyor.");
            }

            _mapper.Map(dto, customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return true;
        }


        // Musteri silme
        public async Task<bool> DeleteCustomerAsync(DeleteCustomerDto dto)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == dto.Id && c.IsActive);
            if (customer == null)
                throw new InvalidOperationException("Silinmek istenen müşteri bulunamadı veya zaten pasif.");

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}