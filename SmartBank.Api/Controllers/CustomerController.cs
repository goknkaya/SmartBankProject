using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.Interfaces;
using SmartBank.Application.DTOs.Customer;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return Ok(customer); // service bulunamazsa zaten exception atıyor
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var ok = await _customerService.CreateCustomerAsync(dto);
            if (!ok) return BadRequest("Müşteri oluşturulamadı.");

            // Varsa döneceğin id'yi service'ten alıp CreatedAtAction ile dönmek daha iyi olur
            // şimdilik 200 dönelim:
            return Ok("Müşteri başarıyla oluşturuldu.");
        }

        // Route id ile, body'deki Id'yi eşitle
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            if (dto.Id != 0 && dto.Id != id)
                return BadRequest("Body'deki Id ile route Id aynı olmalı.");

            dto.Id = id;
            var ok = await _customerService.UpdateCustomerAsync(dto);
            return ok ? Ok("Müşteri başarıyla güncellendi.") : BadRequest("Müşteri güncellenemedi.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var ok = await _customerService.DeleteCustomerAsync(id);
            return ok ? Ok("Müşteri başarıyla silindi.") : BadRequest("Müşteri silinemedi.");
        }
    }
}
