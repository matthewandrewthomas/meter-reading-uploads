using meter_reading_uploads.Models;
using System.IO;

namespace meter_reading_uploads
{
    public interface IMeterReadingProcessor
    {
        public MeterReadingsFileResults ProcessFile(StreamReader reader);
    }
}
