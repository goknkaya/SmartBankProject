using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Chargeback;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargebackController : ControllerBase
    {
        private readonly IChargebackService _chargebackService;
        public ChargebackController(IChargebackService chargebackService)
            => _chargebackService = chargebackService;

        // Case açma
        [HttpPost("open")]
        public async Task<IActionResult> Open([FromBody] CreateChargebackDto dto)
            => Ok(await _chargebackService.OpenAsync(dto));

        // Listeleme (status ve transactionId opsiyonel filtre)
        [HttpGet("cases")]
        public async Task<IActionResult> Cases([FromQuery] string? status = null, [FromQuery] int? transactionId = null)
            => Ok(await _chargebackService.ListAsync(status, transactionId));

        // Tekil case detay (ID üzerinden)
        [HttpGet("cases/{caseId:int}")]
        public async Task<IActionResult> CaseById([FromRoute] int caseId)
        {
            var cb = await _chargebackService.GetCaseAsync(caseId);
            return cb == null ? NotFound() : Ok(cb);
        }

        // Case’e ait event listesi
        [HttpGet("cases/{caseId:int}/events")]
        public async Task<IActionResult> Events([FromRoute] int caseId)
            => Ok(await _chargebackService.GetEventsAsync(caseId));

        // Evidence ekleme
        [HttpPost("cases/{caseId:int}/evidence")]
        public async Task<IActionResult> Evidence([FromRoute] int caseId, [FromBody] AddEvidenceDto dto)
            => Ok(await _chargebackService.AddEvidenceAsync(caseId, dto));

        // Karar verme
        [HttpPost("cases/{caseId:int}/decide")]
        public async Task<IActionResult> Decide([FromRoute] int caseId, [FromBody] DecideChargebackDto dto)
            => Ok(await _chargebackService.DecideAsync(caseId, dto));
    }
}
