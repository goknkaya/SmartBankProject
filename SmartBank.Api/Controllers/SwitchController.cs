// SmartBank.Api/Controllers/SwitchController.cs
using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.DTOs.Switching;
using SmartBank.Application.Interfaces;

[ApiController]
[Route("api/switch")]
public class SwitchController : ControllerBase
{
    private readonly ISwitchService _service;
    public SwitchController(ISwitchService service) => _service = service;

    [HttpPost("process")]
    public async Task<ActionResult<SelectSwitchMessageDto>> Process([FromBody] CreateSwitchMessageDto dto)
        => Ok(await _service.ProcessMessageAsync(dto));

    [HttpGet("messages")]
    public async Task<ActionResult<List<SelectSwitchMessageDto>>> Messages([FromQuery] string? status, [FromQuery] string? issuer)
        => Ok(await _service.GetMessagesAsync(status, issuer));

    [HttpGet("logs/{messageId:int}")]
    public async Task<ActionResult<List<SwitchLogDto>>> Logs([FromRoute] int messageId)
        => Ok(await _service.GetLogsAsync(messageId));
}
