// SmartBank.Application/Interfaces/IClearingService.cs
using SmartBank.Application.DTOs.Clearing;

namespace SmartBank.Application.Interfaces
{
    public interface IClearingService
    {
        // 1) IN dosyası yükle, parse et, eşleştir ve batch oluştur
        Task<SelectClearingBatchDto> UploadIncomingAsync(IncomingUploadRequest req);

        // 2) OUT dosyası üret (günün başarılı işlemleri); dosya baytları ve batch özeti
        Task<(SelectClearingBatchDto batch, byte[] fileBytes, string fileName)> GenerateOutgoingAsync(DateTime settlementDate);

        // 3) Batch ve kayıt sorguları
        Task<SelectClearingBatchDto?> GetBatchAsync(int batchId);
        Task<List<SelectClearingBatchDto>> GetBatchesAsync(string? direction = null, DateTime? settlementDate = null);
        Task<List<SelectClearingRecordDto>> GetRecordsAsync(int batchId, string? matchStatus = null);

        // 4) Hatalı/eşleşmeyen kayıtları yeniden dene
        Task<int> RetryUnmatchedAsync(int batchId);
    }
}
