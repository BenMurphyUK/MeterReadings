using MeterReadingsApi.Data.Entities;
using MeterReadingsApi.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi.Data.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly MeterReadingsDbContext _context;

        public MeterReadingRepository(MeterReadingsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int accountId, DateTime dateTime, int value)
        {
            return await _context.MeterReadings
                .AnyAsync(x => x.AccountId == accountId 
                    && x.MeterReadingDateTime == dateTime 
                    && x.MeterReadValue == value);
        }

        public async Task<MeterReading?> GetLatestByAccountAsync(int accountId)
        {
            return await _context.MeterReadings
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.MeterReadingDateTime)
                .FirstOrDefaultAsync();
        }

        public void AddRange(List<MeterReading> meterReadings)
        {
            _context.MeterReadings.AddRange(meterReadings);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }

}
