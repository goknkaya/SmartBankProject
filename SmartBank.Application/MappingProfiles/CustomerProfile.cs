using AutoMapper;
using SmartBank.Application.DTOs.Customer;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, SelectCustomerDto>().ReverseMap();
            CreateMap<CreateCustomerDto, Customer>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
    .ReverseMap();

            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
        }
    }
}
