using AutoMapper;
using SmartBank.Application.DTOs.Transaction;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, SelectTransactionDto>().ReverseMap();
            CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
        }
    }
}
