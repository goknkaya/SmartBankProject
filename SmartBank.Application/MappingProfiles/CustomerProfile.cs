using AutoMapper;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Entity -> Select DTO
            CreateMap<Customer, SelectCustomerDto>()
                // BirthDate null ise MinValue geç; UI zaten boş gösterecek
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate ?? DateTime.MinValue));

            // Create DTO -> Entity
            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(d => d.BirthDate,
                    o => o.MapFrom(s => s.BirthDate == default ? (DateTime?)null : s.BirthDate));

            // Update DTO -> Entity (yalnızca dolu alanları yaz)
            CreateMap<UpdateCustomerDto, Customer>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
