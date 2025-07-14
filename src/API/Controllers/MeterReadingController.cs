using MeterReadingsApi.Common.Constants;
using MeterReadingsApi.Models.Responses;
using MeterReadingsApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsApi.Controllers
{
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly ILogger<MeterReadingController> _logger;

        public MeterReadingController(
            IMeterReadingService meterReadingService,
            ILogger<MeterReadingController> logger)
        {
            _meterReadingService = meterReadingService;
            _logger = logger;
        }

        [HttpPost("meter-reading-uploads")]
        [ProducesResponseType(typeof(MeterReadingUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MeterReadingUploadResponse>> UploadMeterReadings(IFormFile file)
        {
            // Validate file
            var validationResult = ValidateFile(file);
            if (!validationResult.IsValid)
            {
                _logger.LogError("File validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                return BadRequest(new { errors = validationResult.Errors });
            }

            // Process the file
            using var stream = file.OpenReadStream();
            var result = await _meterReadingService.ProcessMeterReadingsAsync(stream);

            _logger.LogInformation("Completed processing. Successful: {Successful}, Failed: {Failed}",
                result.SuccessfulReadingsCount, result.FailedReadingsCount);

            return Ok(result);
        }

        private (bool IsValid, List<string> Errors) ValidateFile(IFormFile? file)
        {
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                errors.Add("No file provided or file is empty");
                return (false, errors);
            }

            if (file.Length > ValidationConstants.MaxFileSize)
            {
                errors.Add($"File size exceeds maximum allowed size of {ValidationConstants.MaxFileSize} bytes");
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (!string.Equals(fileExtension, ValidationConstants.AllowedFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"Only '{ValidationConstants.AllowedFileExtension}' files are allowed");
            }

            return (errors.Count == 0, errors);
        }
    }
}
