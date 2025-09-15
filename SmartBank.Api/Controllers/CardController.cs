using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Card;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardController : ControllerBase
{
    private readonly ICardService _svc;
    public CardController(ICardService svc) => _svc = svc;

    // GET /api/Card
    [HttpGet]
    public async Task<ActionResult<List<SelectCardDto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    // GET /api/Card/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SelectCardDto>> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    // POST /api/Card
    [HttpPost]
    public async Task<ActionResult<SelectCardDto>> Create([FromBody] CreateCardDto dto)
    {
        try
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            // iş kuralı/unique/validasyon ihlali -> 409 (customer’da yaptığın gibi)
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT /api/Card/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCardDto dto)
    {
        try
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE /api/Card/{id}
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET /api/Card/{id}/pan
    [HttpGet("{id:int}/pan")]
    public async Task<ActionResult<CardPanDto>> GetPan(int id)
    {
        var dto = await _svc.GetPanAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }
}
