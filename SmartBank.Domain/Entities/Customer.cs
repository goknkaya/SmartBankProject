using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        // Ad/soyad artık ayrı sütunlar
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string TCKN { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // 1900 sentinel derdinden kurtulmak için nullable öneriyorum
        public DateTime? BirthDate { get; set; }

        // “M/F” ya da “Male/Female” bekliyoruz, ama nullable da olabilir
        public string? Gender { get; set; }

        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Card> Cards { get; set; } = new List<Card>();

        // Opsiyonel: UI/rapor için hesaplanan özellik
        [NotMapped]
        public string FullName => string.Join(" ",
            new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
