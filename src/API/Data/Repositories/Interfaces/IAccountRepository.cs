using MeterReadingsApi.Data.Entities;

namespace MeterReadingsApi.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> ExistsAsync(int accountId);
    }
}
