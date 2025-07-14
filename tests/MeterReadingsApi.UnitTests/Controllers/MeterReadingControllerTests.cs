using MeterReadingsApi.Controllers;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Models.Responses;
using MeterReadingsApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace MeterReadingsApi.UnitTests.Controllers
{
    public class MeterReadingControllerTests
    {
        private readonly Mock<IMeterReadingService> _meterReadingServiceMock;
        private readonly Mock<ILogger<MeterReadingController>> _loggerMock;
        private readonly MeterReadingController _controller;

        public MeterReadingControllerTests()
        {
            _meterReadingServiceMock = new Mock<IMeterReadingService>();
            _loggerMock = new Mock<ILogger<MeterReadingController>>();
            _controller = new MeterReadingController(_meterReadingServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UploadMeterReadings_WithValidFile_ShouldReturnOkResult()
        {
            // Arrange
            var csvContent = 
                "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                "2344,22/04/2019 09:24,01002";

            var file = CreateMockFile("test.csv", csvContent);

            var expectedResponse = new MeterReadingUploadResponse
            {
                SuccessfulReadings = new List<MeterReadingDto>()
                {
                    new MeterReadingDto
                    {
                        AccountId = 2344,
                        MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                        MeterReadValue = 1002
                    }
                },
            };

            _meterReadingServiceMock.Setup(x => x.ProcessMeterReadingsAsync(It.IsAny<Stream>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UploadMeterReadings(file);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<MeterReadingUploadResponse>().Subject;
            response.SuccessfulReadingsCount.Should().Be(1);
            response.FailedReadingsCount.Should().Be(0);
        }

        [Fact]
        public async Task UploadMeterReadings_WithNullFile_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.UploadMeterReadings(null!);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadMeterReadings_WithWrongFileExtension_ShouldReturnBadRequest()
        {
            // Arrange
            var file = CreateMockFile("test.txt", "some content");

            // Act
            var result = await _controller.UploadMeterReadings(file);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadMeterReadings_WithEmptyFile_ShouldReturnBadRequest()
        {
            // Arrange
            var file = CreateMockFile("test.csv", "");

            // Act
            var result = await _controller.UploadMeterReadings(file);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        private static IFormFile CreateMockFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var file = new Mock<IFormFile>();

            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.Length).Returns(bytes.Length);
            file.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(bytes));

            return file.Object;
        }
    }
}
