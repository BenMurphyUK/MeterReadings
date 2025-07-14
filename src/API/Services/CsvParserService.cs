using CsvHelper;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Services.Interfaces;
using System.Globalization;

namespace MeterReadingsApi.Services
{
    public class CsvParserService : ICsvParserService
    {
        private readonly ILogger<CsvParserService> _logger;

        public CsvParserService(ILogger<CsvParserService> logger)
        {
            _logger = logger;
        }

        public async Task<List<MeterReadingDto>> ParseCsvAsync(Stream csvStream)
        {
            var meterReadings = new List<MeterReadingDto>();

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<MeterReadingDto.CsvMap>();

            await foreach (var record in csv.GetRecordsAsync<MeterReadingDto>())
            {
                meterReadings.Add(record);
            }

            return meterReadings;
        }
    }
}
