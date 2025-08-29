using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Chargeback;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargebackController : ControllerBase
    {
        public readonly IChargebackService _chargebackService;
        public ChargebackController(IChargebackService chargebackService) => _chargebackService = chargebackService;

        [HttpPost("open")]
        public async Task<IActionResult> Open([FromBody] CreateChargebackDto dto) 
            => Ok(await _chargebackService.OpenAsync(dto));

        [HttpGet("cases")]
        public async Task<IActionResult> Cases([FromQuery] string? status = null, [FromQuery] int? txId = null)
            => Ok(await _chargebackService.ListAsync(status, txId));

        [HttpGet("{caseId:int}/events")]
        public async Task<IActionResult> Events([FromRoute] int caseId)
            => Ok(await _chargebackService.GetEventsAsync(caseId));

        [HttpPost("{caseId:int}/evidence")]
        public async Task<IActionResult> Evidence([FromRoute] int caseId, [FromBody] AddEvidenceDto dto)
            => Ok(await _chargebackService.AddEvidenceAsync(caseId, dto));

        [HttpPost("{caseId:int}/decide")]
        public async Task<IActionResult> Decide([FromRoute] int caseId, [FromBody] DecideChargebackDto dto)
            => Ok(await _chargebackService.DecideAsync(caseId, dto));
    }
}
