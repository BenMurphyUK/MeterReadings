using FluentAssertions;
using MeterReadingsApi.Data.Entities;
using MeterReadingsApi.Data.Repositories.Interfaces;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Services;
using MeterReadingsApi.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace MeterReadingsApi.UnitTests.Services
{
    public class MeterReadingServiceTests
    {
        private readonly Mock<ICsvParserService> _csvParserServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;
        private readonly Mock<ILogger<MeterReadingService>> _loggerMock;
        private readonly MeterReadingService _meterReadingService;

        public MeterReadingServiceTests()
        {
            _csvParserServiceMock = new Mock<ICsvParserService>();
            _validationServiceMock = new Mock<IValidationService>();
            _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();
            _loggerMock = new Mock<ILogger<MeterReadingService>>();

            _meterReadingService = new MeterReadingService(
                _csvParserServiceMock.Object,
                _validationServiceMock.Object,
                _meterReadingRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessMeterReadingsAsync_WithValidReadings_ShouldReturnSuccessfulResult()
        {
            // Arrange
            var meterReadings = new List<MeterReadingDto>
            {
                new() { AccountId = 2344, MeterReadingDateTime = DateTime.Now, MeterReadValue = 12345 },
                new() { AccountId = 2233, MeterReadingDateTime = DateTime.Now, MeterReadValue = 67890 }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test csv"));

            _csvParserServiceMock.Setup(x => x.ParseCsvAsync(It.IsAny<Stream>()))
                .ReturnsAsync(meterReadings);

            _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<MeterReadingDto>()))
                .ReturnsAsync((true, new List<string>()));

            _meterReadingRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(2);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(stream);

            // Assert
            result.SuccessfulReadingsCount.Should().Be(2);
            result.FailedReadingsCount.Should().Be(0);

            _meterReadingRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<MeterReading>>()), Times.Once);
            _meterReadingRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessMeterReadingsAsync_WithInvalidReadings_ShouldReturnFailedResult()
        {
            // Arrange
            var meterReadings = new List<MeterReadingDto>
            {
                new() { AccountId = 9999, MeterReadingDateTime = DateTime.Now, MeterReadValue = 12345 },
                new() { AccountId = 2233, MeterReadingDateTime = DateTime.Now, MeterReadValue = 67890 }
            };

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test csv"));

            _csvParserServiceMock.Setup(x => x.ParseCsvAsync(It.IsAny<Stream>()))
                .ReturnsAsync(meterReadings);

            // First reading is invalid
            _validationServiceMock.SetupSequence(x => x.ValidateAsync(It.IsAny<MeterReadingDto>()))
                .ReturnsAsync((false, new List<string> { "Account does not exist" }))
                .ReturnsAsync((true, new List<string>()));

            _meterReadingRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(stream);

            // Assert
            result.SuccessfulReadingsCount.Should().Be(1);
            result.FailedReadingsCount.Should().Be(1);
            result.FailedReadings[0].Errors.Should().Contain("Account does not exist");
        }
    }
}
