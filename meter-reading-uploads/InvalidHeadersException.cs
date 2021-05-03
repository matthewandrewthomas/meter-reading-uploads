using System;
using System.Runtime.Serialization;

namespace meter_reading_uploads
{
    [Serializable]
    public class InvalidHeadersException : Exception
    {
        public InvalidHeadersException()
        {
        }

        public InvalidHeadersException(string message) : base(message)
        {
        }

        public InvalidHeadersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidHeadersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}