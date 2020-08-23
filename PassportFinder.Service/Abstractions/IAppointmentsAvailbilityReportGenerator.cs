using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassportFinder.Service.Abstractions
{
    public interface IAppointmentsAvailbilityReportGenerator
    {
        Task Generate(ReportInput input, IReadOnlyCollection<DPFCity> overrideDPFCities = null, string overrideUf = null, bool noDelaysExpected = false);
    }

    public class ReportInput
    {
        public string[] EmailListToNotify { get; set; }
        public string CPF { get; set; }
        public string Protocol { get; set; }
        public string UF { get; set; }
        public DateTime BirthDate { get; set; }
    }
}