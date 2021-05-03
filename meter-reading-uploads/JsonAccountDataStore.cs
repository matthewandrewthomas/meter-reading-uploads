using meter_reading_uploads.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace meter_reading_uploads
{
    public class JsonAccountDataStore : IAccountDataStore
    {
        private JsonSerializerOptions _jsonSerializerOptions;

        private string _filePath;

        private IList<CustomerAccount> _customerAccounts;
        
        public JsonAccountDataStore()
        {
            _jsonSerializerOptions  = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            _filePath = Path.Combine("Data", "CustomerAccounts.json");

            _customerAccounts = LoadCustomerAccounts().ToList();
        }

        public bool AddCustomerAccount(CustomerAccount customerAccount)
        {
            if (_customerAccounts.Any(a => a.AccountId.Equals(customerAccount.AccountId)))
            {
                return false;
            }

            _customerAccounts.Add(customerAccount);

            SaveCustomerAccounts(_customerAccounts);

            return true;
        }

        public CustomerAccount GetCustomerAccount(string customerAccountId)
        {
            return _customerAccounts.FirstOrDefault(a => a.AccountId == customerAccountId);
        }

        private IEnumerable<CustomerAccount> LoadCustomerAccounts()
        {
            if (!File.Exists(_filePath))
            {
                return new List<CustomerAccount>();
            }

            var jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<IEnumerable<CustomerAccount>>(jsonString, _jsonSerializerOptions);
        }

        private void SaveCustomerAccounts(IEnumerable<CustomerAccount> customerAccounts)
        {
            var jsonString = JsonSerializer.Serialize(customerAccounts, _jsonSerializerOptions);
            File.WriteAllText(_filePath, jsonString);
        }
    }
}
