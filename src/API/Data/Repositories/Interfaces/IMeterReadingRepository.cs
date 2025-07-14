using MeterReadingsApi.Data.Entities;

namespace MeterReadingsApi.Data.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<bool> ExistsAsync(int accountId, DateTime dateTime, int value);
        Task<MeterReading?> GetLatestByAccountAsync(int accountId);
        void AddRange(List<MeterReading> meterReadings);
        Task<int> SaveChangesAsync();
    }
}
