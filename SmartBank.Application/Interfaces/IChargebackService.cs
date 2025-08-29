using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBank.Application.DTOs.Chargeback;

namespace SmartBank.Application.Interfaces
{
    public interface IChargebackService
    {
        Task<SelectChargebackCaseDto> OpenAsync(CreateChargebackDto dto);
        Task<List<SelectChargebackCaseDto>> ListAsync(string? status = null, int? txId = null);
        Task<List<SelectChargebackEventDto>> GetEventsAsync(int caseId);
        Task<SelectChargebackCaseDto> AddEvidenceAsync(int caseId, AddEvidenceDto dto);
        Task<SelectChargebackCaseDto> DecideAsync(int caseId, DecideChargebackDto dto);
    }
}
