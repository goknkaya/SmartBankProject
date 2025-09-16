using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core.Contracts;

public class CreateCardDto
{
    public int CustomerId { get; set; }
    public string CardHolderName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public string ExpiryMonth { get; set; } = "";     // "01".."12"
    public string ExpiryYear { get; set; } = "";      // "25" veya "2025"
    public string Currency { get; set; } = "TRY";
    public string PinHash { get; set; } = "";         // SHA-256 hex (64)
    public string CardStatus { get; set; } = "A";     // A/B/C
    public string CardType { get; set; } = "D";       // C/D/P
    public string CardStatusDescription { get; set; } = "";
    public bool IsVirtual { get; set; }
    public bool IsContactless { get; set; }
    public bool IsBlocked { get; set; }
    public decimal CardLimit { get; set; }
    public decimal DailyLimit { get; set; }
    public decimal TransactionLimit { get; set; }
    public int FailedPinAttempts { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string CardProvider { get; set; } = "V";   // V/M/T
    public string CardIssuerBank { get; set; } = "";
    public string CardStatusChangeReason { get; set; } = "";
    public int? ParentCardId { get; set; }
}

public class UpdateCardDto
{
    public int Id { get; set; }
    public string? CardStatus { get; set; }
    public string? CardStatusDescription { get; set; }
    public bool? IsBlocked { get; set; }
    public bool? IsContactless { get; set; }
    public bool? IsVirtual { get; set; }
    public decimal? CardLimit { get; set; }
    public decimal? DailyLimit { get; set; }
    public decimal? TransactionLimit { get; set; }
    public int? FailedPinAttempts { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? CardStatusChangeReason { get; set; }
    public string? CardProvider { get; set; }
    public string? CardIssuerBank { get; set; }
    public int? ParentCardId { get; set; }
}

public class SelectCardDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerNumber { get; set; }
    public string? MaskedCardNumber { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public string Currency { get; set; } = "TRY";
    public string CardStatus { get; set; } = "A";
    public string? CardStatusDescription { get; set; }
    public string CardType { get; set; } = "D";
    public bool IsVirtual { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsContactless { get; set; }
    public decimal CardLimit { get; set; }
    public decimal DailyLimit { get; set; }
    public decimal TransactionLimit { get; set; }
    public int FailedPinAttempts { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string CardProvider { get; set; } = "V";
    public string CardIssuerBank { get; set; } = "";
    public string CardStatusChangeReason { get; set; } = "";
    public int? ParentCardId { get; set; }
}
