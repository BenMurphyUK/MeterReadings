using FluentAssertions;
using MeterReadingsApi.Data.Entities;
using MeterReadingsApi.Data.Repositories.Interfaces;
using MeterReadingsApi.Models.Dtos;
using MeterReadingsApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeterReadingsApi.UnitTests.Services
{
    public class ValidationServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;
        private readonly Mock<ILogger<ValidationService>> _loggerMock;
        private readonly ValidationService _validationService;

        public ValidationServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();
            _loggerMock = new Mock<ILogger<ValidationService>>();

            _validationService = new ValidationService(
                _accountRepositoryMock.Object,
                _meterReadingRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Theory]
        [InlineData(12345, true)]
        [InlineData(00001, true)]
        [InlineData(99999, true)]
        [InlineData(-1, false)]     // Negative
        [InlineData(123456, false)] // Out of range
        public void IsReadingValueValid_ShouldReturnExpectedResult(int value, bool expected)
        {
            // Act
            var result = _validationService.IsReadingValueValid(value);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task ValidateAsync_WhenAccountDoesNotExist_ShouldReturnValidationError()
        {
            // Arrange
            var meterReading = new MeterReadingDto
            {
                AccountId = 9999,
                MeterReadingDateTime = DateTime.Now,
                MeterReadValue = 12345
            };

            _accountRepositoryMock.Setup(x => x.ExistsAsync(9999))
                .ReturnsAsync(false);

            // Act
            var (isValid, errors) = await _validationService.ValidateAsync(meterReading);

            // Assert
            isValid.Should().BeFalse();
            errors.Should().Contain("An account with the ID 9999 does not exist");
        }

        [Fact]
        public async Task ValidateAsync_WhenReadingIsDuplicate_ShouldReturnValidationError()
        {
            // Arrange
            var meterReading = new MeterReadingDto
            {
                AccountId = 1234,
                MeterReadingDateTime = new DateTime(2023, 1, 1),
                MeterReadValue = 12345
            };

            _accountRepositoryMock.Setup(x => x.ExistsAsync(1234))
                .ReturnsAsync(true);

            _meterReadingRepositoryMock.Setup(x => x.ExistsAsync(1234, It.IsAny<DateTime>(), 12345))
                .ReturnsAsync(true);

            // Act
            var (isValid, errors) = await _validationService.ValidateAsync(meterReading);

            // Assert
            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.Contains("Duplicate reading"));
        }

        [Fact]
        public async Task ValidateAsync_WhenReadingIsOlderThanExisting_ShouldReturnValidationError()
        {
            // Arrange
            var existingReading = new MeterReading
            {
                AccountId = 1234,
                MeterReadingDateTime = new DateTime(2023, 2, 1),
                MeterReadValue = 11111
            };

            var newReading = new MeterReadingDto
            {
                AccountId = 1234,
                MeterReadingDateTime = new DateTime(2023, 1, 1), // Older than existing
                MeterReadValue = 12345
            };

            _accountRepositoryMock.Setup(x => x.ExistsAsync(1234))
                .ReturnsAsync(true);

            _meterReadingRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(false); // Pass the duplicate reading check

            _meterReadingRepositoryMock.Setup(x => x.GetLatestByAccountAsync(1234))
                .ReturnsAsync(existingReading);

            // Act
            var (isValid, errors) = await _validationService.ValidateAsync(newReading);

            // Assert
            isValid.Should().BeFalse();
            errors.Should().Contain(e => e.Contains("older than existing readings"));
        }

        [Fact]
        public async Task ValidateAsync_WhenAllValidationsPass_ShouldReturnValid()
        {
            // Arrange
            var meterReading = new MeterReadingDto
            {
                AccountId = 1234,
                MeterReadingDateTime = new DateTime(2023, 2, 1),
                MeterReadValue = 12345
            };

            _accountRepositoryMock.Setup(x => x.ExistsAsync(1234))
                .ReturnsAsync(true); // Account exists

            _meterReadingRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(false); // No duplicate reading

            _meterReadingRepositoryMock.Setup(x => x.GetLatestByAccountAsync(1234))
                .ReturnsAsync((MeterReading?)null); // No existing readings

            // Act
            var (isValid, errors) = await _validationService.ValidateAsync(meterReading);

            // Assert
            isValid.Should().BeTrue();
            errors.Should().BeEmpty();
        }
    }
}
