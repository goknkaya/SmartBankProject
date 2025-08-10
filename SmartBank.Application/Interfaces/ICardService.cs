using SmartBank.Application.DTOs.Card;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.Interfaces
{
    public interface ICardService
    {
        Task<List<SelectCardDto>> GetAllCardsAsync();
        Task<SelectCardDto?> GetCardByIdAsync(int id);
        Task<bool> CreateCardAsync(CreateCardDto dto);
        Task<bool> UpdateCardAsync(UpdateCardDto dto);
        Task<bool> DeleteCardAsync(int id);
    }
}