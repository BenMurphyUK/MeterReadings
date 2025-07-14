using MeterReadingsApi.Data.Entities;
using MeterReadingsApi.Data.Repositories.Interfaces;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Models.Responses;
using MeterReadingsApi.Services.Interfaces;

namespace MeterReadingsApi.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly ICsvParserService _csvParserService;
        private readonly IValidationService _validationService;
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly ILogger<MeterReadingService> _logger;

        public MeterReadingService(
            ICsvParserService csvParserService,
            IValidationService validationService,
            IMeterReadingRepository meterReadingRepository,
            ILogger<MeterReadingService> logger)
        {
            _csvParserService = csvParserService;
            _validationService = validationService;
            _meterReadingRepository = meterReadingRepository;
            _logger = logger;
        }

        public async Task<MeterReadingUploadResponse> ProcessMeterReadingsAsync(Stream csvStream)
        {
            var response = new MeterReadingUploadResponse();

            // Parse CSV
            var meterReadingDtos = await _csvParserService.ParseCsvAsync(csvStream);
            _logger.LogInformation("Starting to process meter readings CSV");

            var validReadings = new List<MeterReading>();

            // Process each reading
            foreach (var dto in meterReadingDtos)
            {
                var (isValid, errors) = await _validationService.ValidateAsync(dto);

                if (isValid)
                {
                    var meterReading = new MeterReading
                    {
                        AccountId = dto.AccountId,
                        MeterReadingDateTime = dto.MeterReadingDateTime,
                        MeterReadValue = dto.MeterReadValue
                    };

                    response.SuccessfulReadings.Add(dto);
                    validReadings.Add(meterReading);
                }
                else
                {
                    response.FailedReadings.Add(new FailedMeterReadingDto
                    {
                        MeterReading = dto,
                        Errors = errors
                    });
                }
            }

            // Save valid readings to database
            if (validReadings.Count > 0)
            {
                _meterReadingRepository.AddRange(validReadings);
                await _meterReadingRepository.SaveChangesAsync();
            }

            _logger.LogInformation("Successfully saved {Count} meter readings", validReadings.Count);

            return response;
        }
    }
}
