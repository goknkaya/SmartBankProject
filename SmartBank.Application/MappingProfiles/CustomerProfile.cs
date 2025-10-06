using AutoMapper;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Entity -> Select DTO (null ise MinValue)
            CreateMap<Customer, SelectCustomerDto>()
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate ?? DateTime.MinValue));

            // Create DTO -> Entity (default 01.01.0001 ise null'a çevir)
            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(d => d.BirthDate,
                    o => o.MapFrom(s => s.BirthDate == default ? (DateTime?)null : s.BirthDate));

            // Update DTO -> Entity (sadece dolu alanlar)
            CreateMap<UpdateCustomerDto, Customer>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
