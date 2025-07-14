using MeterReadingsApi.Models.Dtos;

namespace MeterReadingsApi.Services.Interfaces
{
    public interface IValidationService
    {
        Task<(bool IsValid, List<string> Errors)> ValidateAsync(MeterReadingDto meterReading);
        Task<bool> IsAccountValidAsync(int accountId);
        bool IsReadingValueValid(int readingValue);
        Task<bool> IsDuplicateReadingAsync(int accountId, DateTime dateTime, int value);
        Task<bool> IsReadingNewerThanExistingAsync(int accountId, DateTime dateTime);
    }
}
