using meter_reading_uploads.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace meter_reading_uploads
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IAccountDataStore, JsonAccountDataStore>();
            services.AddSingleton<IReadingDataStore, JsonReadingDataStore>();
            services.AddScoped<IMeterReadingProcessor, MeterReadingProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAccountDataStore accountData)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PopulateAccountData(accountData);
        }

        private void PopulateAccountData(IAccountDataStore accountData)
        {
            var filePath = Path.Combine("Data", "Test_Accounts.csv");

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                string[] headers = reader.ReadLine().Split(',');

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    CustomerAccount customerAccount = new CustomerAccount
                    {
                        AccountId = values[0].ToString(),
                        FirstName = values[1].ToString(),
                        LastName = values[2].ToString()
                    };

                    accountData.AddCustomerAccount(customerAccount);
                }
            }
        }
    }
}
