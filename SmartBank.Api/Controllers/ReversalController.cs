using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.Interfaces;
using SmartBank.Application.DTOs.Reversal;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReversalController : ControllerBase
    {
        private readonly IReversalService _reversalService;

        public ReversalController(IReversalService reversalService)
        {
            _reversalService = reversalService;
        }

        // POST: api/Reversal
        [HttpPost]
        public async Task<IActionResult> CreateReversal([FromBody] CreateReversalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ok = await _reversalService.CreateReversalAsync(dto);
                return ok ? Ok("Reversal başarıyla oluşturuldu.") : BadRequest("Reversal oluşturulamadı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Reversal/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReversalById(int id)
        {
            try
            {
                var item = await _reversalService.GetReversalByIdAsync(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Reversal/tx/{transactionId}
        [HttpGet("tx/{transactionId}")]
        public async Task<IActionResult> GetReversalByTransactionId(int transactionId)
        {
            var items = await _reversalService.GetReversalsByTransactionIdAsync(transactionId);
            return Ok(items);
        }

        // POST: api/Reversal/{id}/void
        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidReversal(int id, [FromBody] VoidReversalRequest body)
        {
            if (body is null || string.IsNullOrWhiteSpace(body.PerformedBy))
                return BadRequest("PerformedBy zorunludur.");

            try
            {
                var ok = await _reversalService.VoidReversalAsync(id, body.PerformedBy!, body.Reason ?? "");
                return ok ? Ok("Reversal başarıyla VOID edildi.") : BadRequest("Reversal VOID edilemedi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Void çağrısı için minimal request modeli
        public class VoidReversalRequest
        {
            public string? PerformedBy { get; set; }
            public string? Reason { get; set; }
        }
    }
}
