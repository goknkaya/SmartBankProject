using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerCoreDbContext _db;
        private readonly IMapper _mapper;

        public CustomerService(CustomerCoreDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET list
        public async Task<List<SelectCustomerDto>> GetAllAsync()
        {
            var customers = await _db.Customers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return _mapper.Map<List<SelectCustomerDto>>(customers);
        }

        // GET by id
        public async Task<SelectCustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return customer is null ? null : _mapper.Map<SelectCustomerDto>(customer);
        }

        // POST -> created dto
        public async Task<SelectCustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            // Uniqueness kontrolleri
            if (await _db.Customers.AnyAsync(c => c.TCKN == dto.TCKN && c.IsActive))
                throw new InvalidOperationException("Bu TCKN ile zaten kayıtlı bir müşteri var.");

            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                await _db.Customers.AnyAsync(c => c.Email == dto.Email && c.IsActive))
                throw new InvalidOperationException("Bu e-posta adresi ile zaten kayıtlı bir müşteri var.");

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) &&
                await _db.Customers.AnyAsync(c => c.PhoneNumber == dto.PhoneNumber && c.IsActive))
                throw new InvalidOperationException("Bu telefon numarası ile zaten kayıtlı bir müşteri var.");

            var customer = _mapper.Map<Customer>(dto);
            customer.IsActive = true;
            customer.CreatedAt = DateTime.Now;
            customer.UpdatedAt = DateTime.Now;

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return _mapper.Map<SelectCustomerDto>(customer);
        }

        // PUT -> 204 NoContent (gövde yok)
        public async Task UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
                ?? throw new KeyNotFoundException("Güncellenecek müşteri bulunamadı.");

            // Email başka biri tarafından kullanılıyor mu?
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                bool emailExists = await _db.Customers
                    .AnyAsync(c => c.Email == dto.Email && c.Id != id && c.IsActive);
                if (emailExists)
                    throw new InvalidOperationException("Bu e-posta adresi başka bir müşteri tarafından kullanılıyor.");
            }

            // Telefon başka biri tarafından kullanılıyor mu?
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                bool phoneExists = await _db.Customers
                    .AnyAsync(c => c.PhoneNumber == dto.PhoneNumber && c.Id != id && c.IsActive);
                if (phoneExists)
                    throw new InvalidOperationException("Bu telefon numarası başka bir müşteri tarafından kullanılıyor.");
            }

            // Sadece update alanlarını uygula
            customer.Email = dto.Email ?? customer.Email;
            customer.PhoneNumber = dto.PhoneNumber ?? customer.PhoneNumber;
            customer.AddressLine = dto.AddressLine ?? customer.AddressLine;
            customer.City = dto.City ?? customer.City;
            customer.Country = dto.Country ?? customer.Country;

            customer.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        // DELETE -> soft delete
        public async Task DeleteAsync(int id)
        {
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
                ?? throw new KeyNotFoundException("Silinmek istenen müşteri bulunamadı veya zaten pasif.");

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }
    }
}
