using FluentAssertions;
using meter_reading_uploads;
using meter_reading_uploads.Helpers;
using meter_reading_uploads.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace meter_reading_uploads_tests.UnitTests
{
    [TestClass]
    public class ValidationHelpersTests
    {
        [TestMethod]
        public void ValidHeaders_WithCorrectValues_ReturnsTrue()
        {
            var headers = new string[] { "AccountId", "MeterReadingDateTime", "MeterReadValue" };
            headers.ValidateHeaders().Should().BeTrue();
        }

        [TestMethod]
        public void ValidHeaders_WithIncorrectSize_ReturnsFalse()
        {
            var headers = new string[] { "abc", "def" };
            headers.ValidateHeaders().Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("AccountId", "MeterReadingDateTime", "")]
        [DataRow("AccountId", "", "MeterReadValue")]
        [DataRow("", "MeterReadingDateTime", "MeterReadValue")]
        [DataRow("", "", "")]
        [DataRow("abc", "def", "ghi")]
        public void ValidHeaders_WithIncorrectValues_ReturnsFalse(string header1, string header2, string header3)
        {
            var headers = new string[] { header1, header2, header3 };
            headers.ValidateHeaders().Should().BeFalse();
        }

        public void ValidateRows_ValidReading_ReturnsSuccess()
        {
            MeterReading meterReading = new MeterReading()
            {
                AccountId = "12345",
                DateTime = "22/04/2019 09:24",
                Value = "12345"
            };

            CustomerAccount customerAccount = new CustomerAccount()
            {
                AccountId = meterReading.AccountId,
                FirstName = "John",
                LastName = "Smith"
            };

            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.MeterReadingExists(meterReading).Returns(false);

            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount(meterReading.AccountId).Returns(customerAccount);

            var result = meterReading.ValidateMeterReading(readingDataStore, accountDataStore);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ValidateRows_DuplicateReading_ReturnsFailure()
        {
            MeterReading meterReading = new MeterReading()
            {
                AccountId = "12345",
                DateTime = "22/04/2019 09:24",
                Value = "12345"
            };

            CustomerAccount customerAccount = new CustomerAccount()
            {
                AccountId = meterReading.AccountId,
                FirstName = "John",
                LastName = "Smith"
            };

            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.MeterReadingExists(meterReading).Returns(true);

            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount(meterReading.AccountId).Returns(customerAccount);

            var result = meterReading.ValidateMeterReading(readingDataStore, accountDataStore);
            result.Should().Be("Cannot add the same reading twice\r\n");
        }

        [TestMethod]
        public void ValidateRows_InvalidAccountId_ReturnsFailure()
        {
            MeterReading meterReading = new MeterReading()
            {
                AccountId = "12345",
                DateTime = "22/04/2019 09:24",
                Value = "12345"
            };

            CustomerAccount customerAccount = null;

            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.MeterReadingExists(meterReading).Returns(false);

            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount(meterReading.AccountId).Returns(customerAccount);

            var result = meterReading.ValidateMeterReading(readingDataStore, accountDataStore);
            result.Should().Be("A meter reading must be associated to a valid account ID\r\n");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("     ")]
        [DataRow("-1234")]
        [DataRow("1234")]
        [DataRow("123456")]
        [DataRow("TESTY")]
        [DataRow(null)]
        public void ValidateRows_InvalidFormat_ReturnsFailure(string readingValue)
        {
            MeterReading meterReading = new MeterReading()
            {
                AccountId = "12345",
                DateTime = "22/04/2019 09:24",
                Value = readingValue
            };

            CustomerAccount customerAccount = new CustomerAccount()
            {
                AccountId = meterReading.AccountId,
                FirstName = "John",
                LastName = "Smith"
            };

            var readingDataStore = Substitute.For<IReadingDataStore>();
            readingDataStore.MeterReadingExists(meterReading).Returns(false);

            var accountDataStore = Substitute.For<IAccountDataStore>();
            accountDataStore.GetCustomerAccount(meterReading.AccountId).Returns(customerAccount);

            var result = meterReading.ValidateMeterReading(readingDataStore, accountDataStore);
            result.Should().Be("Meter reading must be in the format NNNNN\r\n");
        }
    }
}
