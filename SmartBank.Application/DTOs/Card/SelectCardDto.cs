public class SelectCardDto
{
    public int Id { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerNumber { get; set; }
    public string? MaskedCardNumber { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public string Currency { get; set; }
    public string CardStatus { get; set; }
    public string? CardStatusDescription { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsBlocked { get; set; }
    public decimal CardLimit { get; set; }
    public decimal DailyLimit { get; set; }
    public decimal TransactionLimit { get; set; }
    public int FailedPinAttempts { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string CardProvider { get; set; }
    public string CardIssuerBank { get; set; }
    public string CardStatusChangeReason { get; set; }
    public int? ParentCardId { get; set; }
    public bool IsContactless { get; set; }
}

public class CardDetailDto : SelectCardDto
{
    public string CardNumber { get; set; } = "";
}