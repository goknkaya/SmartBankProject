using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core.Contracts;

public class CreateCustomerDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string TCKN { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = "Unknown";
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}

public class UpdateCustomerDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}

public class SelectCustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string TCKN { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = "";
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

