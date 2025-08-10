using SmartBank.Application.DTOs.Reversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface IReversalService
    {
        Task<bool> CreateReversalAsync(CreateReversalDto dto);
        Task<List<SelectReversalDto>> GetAllReversalsAsync();
        Task<SelectReversalDto> GetReversalByIdAsync(int id);
        Task<List<SelectReversalDto>> GetReversalsByTransactionIdAsync (int transactionId);
        Task<bool> VoidReversalAsync(int id, string performedBy, string reason);
    }
}
