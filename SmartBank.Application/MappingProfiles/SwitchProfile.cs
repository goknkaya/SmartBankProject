using AutoMapper;
using SmartBank.Application.DTOs.Switching;
using SmartBank.Domain.Entities.Switching;

namespace SmartBank.Application.MappingProfiles
{
    public class SwitchProfile : Profile
    {
        public SwitchProfile()
        {
            CreateMap<SwitchMessage, SelectSwitchMessageDto>().ReverseMap();
        }
    }
}
