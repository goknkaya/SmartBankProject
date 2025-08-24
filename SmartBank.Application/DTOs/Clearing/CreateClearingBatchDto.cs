// SmartBank.Application/DTOs/Clearing/CreateClearingBatchDto.cs
namespace SmartBank.Application.DTOs.Clearing
{
    public class CreateClearingBatchDto
    {
        public string Direction { get; set; } = "IN";
        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public DateTime SettlementDate { get; set; }
        public string? Notes { get; set; }
    }
}
