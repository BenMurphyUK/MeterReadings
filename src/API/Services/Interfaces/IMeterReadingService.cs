using MeterReadingsApi.Models.Responses;

namespace MeterReadingsApi.Services.Interfaces
{
    public interface IMeterReadingService
    {
        Task<MeterReadingUploadResponse> ProcessMeterReadingsAsync(Stream csvStream);
    }
}
