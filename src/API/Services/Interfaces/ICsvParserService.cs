using MeterReadingsApi.Models.Dtos;

namespace MeterReadingsApi.Services.Interfaces
{
    public interface ICsvParserService
    {
        Task<List<MeterReadingDto>> ParseCsvAsync(Stream csvStream);
    }
}
