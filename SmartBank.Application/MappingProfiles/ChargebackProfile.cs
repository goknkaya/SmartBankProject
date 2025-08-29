using AutoMapper;
using SmartBank.Application.DTOs.Chargeback;
using SmartBank.Domain.Entities.Chargeback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.MappingProfiles
{
    public class ChargebackProfile : Profile
    {
        public ChargebackProfile()
        {
            CreateMap<ChargebackCase, SelectChargebackCaseDto>();
            CreateMap<ChargebackEvent, SelectChargebackEventDto>();
        }
    }
}
