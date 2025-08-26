using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities.Switching
{
    public class CardBin
    {
        public int Id { get; set; }

        [MaxLength(6)]
        public string Bin { get; set; } = "";     // 450803

        [MaxLength(50)]
        public string Issuer { get; set; } = "";  // SmartBank/AnotherBank
        public bool IsDomestic { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}
