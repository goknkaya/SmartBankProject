// SmartBank.Application/MappingProfiles/ClearingProfile.cs
using AutoMapper;
using SmartBank.Application.DTOs.Clearing;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class ClearingProfile : Profile
    {
        public ClearingProfile()
        {
            CreateMap<ClearingBatch, SelectClearingBatchDto>();
            CreateMap<ClearingRecord, SelectClearingRecordDto>();

            // İstersen metadata’dan entity üretmek için:
            CreateMap<CreateClearingBatchDto, ClearingBatch>();
        }
    }
}
