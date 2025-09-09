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
            // READ (Select)
            CreateMap<Card, SelectCardDto>()
                // 4546 03** **** 4508 gibi maskeleme
                .ForMember(d => d.MaskedCardNumber,
                    opt => opt.MapFrom(s => MaskCardNumber(s.CardNumber)))

                // Müşteri Ad Soyad (FirstName + LastName; yoksa null)
                .ForMember(d => d.CustomerFullName,
                    opt => opt.MapFrom(s =>
                        s.Customer == null
                            ? null
                            : string.Join(" ",
                                new[] { s.Customer.FirstName, s.Customer.LastName }
                                    .Where(x => !string.IsNullOrWhiteSpace(x)))))

                // Müşteri numarası (TCKN)
                .ForMember(d => d.CustomerNumber,
                    opt => opt.MapFrom(s => s.Customer != null ? s.Customer.TCKN : null))

                // Ay (MM)
                .ForMember(d => d.ExpiryMonth,
                    opt => opt.MapFrom(s => s.ExpiryDate.Month.ToString("00")))

                // Yıl (yy)
                .ForMember(d => d.ExpiryYear,
                    opt => opt.MapFrom(s => s.ExpiryDate.ToString("yy")));

            // CREATE
            CreateMap<CreateCardDto, Card>()
                .ForMember(d => d.ExpiryDate,
                    opt => opt.MapFrom(src => BuildExpiryDate(src)));

            // UPDATE (null gelen alanları dokunma)
            CreateMap<Card, UpdateCardDto>().ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }

        // ---- helpers ----

        private static DateTime BuildExpiryDate(CreateCardDto src)
        {
            if (string.IsNullOrWhiteSpace(src.ExpiryMonth))
                throw new ArgumentException("ExpiryMonth is required.");
            if (string.IsNullOrWhiteSpace(src.ExpiryYear))
                throw new ArgumentException("ExpiryYear is required.");

            var month = int.Parse(src.ExpiryMonth);
            if (month is < 1 or > 12)
                throw new ArgumentOutOfRangeException(nameof(src.ExpiryMonth), "Month must be 01..12.");

            // "25" -> 2025, "2027" -> 2027
            var year = src.ExpiryYear.Length == 2
                ? 2000 + int.Parse(src.ExpiryYear)
                : int.Parse(src.ExpiryYear);

            // Kartların son günü o ayın son günü olarak kabul edilir
            var day = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, day);
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "**** **** **** ****";

            var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length < 10) return "**** **** **** ****";

            // 6 baş + 4 son; arası maskeli
            const int visibleStart = 6;
            const int visibleEnd = 4;
            var len = digitsOnly.Length;
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
