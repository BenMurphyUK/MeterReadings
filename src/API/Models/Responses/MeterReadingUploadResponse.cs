using MeterReadingsApi.Models.Dtos;

namespace MeterReadingsApi.Models.Responses
{
    public class MeterReadingUploadResponse
    {
        public int SuccessfulReadingsCount => SuccessfulReadings.Count;
        public List<MeterReadingDto> SuccessfulReadings { get; set; } = new();
        public int FailedReadingsCount => FailedReadings.Count;
        public List<FailedMeterReadingDto> FailedReadings { get; set; } = new();
    }

    public class FailedMeterReadingDto
    {
        public MeterReadingDto MeterReading { get; set; } = null!;
        public List<string> Errors { get; set; } = new();
    }
}
