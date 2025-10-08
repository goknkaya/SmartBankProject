using SmartBank.Application.DTOs.Switching;

namespace SmartBank.Application.Interfaces
{
    public interface ISwitchService
    {
        Task<SelectSwitchMessageDto> ProcessMessageAsync(CreateSwitchMessageDto dto);
        Task<List<SelectSwitchMessageDto>> GetMessagesAsync(string? status = null, string? issuer = null);
        Task<List<SwitchLogDto>> GetLogsAsync(int messageId);
    }
}
