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
        try
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            // Aynılık (unique) ihlali vb. iş kuralı -> 409
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT /api/Customer/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SelectCustomerDto>> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            await _svc.UpdateAsync(id, dto);
            var updated = await _svc.GetByIdAsync(id);
            return updated is null ? NotFound() : Ok(updated); // 200 + body
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }


    // DELETE /api/Customer/{id}  (soft delete içeride)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
