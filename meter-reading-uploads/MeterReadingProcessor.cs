using meter_reading_uploads.Helpers;
using meter_reading_uploads.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace meter_reading_uploads
{
    public class MeterReadingProcessor : IMeterReadingProcessor
    {
        private ILogger<MeterReadingProcessor> _logger;

        private IReadingDataStore _readingDataStore;

        private IAccountDataStore _accountDataStore;

        public MeterReadingProcessor(
            ILogger<MeterReadingProcessor> logger,
            IReadingDataStore readingDataStore,
            IAccountDataStore accountDataStore)
        {
            _logger = logger;
            _readingDataStore = readingDataStore;
            _accountDataStore = accountDataStore;
        }

        public MeterReadingsFileResults ProcessFile(StreamReader reader)
        {
            string[] headers = reader.ReadLine().Split(',');

            if (!headers.ValidateHeaders())
            {
                _logger.LogWarning("Invalid file headers");

                throw new InvalidHeadersException();
            }

            var results = new MeterReadingsFileResults
            {
                Successful = 0,
                Failed = 0
            };

            while (!reader.EndOfStream)
            {
                string[] values = reader.ReadLine().Split(',');

                if (values.Count() != 3)
                {
                    results.Failed++;

                    _logger.LogWarning("Invalid row size");

                    continue;
                }

                var meterReading = new MeterReading
                {
                    AccountId = values[0].ToString(),
                    DateTime = values[1].ToString(),
                    Value = values[2].ToString()
                };

                var validationResult = meterReading.ValidateMeterReading(_readingDataStore, _accountDataStore);

                if (string.IsNullOrEmpty(validationResult))
                {
                    if (_readingDataStore.AddMeterReading(meterReading))
                    {
                        results.Successful++;

                        _logger.LogInformation("Meter reading read successfully");

                        continue;
                    }
                }

                results.Failed++;
                
                _logger.LogWarning(validationResult);
            }

            return results;
        }
    }
}
