using MeterReadingsApi.Common.Constants;
using MeterReadingsApi.Data.Repositories.Interfaces;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Services.Interfaces;

namespace MeterReadingsApi.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMeterReadingRepository _meterReadingRepository;

        public ValidationService(
            IAccountRepository accountRepository,
            IMeterReadingRepository meterReadingRepository)
        {
            _accountRepository = accountRepository;
            _meterReadingRepository = meterReadingRepository;
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(MeterReadingDto meterReading)
        {
            var errors = new List<string>();

            if (!await IsAccountValidAsync(meterReading.AccountId))
            {
                errors.Add($"Account ID {meterReading.AccountId} does not exist");
            }

            if (!IsReadingValueValid(meterReading.MeterReadValue))
            {
                errors.Add($"Meter reading value {meterReading.MeterReadValue} is not in the correct format (NNNNN)");
            }

            if (await IsDuplicateReadingAsync(meterReading.AccountId, meterReading.MeterReadingDateTime, meterReading.MeterReadValue))
            {
                errors.Add($"Duplicate reading for Account {meterReading.AccountId} at {meterReading.MeterReadingDateTime}");
            }

            // Validate reading is newer than existing (Nice to have)
            if (!await IsReadingNewerThanExistingAsync(meterReading.AccountId, meterReading.MeterReadingDateTime))
            {
                errors.Add($"Reading for Account {meterReading.AccountId} at {meterReading.MeterReadingDateTime} is older than existing readings");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<bool> IsAccountValidAsync(int accountId)
        {
            return await _accountRepository.ExistsAsync(accountId);
        }

        public async Task<bool> IsDuplicateReadingAsync(int accountId, DateTime dateTime, int value)
        {
            return await _meterReadingRepository.ExistsAsync(accountId, dateTime, value);
        }

        public async Task<bool> IsReadingNewerThanExistingAsync(int accountId, DateTime dateTime)
        {
            var latestReading = await _meterReadingRepository.GetLatestByAccountAsync(accountId);

            // If no existing reading, any new reading is valid
            if (latestReading == null)
            {
                return true;
            }

            // New reading must be newer than the latest existing reading
            return dateTime > latestReading.MeterReadingDateTime;
        }

        public bool IsReadingValueValid(int readingValue)
        {
            // Check if value is within valid range (0-99999)
            if (readingValue < ValidationConstants.MeterReadingValueMinValue ||
                readingValue > ValidationConstants.MeterReadingValueMaxValue)
            {
                return false;
            }

            // If the "NNNNN" format is required, then we would need to check this as a string before parsing it to an integer.
            // But I assume that this validation requirement means that the value should just be within the specified range.

            return true;
        }
    }
}
