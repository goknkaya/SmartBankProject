// SmartBank.Application/DTOs/Clearing/SelectClearingBatchDto.cs
namespace SmartBank.Application.DTOs.Clearing
{
    public class SelectClearingBatchDto
    {
        public int Id { get; set; }
        public string Direction { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public DateTime SettlementDate { get; set; }
        public string Status { get; set; } = "N";
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
    }
}
