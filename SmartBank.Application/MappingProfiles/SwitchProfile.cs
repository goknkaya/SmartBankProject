using AutoMapper;
using SmartBank.Application.DTOs.Switching;
using SmartBank.Domain.Entities.Switching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
