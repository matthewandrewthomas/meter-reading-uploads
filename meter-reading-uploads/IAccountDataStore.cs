using meter_reading_uploads.Models;

namespace meter_reading_uploads
{
    public interface IAccountDataStore
    {
        public bool AddCustomerAccount(CustomerAccount customerAccount);

        public CustomerAccount GetCustomerAccount(string customerAccountId);
    }
}
