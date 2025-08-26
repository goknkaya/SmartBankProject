using SmartBank.Application.DTOs.Switching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface ISwitchService
    {
        Task<SelectSwitchMessageDto> ProcessMessageAsync(CreateSwitchMessageDto dto);
        Task<List<SelectSwitchMessageDto>> GetMessagesAsync(string? status  = null, string? issuer = null);
        Task<List<object>> GetLogsAsync(int messageId);
    }
}
