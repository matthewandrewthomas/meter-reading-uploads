using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using meter_reading_uploads.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace meter_reading_uploads.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    public class MeterReadingsUploadsController : ControllerBase
    {
        private readonly ILogger<MeterReadingsUploadsController> _logger;

        private IMeterReadingProcessor _meterReadingProcessor;

        public MeterReadingsUploadsController(
            ILogger<MeterReadingsUploadsController> logger,
            IMeterReadingProcessor meterReadingProcessor)
        {
            _logger = logger;
            _meterReadingProcessor = meterReadingProcessor;
        }

        [HttpPost]
        public IActionResult Post(IFormFile file)
        {
            if (file == null || !file.FileName.EndsWith(".csv"))
            {
                _logger.LogWarning("Missing file, or uploaded file is not a CSV file");

                return BadRequest();
            }

            MeterReadingsFileResults results;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                try
                {
                    results = _meterReadingProcessor.ProcessFile(reader);
                }
                catch (InvalidHeadersException)
                {
                    return BadRequest();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    return StatusCode(500);
                }
            }

            return Ok(results);
        }
    }
}
