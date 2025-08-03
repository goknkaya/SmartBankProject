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
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src =>
                    MaskCardNumber(src.CardNumber)))
                .ReverseMap();

            CreateMap<CreateCardDto, Card>().ReverseMap();
            CreateMap<Card, UpdateCardDto>().ReverseMap();
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 10)
                return "**** **** **** ****"; // çok kısa, maskelenemez

            var digitsOnly = cardNumber.Replace(" ", "").Trim();

            int length = digitsOnly.Length;
            int visibleStart = 6;
            int visibleEnd = 4;

            if (length < visibleStart + visibleEnd)
                return "**** **** **** ****"; // yetersiz uzunluk

            var start = digitsOnly.Substring(0, visibleStart);
            var end = digitsOnly.Substring(length - visibleEnd);
            var maskLength = length - visibleStart - visibleEnd;

            var masked = new string('*', maskLength);

            string combined = start + masked + end;

            // 4'erli gruplama
            return string.Join(" ", Enumerable.Range(0, combined.Length / 4 + 1)
                .Select(i => combined.Skip(i * 4).Take(4))
                .Where(x => x.Any())
                .Select(x => new string(x.ToArray())));
        }

    }
}
