using SmartBank.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> CreateTransactionAsync (CreateTransactionDto dto);
        Task<List<SelectTransactionDto>> GetAllTransactionsAsync();
        Task<SelectTransactionDto?> GetTransactionByIdAsync (int id);
        Task<List<SelectTransactionDto>> GetTransactionByCardIdAsync (int cardId);
    }
}
