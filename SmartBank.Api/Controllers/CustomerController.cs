using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _svc;
    public CustomerController(ICustomerService svc) => _svc = svc;

    // GET /api/Customer
    [HttpGet]
    public async Task<ActionResult<List<SelectCustomerDto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    // GET /api/Customer/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SelectCustomerDto>> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    // POST /api/Customer
    [HttpPost]
    public async Task<ActionResult<SelectCustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        // 201 Created + Location header
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/Customer/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        await _svc.UpdateAsync(id, dto);
        return NoContent(); // 204
    }

    // DELETE /api/Customer/{id}  (soft delete içeride)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent(); // 204
    }
}
