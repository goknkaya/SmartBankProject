using AutoMapper;
using SmartBank.Application.DTOs.Reversal;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class ReversalProfile : Profile
    {
        public ReversalProfile()
        {
            CreateMap<Reversal, SelectReversalDto>();
            CreateMap<CreateReversalDto, Reversal>();
        }
    }
}
