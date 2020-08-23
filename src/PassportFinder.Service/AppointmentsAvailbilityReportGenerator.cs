using Microsoft.Extensions.Logging;
using NLog;
using PassportFinder.Data.Abstractions;
using PassportFinder.Model;
using PassportFinder.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Service
{
    public class AppointmentsAvailbilityReportGenerator : IAppointmentsAvailbilityReportGenerator
    {
        private readonly IDPFGateway _dpfGateway = null;
        private readonly IAppointmentsAvailbilityReportSender _appointmentsAvailbilityReportSender = null;
        private readonly ILogger<AppointmentsAvailbilityReportGenerator> _logger;        

        public AppointmentsAvailbilityReportGenerator(IDPFGateway dpfGateway, IAppointmentsAvailbilityReportSender appointmentsAvailbilityReportSender, ILogger<AppointmentsAvailbilityReportGenerator> logger)
        {
            this._dpfGateway = dpfGateway;
            this._appointmentsAvailbilityReportSender = appointmentsAvailbilityReportSender;
            this._logger = logger;
        }

        public async Task Generate(ReportInput input, IReadOnlyCollection<DPFCity> overrideDPFCities = null, string overrideUf = null, bool noDelaysExpected = false)
        {
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult = new Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>>();

            logInfo($"Starting report generation");

            var cities = await this._dpfGateway.GetAvailbleCities(input.CPF, input.Protocol, input.BirthDate);
            if (overrideDPFCities?.Count > 0)
                cities.Data = overrideDPFCities;
            if (!String.IsNullOrEmpty(overrideUf))
                input.UF = overrideUf;

            logInfo($"{cities.Data.Count} cities found");

            foreach (var city in cities.Data.OrderBy(a => a.Id))
            {
                await randomDelay(noDelaysExpected);
                logInfo($"Checking city {city.Id}-{city.Name}");

                var offices = await this._dpfGateway.GetAvailbleOffices(cities.SessionData, city.Id);
                logInfo($"{offices.Data.Count} offices found");

                foreach (var office in offices.Data)
                {                    
                    logInfo($"Checking office {office.Id}-{office.Name}");

                    if (office.IsAppointmentMandatory)
                        logInfo($"Office with appointment mandatory.");

                    IReadOnlyCollection<string> appointmentAlerts = null;

                    if (office.HaveAlerts)
                        logInfo($"Office with alerts '{office.Alerts}'.");
                    else if (office.IsAppointmentMandatory)
                    {
                        await randomDelay(noDelaysExpected);
                        appointmentAlerts = (await this._dpfGateway.GetAppointmentAlertsFromOffice(cities.SessionData, input.UF, city.Id, office.Id)).Data;
                        if (appointmentAlerts.Count > 0)
                            logInfo($"Office with appoitment alerts '{appointmentAlerts.FirstOrDefault()}'.");
                    }

                    registerReport(dictionaryResult, city, office, appointmentAlerts);

                    logInfo($"Office {office.Id}-{office.Name} checked");
                }

                logInfo($"City {city.Id}-{city.Name} checked");
            }

            var emailSent = this._appointmentsAvailbilityReportSender.Send(input.EmailListToNotify, dictionaryResult); 

            logInfo($"Report generation finished, email sent={emailSent}");
        }

        private void registerReport(Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult, DPFCity city, DPFOffice office, IReadOnlyCollection<string> appointmentAlerts)
        {
            var value = new Tuple<DPFOffice, IReadOnlyCollection<string>>(office, appointmentAlerts);
            if (dictionaryResult.ContainsKey(city))
                dictionaryResult[city].Add(value);
            else
                dictionaryResult.Add(city, new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>() { value });
        }

        private void logInfo(string info)
        {
            this._logger.LogInformation($"{info}");
        }

        private async Task randomDelay(bool noDelaysExpected)
        {
            if (!noDelaysExpected)
                await Task.Delay(new Random().Next(20000, 30000));
        }
    }
}
