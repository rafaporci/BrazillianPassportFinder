using PassportFinder.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassportFinder.Service.Abstractions
{
    public interface IAppointmentsAvailbilityReportGenerator
    {
        Task Generate(IReadOnlyCollection<DPFCity> overrideDPFCities = null, string overrideUf = null, bool noDelaysExpected = false);
    }
}