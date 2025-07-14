using MeterReadingsApi.Data.Entities;

namespace MeterReadingsApi.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<bool> ExistsAsync(int accountId);
        Task<List<Account>> GetAllAsync();
        void AddRange(List<Account> accounts);
        Task<int> SaveChangesAsync();
    }
}
