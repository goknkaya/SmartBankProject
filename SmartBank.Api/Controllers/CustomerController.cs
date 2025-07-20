using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.Interfaces;
using SmartBank.Application.DTOs;

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
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            var createdCustomer = await _customerService.AddCustomerAsync(customerDto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
        {
            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
                return Ok(updatedCustomer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
