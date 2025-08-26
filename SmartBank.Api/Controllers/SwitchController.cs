using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Switching;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SwitchController : ControllerBase
    {
        private readonly ISwitchService _switchService;

        public SwitchController(ISwitchService switchService)
        {
            _switchService = switchService;
        }

        /// <summary>
        /// Switch mesajını işler: BIN' den issuer bulur, gerçek kartı doğrular,
        /// idempotency/duplicate guard uygular, gerekiyorsa Transaction oluşturur.
        /// </summary>

        [HttpPost("process")]
        [ProducesResponseType(typeof(SelectSwitchMessageDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Process([FromBody] CreateSwitchMessageDto dto, CancellationToken cancellationToken)
        {
            if (dto is null) return BadRequest(new { error = "Body boş olamaz." });
            if (string.IsNullOrWhiteSpace(dto.PAN)) return BadRequest(new { error = "PAN zorunlu." });
            if (string.IsNullOrWhiteSpace(dto.Currency)) dto.Currency = "TRY";
            if (string.IsNullOrWhiteSpace(dto.Acquirer)) dto.Acquirer = "DemoPOS";

            try
            {
                var res = await _switchService.ProcessMessageAsync(dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        /// <summary>
        /// İşlenen switch mesajlarını listeler (opsiyonel status/issuer filtresi).
        /// </summary>

        [HttpGet("messages")]
        [ProducesResponseType(typeof(List<SelectSwitchMessageDto>), 200)]
        public async Task<IActionResult> Messages([FromQuery] string? status = null, [FromQuery] string? issuer = null, CancellationToken cancellationToken = default)
        {
            var list = await _switchService.GetMessagesAsync(status, issuer);
            return Ok(list);
        }

        /// <summary>
        /// Belirli bir mesajın loglarını dönerç
        /// </summary>

        [HttpGet("logs/{messageId:int}")]
        [ProducesResponseType(typeof(List<object>), 200)]
        public async Task<IActionResult> Logs([FromRoute] int messageId, CancellationToken cancellationToken = default)
        {
            var list = await _switchService.GetLogsAsync(messageId);
            return Ok(list);
        }
    }
}
