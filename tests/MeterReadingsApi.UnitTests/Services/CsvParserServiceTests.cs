using MeterReadingsApi.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MeterReadingsApi.UnitTests.Services
{
    public class CsvParserServiceTests
    {
        private readonly Mock<ILogger<CsvParserService>> _loggerMock;
        private readonly CsvParserService _csvParserService;

        public CsvParserServiceTests()
        {
            _loggerMock = new Mock<ILogger<CsvParserService>>();
            _csvParserService = new CsvParserService(_loggerMock.Object);
        }

        [Fact]
        public async Task ParseCsvAsync_WithValidCsv_ShouldReturnMeterReadings()
        {
            // Arrange
            var csvContent = 
                "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                "2344,22/04/2019 09:24,1002\n" +
                "2233,22/04/2019 12:25,323";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvAsync(stream);

            // Assert
            result.Should().HaveCount(2);
            result[0].AccountId.Should().Be(2344);
            result[0].MeterReadValue.Should().Be(1002);
            result[1].AccountId.Should().Be(2233);
            result[1].MeterReadValue.Should().Be(323);
        }

        [Fact]
        public async Task ParseCsvAsync_WithEmptyStream_ShouldReturnEmptyList()
        {
            // Arrange
            var csvContent = "AccountId,MeterReadingDateTime,MeterReadValue\n";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvAsync(stream);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ParseCsvAsync_WithInvalidCsv_ShouldThrowException()
        {
            // Arrange
            var csvContent = "Invalid CSV content without proper headers";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act & Assert
            await Assert.ThrowsAsync<CsvHelper.HeaderValidationException>(() => _csvParserService.ParseCsvAsync(stream));
        }
    }
}
