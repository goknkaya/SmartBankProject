// SmartBank.Application/DTOs/Clearing/SelectClearingRecordDto.cs
namespace SmartBank.Application.DTOs.Clearing
{
    public class SelectClearingRecordDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int LineNumber { get; set; }
        public int? TransactionId { get; set; }
        public int? CardId { get; set; }
        public string? CardLast4 { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public DateTime? TransactionDate { get; set; }
        public string? MerchantName { get; set; }
        public string MatchStatus { get; set; } = "P";
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
