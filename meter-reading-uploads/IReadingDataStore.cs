using meter_reading_uploads.Models;

namespace meter_reading_uploads
{
    public interface IReadingDataStore
    {
        public bool AddMeterReading(MeterReading meterReading);

        bool MeterReadingExists(MeterReading meterReading);
    }
}
