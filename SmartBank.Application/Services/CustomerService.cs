using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs;
using SmartBank.Application.Interfaces;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

namespace SmartBank.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly SmartBankDbContext _dbContext;

        public CustomerService(SmartBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Tum musteriler gelir
        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            var result = await _dbContext.Customers.Select(c => new CustomerDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                NationalId = c.NationalId,
                CreatedAt = c.CreatedAt
            }).ToListAsync();

            return result;
        }

        // ID ' ye gore musteriler gelir
        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                NationalId = customer.NationalId,
                CreatedAt = customer.CreatedAt
            };
        }

        // Yeni musteri ekle
        public async Task<Customer> AddCustomerAsync(CustomerDto customerDto)
        {
            // TCKN kontrolu
            if (await _dbContext.Customers.AnyAsync(c => c.NationalId == customerDto.NationalId))
                throw new InvalidOperationException("Bu TCKN zaten sistemde kayıtlı.");

            // Mail kontrolu
            if (await _dbContext.Customers.AnyAsync(c => c.Email == customerDto.Email))
                throw new InvalidOperationException("Bu e-posta adresi zaten sistemde kayıtlı.");

            // Telefon kontrolu - sadece farkli TCKN ile eslesiyorsa engelle
            var existingPhone = await _dbContext.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == customerDto.PhoneNumber);
            if (existingPhone != null && existingPhone.NationalId != customerDto.NationalId)
                throw new InvalidOperationException("Bu telefon numarası başka bir kullanıcıya ait.");

            // Yeni musteri nesnesi olustur
            var newCustomer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                PhoneNumber = customerDto.PhoneNumber,
                NationalId = customerDto.NationalId,
                CreatedAt = customerDto.CreatedAt
            };

            _dbContext.Customers.Add(newCustomer);
            await _dbContext.SaveChangesAsync();

            return newCustomer;
        }

        // Musteri guncelleme

        public async Task<Customer> UpdateCustomerAsync(int id, CustomerDto customerDto)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null)
                throw new InvalidOperationException("Müşteri bulunamadı.");

            // Eşsiz alanlar için kontrol yapalım ama aynı ID' li kayıt dışında olacak şekilde
            if (await _dbContext.Customers.AnyAsync(c => c.NationalId == customerDto.NationalId))
                throw new InvalidOperationException("Bu TCKN başka bir müşteri tarafından kullanılıyor.");

            if (await _dbContext.Customers.AnyAsync(c => c.Email == customerDto.Email))
                throw new InvalidOperationException("Bu e-posta adresi başka bir müşteri tarafından kullanılıyor.");

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.NationalId = customerDto.NationalId;
            customer.CreatedAt = customerDto.CreatedAt;

            await _dbContext.SaveChangesAsync();

            return customer;
        }
    }
}