using meter_reading_uploads.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace meter_reading_uploads
{
    public class JsonReadingDataStore : IReadingDataStore
    {
        private JsonSerializerOptions _jsonSerializerOptions;

        private IList<MeterReading> _meterReadings;

        private string _filePath;

        public JsonReadingDataStore()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            _filePath = Path.Combine("Data", "MeterReadings.json");

            _meterReadings = LoadMeterReadings();
        }

        public bool AddMeterReading(MeterReading meterReading)
        {
            if (MeterReadingExists(meterReading))
            {
                return false;
            }

            _meterReadings.Add(meterReading);

            SaveMeterReadings(_meterReadings);

            return true;
        }

        public bool MeterReadingExists(MeterReading meterReading)
        {
            return _meterReadings.Any(r => r.AccountId.Equals(meterReading.AccountId) && r.DateTime.Equals(meterReading.DateTime) && r.Value.Equals(meterReading.Value));
        }

        private IList<MeterReading> LoadMeterReadings()
        {
            if (!File.Exists(_filePath))
            {
                return new List<MeterReading>();
            }

            var jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<IList<MeterReading>>(jsonString, _jsonSerializerOptions);
        }

        private void SaveMeterReadings(IEnumerable<MeterReading> meterReadings)
        {
            var jsonString = JsonSerializer.Serialize(meterReadings, _jsonSerializerOptions);
            File.WriteAllText(_filePath, jsonString);
        }
    }
}
