using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Card;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers
{
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

        // GET /api/Card/5
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
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /api/Card/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCardDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent(); // 204
        }

        // DELETE /api/Card/5  (soft delete içeride)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent(); // 204
        }
    }
}
