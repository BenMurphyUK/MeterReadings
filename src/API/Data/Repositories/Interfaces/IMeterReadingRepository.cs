using MeterReadingsApi.Data.Entities;

namespace MeterReadingsApi.Data.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<MeterReading?> GetByAccountAndDateTimeAsync(int accountId, DateTime dateTime);
        Task<bool> ExistsAsync(int accountId, DateTime dateTime, int value);
        Task<MeterReading?> GetLatestByAccountAsync(int accountId);
        void Add(MeterReading meterReading);
        void AddRange(List<MeterReading> meterReadings);
        Task<int> SaveChangesAsync();
    }
}
