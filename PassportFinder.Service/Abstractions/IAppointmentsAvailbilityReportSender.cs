using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassportFinder.Service.Abstractions
{
    public interface IAppointmentsAvailbilityReportSender
    {
        bool Send(Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult);
    }
}