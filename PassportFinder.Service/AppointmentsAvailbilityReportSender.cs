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
    public class AppointmentsAvailbilityReportSender : IAppointmentsAvailbilityReportSender
    {        
        private readonly IEmailNotifier _emailNotifier = null;
        private readonly ILogger<AppointmentsAvailbilityReportSender> _logger;

        public AppointmentsAvailbilityReportSender(IEmailNotifier emailNotifier, ILogger<AppointmentsAvailbilityReportSender> logger)
        {            
            this._emailNotifier = emailNotifier;
            this._logger = logger;
        }

        public bool Send(Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult)
        {
            return sendEmail(dictionaryResult);
        }
        private bool sendEmail(Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult)
        {
            return this._emailNotifier.Send(new string[2] { "ac.jaen@gmail.com", "rafaporci@gmail.com" }, $"DPF Passaporte - Relatório - {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", generateInfoReport(dictionaryResult));
        }

        private string generateInfoReport(Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<h2>DPF Passaporte - Relatório - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}</h2>");

            stringBuilder.Append($"<h3>Destaques</h3>");
            int lengthBefore = stringBuilder.Length;
            foreach (var dictCity in dictionaryResult)
            {
                foreach (var dictOffice in dictCity.Value.Where(a => !a.Item1.IsAppointmentMandatory && !a.Item1.HaveAlerts))
                    stringBuilder.Append($"<li>{dictCity.Key.Name}-{dictOffice.Item1.Name} <font color='blue'>não necessita de agendamento e está sem alertas</font></li>");

                foreach (var dictOffice in dictCity.Value.Where(a => a.Item1.IsAppointmentMandatory && !a.Item1.HaveAlerts && a.Item2.Count == 0))
                    stringBuilder.Append($"<li>{dictCity.Key.Name}-{dictOffice.Item1.Name} <font color='blue'>necessita de agendamento e está sem alertas</font></li>");
            }
            if (stringBuilder.Length == lengthBefore)
                stringBuilder.Append($"<li><i>Não há destaques no relatório de hoje</i></li>");

            stringBuilder.Append($"<h3>Detalhes</h3>");
            foreach (var dictCity in dictionaryResult)
            {
                foreach (var dictOffice in dictCity.Value)
                    stringBuilder.Append($"<li>{dictCity.Key.Name}-{dictOffice.Item1.Name} {(!dictOffice.Item1.IsAppointmentMandatory ? "não" : String.Empty)} necessita de agendamento {(dictOffice.Item1.HaveAlerts ? "<font color='red'>" + dictOffice.Item1.Alerts + "</font>" : String.Empty)} {(dictOffice.Item2?.Count > 0 ? "<font color='red'>" + dictOffice.Item2.FirstOrDefault() + "</font>" : String.Empty)}</li>");
            }

            return stringBuilder.ToString();
        }

    }
}
