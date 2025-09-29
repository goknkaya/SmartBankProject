using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Clearing;
using SmartBank.Application.Interfaces;

namespace SmartBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClearingController : ControllerBase
    {
        private readonly IClearingService _svc;
        public ClearingController(IClearingService svc) => _svc = svc;

        // 1) IN dosyası yükle (multipart/form-data)
        [HttpPost("in")]
        [RequestSizeLimit(200_000_000)]
        public async Task<IActionResult> UploadIncoming([FromForm] IncomingUploadRequest req)
        {
            if (req.File == null || req.File.Length == 0) return BadRequest("Dosya boş.");
            var dto = await _svc.UploadIncomingAsync(req);
            return Ok(dto);
        }

        // 2) OUT dosyası üret → CSV indir
        // Query: ?settlementDate=2025-08-15
        [HttpPost("outgoing")]
        [Produces("text/csv")]
        public async Task<IActionResult> GenerateOutgoing([FromQuery] DateTime settlementDate)
        {
            var (_, bytes, name) = await _svc.GenerateOutgoingAsync(settlementDate);
            return File(bytes, "text/csv", name);
        }

        // 3) Batch listesi
        [HttpGet("batches")]
        [ProducesResponseType(typeof(List<SelectClearingBatchDto>), 200)]
        public async Task<IActionResult> GetBatches([FromQuery] string? direction = null, [FromQuery] DateTime? settlementDate = null)
        {
            var list = await _svc.GetBatchesAsync(direction, settlementDate);
            return Ok(list);
        }

        // 4) Batch detay
        [HttpGet("batches/{batchId:int}")]
        [ProducesResponseType(typeof(SelectClearingBatchDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBatch([FromRoute] int batchId)
        {
            var dto = await _svc.GetBatchAsync(batchId);
            return dto == null ? NotFound() : Ok(dto);
        }

        // 5) Batch kayıtları
        [HttpGet("batches/{batchId:int}/records")]
        [ProducesResponseType(typeof(List<SelectClearingRecordDto>), 200)]
        public async Task<IActionResult> GetRecords([FromRoute] int batchId, [FromQuery] string? status = null)
        {
            var list = await _svc.GetRecordsAsync(batchId, status);
            return Ok(list);
        }

        // 6) Hatalıları yeniden dene
        [HttpPost("batches/{batchId:int}/retry")]
        public async Task<IActionResult> Retry([FromRoute] int batchId)
        {
            var fixedCount = await _svc.RetryUnmatchedAsync(batchId);
            return Ok(new { retried = fixedCount });
        }
    }
}
