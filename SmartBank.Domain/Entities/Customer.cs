using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string NationalId { get; set; } // TCKN
        public DateTime CreatedAt { get; set; }
    }
}
