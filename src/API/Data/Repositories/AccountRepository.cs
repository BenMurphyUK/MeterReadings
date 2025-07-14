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

        public async Task<bool> ExistsAsync(int accountId)
        {
            return await _context.Accounts
                .AnyAsync(x => x.Id == accountId);
        }
    }
}
