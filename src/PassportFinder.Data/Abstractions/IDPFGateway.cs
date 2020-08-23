using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassportFinder.Data.Abstractions
{
    public interface IDPFGateway
    {
        Task<DPFResponse<IReadOnlyCollection<string>>> GetAppointmentAlertsFromOffice(SessionData sessionData, string uf, string cityId, string officeId);
        Task<DPFResponse<IReadOnlyCollection<DPFCity>>> GetAvailbleCities(string cpf, string protocol, DateTime birthDate);
        Task<DPFResponse<IReadOnlyCollection<DPFOffice>>> GetAvailbleOffices(SessionData sessionData, string cityId);
    }
}