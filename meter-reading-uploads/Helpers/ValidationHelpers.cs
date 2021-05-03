using meter_reading_uploads.Models;
using System.Linq;
using System.Text;

namespace meter_reading_uploads.Helpers
{
    public static class ValidationHelpers
    {
        public static bool ValidateHeaders(this string[] headers)
        {
            return headers.Count() == 3
                && headers[0] == "AccountId"
                && headers[1] == "MeterReadingDateTime"
                && headers[2] == "MeterReadValue";
        }

        public static string ValidateMeterReading(this MeterReading meterReading, IReadingDataStore readingDataStore, IAccountDataStore accountDataStore)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (readingDataStore.MeterReadingExists(meterReading))
            {
                stringBuilder.AppendLine("Cannot add the same reading twice");
            }

            if (accountDataStore.GetCustomerAccount(meterReading.AccountId) == null)
            {
                stringBuilder.AppendLine("A meter reading must be associated to a valid account ID");
            }

            var isIntValue = int.TryParse(meterReading.Value, out var intValue);

            if (!isIntValue || intValue < 0 || meterReading.Value.Length != 5)
            {
                stringBuilder.AppendLine("Meter reading must be in the format NNNNN");
            }

            return stringBuilder.ToString();
        }
    }
}
