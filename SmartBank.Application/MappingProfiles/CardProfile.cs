using System;
using System.Linq;
using AutoMapper;
using SmartBank.Application.DTOs.Card;
using SmartBank.Domain.Entities;

namespace SmartBank.Application.MappingProfiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<Card, SelectCardDto>()
            .ForMember(d => d.MaskedCardNumber,
                opt => opt.MapFrom(s => MaskCardNumber(s.CardNumber)))
            .ForMember(d => d.CustomerFullName,
                opt => opt.MapFrom(s => s.Customer != null ? s.Customer.FullName : null))
            .ForMember(d => d.CustomerNumber,
                opt => opt.MapFrom(s => s.Customer != null ? s.Customer.TCKN : null))
            .ForMember(d => d.ExpiryMonth,
                opt => opt.MapFrom(s => s.ExpiryDate.Month.ToString("00")))
            .ForMember(d => d.ExpiryYear,
                opt => opt.MapFrom(s => s.ExpiryDate.ToString("yy")))
            .ReverseMap();

        CreateMap<CreateCardDto, Card>()
            .ForMember(d => d.ExpiryDate,
                opt => opt.MapFrom(src => new DateTime(
                    src.ExpiryYear.Length == 2 ? 2000 + int.Parse(src.ExpiryYear) : int.Parse(src.ExpiryYear),
                    int.Parse(src.ExpiryMonth),
                    DateTime.DaysInMonth(
                        src.ExpiryYear.Length == 2 ? 2000 + int.Parse(src.ExpiryYear) : int.Parse(src.ExpiryYear),
                        int.Parse(src.ExpiryMonth)
                    ))));

        CreateMap<Card, UpdateCardDto>().ReverseMap();
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "**** **** **** ****";

            var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length < 10) return "**** **** **** ****";

            // 6 baş, 4 son, arası maskeli
            int visibleStart = 6, visibleEnd = 4, len = digitsOnly.Length;
            if (len < visibleStart + visibleEnd) return "**** **** **** ****";

            var start = digitsOnly[..visibleStart];
            var end = digitsOnly[^visibleEnd..];
            var masked = new string('*', len - visibleStart - visibleEnd);
            var combined = start + masked + end;

            // 4’erli gruplama
            return string.Join(" ",
                Enumerable.Range(0, (combined.Length + 3) / 4)
                          .Select(i => combined.Skip(i * 4).Take(4))
                          .Where(g => g.Any())
                          .Select(g => new string(g.ToArray())));
        }
    }
}
