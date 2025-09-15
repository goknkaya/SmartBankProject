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
        Task<List<SelectCardDto>> GetAllAsync();
        Task<SelectCardDto?> GetByIdAsync(int id);
        Task<SelectCardDto> CreateAsync(CreateCardDto dto);
        Task UpdateAsync(int id, UpdateCardDto dto);
        Task DeleteAsync(int id);
        Task<CardPanDto?> GetPanAsync(int id);

    }
}