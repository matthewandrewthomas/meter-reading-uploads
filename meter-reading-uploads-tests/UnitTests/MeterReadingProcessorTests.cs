using FluentAssertions;
using meter_reading_uploads;
using meter_reading_uploads.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace meter_reading_uploads_tests.UnitTests
{
    [TestClass]
    public class MeterReadingProcessorTests
    {
        [TestMethod]
        public void MeterReadingProcessor_MissingHeaders_ThrowsException()
        {
            var logger = Substitute.For<ILogger<MeterReadingProcessor>>();
            var readingDataStore = Substitute.For<IReadingDataStore>();
            var accountDataStore = Substitute.For<IAccountDataStore>();
            var processor = new MeterReadingProcessor(logger, readingDataStore, accountDataStore);
            var filePath = "MeterReadings_MissingHeaders.csv";

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                Assert.ThrowsException<InvalidHeadersException>(() => processor.ProcessFile(reader));
            }
        }

        [TestMethod]
        public void MeterReadingProcessor_ValidFile_ProcessedSuccessfully()
        {
            var customerAccount = new CustomerAccount
            {
                AccountId = "2344",
                FirstName = "John",
                LastName = "Smith"
            };

            var logger = Substitute.For<ILogger<MeterReadingProcessor>>();
            
            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.AddMeterReading(Arg.Any<MeterReading>()).Returns(true);
            
            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount("2344").Returns(customerAccount);

            var processor = new MeterReadingProcessor(logger, readingDataStore, accountDataStore);
            var filePath = "MeterReadings_OneFailOneSuccess.csv";

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                var results = processor.ProcessFile(reader);

                results.Successful.Should().Be(1);
                results.Failed.Should().Be(1);
            }
        }

        [TestMethod]
        public void MeterReadingProcessor_UnableToSave_HandlesError()
        {
            var customerAccount = new CustomerAccount
            {
                AccountId = "2344",
                FirstName = "John",
                LastName = "Smith"
            };

            var logger = Substitute.For<ILogger<MeterReadingProcessor>>();

            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.AddMeterReading(Arg.Any<MeterReading>()).Returns(false);

            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount("2344").Returns(customerAccount);

            var processor = new MeterReadingProcessor(logger, readingDataStore, accountDataStore);
            var filePath = "MeterReadings_OneFailOneSuccess.csv";

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                var results = processor.ProcessFile(reader);

                results.Successful.Should().Be(0);
                results.Failed.Should().Be(2);
            }
        }
    }
}
