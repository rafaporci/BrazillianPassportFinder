using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using PassportFinder.Data.Extentions;
using PassportFinder.Service.Abstractions;
using PassportFinder.Service.Extentions;
using System;

namespace PassportFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var input = new ReportInput();

            input.EmailListToNotify = new string[1] { "myemail@email.com" };
            input.CPF = "000.000.000-00"; // provide here a CPF
            input.Protocol = "000000000000000000000"; // provide here a protocol
            input.BirthDate = new DateTime(); // provide here a birth date
            input.UF = "SP";

            try
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory()) 
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();

                var servicesProvider = BuildDi(config);
                using (servicesProvider as IDisposable)
                {   
                    servicesProvider.GetRequiredService<IAppointmentsAvailbilityReportGenerator>().Generate(input).Wait();
                }
            }
            catch (Exception ex)
            {                
                logger.Error(ex, "Stopped program because an exception occured");
                throw;
            }
            finally
            {                
                LogManager.Shutdown();
            }
        }

        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
               .AddDataServicesDependencyInjection()     
               .AddServicesDependencyInjection()
               .AddLogging(loggingBuilder =>
               {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
               })
               .BuildServiceProvider();
        }
    }
}
