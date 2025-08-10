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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound("Müşteri bulunamadı.");

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.CreateCustomerAsync(dto);
            return result ? Ok("Müşteri başarıyla oluşturuldu.") : BadRequest("Müşteri oluşturulamadı.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.UpdateCustomerAsync(dto);
            return result ? Ok("Müşteri başarıyla güncellendi.") : BadRequest("Müşteri güncellenemedi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.DeleteCustomerAsync(id);
            return result ? Ok("Müşteri başarıyla silindi.") : BadRequest("Müşteri silinemedi.");
        }

    }
}
