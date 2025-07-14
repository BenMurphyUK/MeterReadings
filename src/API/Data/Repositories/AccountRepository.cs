using MeterReadingsApi.Data.Entities;
using MeterReadingsApi.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MeterReadingsDbContext _context;

        public AccountRepository(MeterReadingsDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(x => x.MeterReadings)
                .FirstOrDefaultAsync(x => x.Id == accountId);
        }

        public async Task<bool> ExistsAsync(int accountId)
        {
            return await _context.Accounts
                .AnyAsync(x => x.Id == accountId);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(x => x.MeterReadings)
                .ToListAsync();
        }

        public void AddRange(List<Account> accounts)
        {
            _context.Accounts.AddRange(accounts);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
