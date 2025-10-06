using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBank.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [MaxLength(50)] public string FirstName { get; set; } = null!;
        [MaxLength(50)] public string LastName { get; set; } = null!;

        [MaxLength(11)] public string TCKN { get; set; } = null!;
        [MaxLength(100)] public string Email { get; set; } = null!;
        [MaxLength(20)] public string PhoneNumber { get; set; } = null!;

        public DateTime? BirthDate { get; set; }
        [MaxLength(10)] public string? Gender { get; set; }   // "M" / "F" / null

        [MaxLength(250)] public string? AddressLine { get; set; }
        [MaxLength(100)] public string? City { get; set; }
        [MaxLength(100)] public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Card> Cards { get; set; } = new List<Card>();

        [NotMapped]
        public string FullName => string.Join(" ",
            new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
