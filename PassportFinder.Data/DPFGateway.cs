using PassportFinder.Data.Abstractions;
using PassportFinder.Data.Builders;
using PassportFinder.Data.DPFPagesActs;
using PassportFinder.Data.Extentions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Data
{
    public class DPFGateway : IDPFGateway
    {
        private readonly HttpClient _httpClient = null;
        private readonly DPFCityFinder _dpfCityFinder = null;
        private readonly DPFApoitmentAlertsFinder _dpfApoitmentAlertsFinder = null;
        private readonly DPFOfficeFinder _dpfOfficeFinder = null;
        private readonly FormActionFinder _formActionFinder = null;
        private readonly Encoding _encoding = null;

        public DPFGateway(DPFCityFinder dpfCityFinder, DPFOfficeFinder dpfOfficeFinder, DPFApoitmentAlertsFinder dpfApoitmentAlertsFinder, FormActionFinder formActionFinder, HttpClient httpClient)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this._encoding = Encoding.GetEncoding(1252);

            this._httpClient = httpClient;
            this._dpfCityFinder = dpfCityFinder;
            this._dpfOfficeFinder = dpfOfficeFinder;
            this._dpfApoitmentAlertsFinder = dpfApoitmentAlertsFinder;
            this._formActionFinder = formActionFinder;
        }

        public async Task<DPFResponse<IReadOnlyCollection<DPFCity>>> GetAvailbleCities(string cpf, string protocol, DateTime birthDate)
        {
            var dpfGetCitiesAct = new DPFGetCitiesAct(this._httpClient, this._encoding);
            var responseStartSchedulePage = await dpfGetCitiesAct.GetStartSchedulePage();
            await randomDelay();

            var sessionData = new SessionDataBuilder().Build(responseStartSchedulePage);

            var actionStartSchedulePage = this._formActionFinder.Find(await responseStartSchedulePage.Content.ReadAsStringWithEncondingAsync(this._encoding));
            var responseStartDoSchedulePage = await dpfGetCitiesAct.PostStartDoSchedulePage(sessionData, actionStartSchedulePage);
            await randomDelay();

            var responseDoSchedulePrincipalPage = await dpfGetCitiesAct.PostDoSchedulePrincipalPage(cpf, protocol, birthDate, actionStartSchedulePage, this._formActionFinder.Find(await responseStartDoSchedulePage.Content.ReadAsStringWithEncondingAsync(this._encoding)), sessionData);

            var listCities = this._dpfCityFinder.Find((await responseDoSchedulePrincipalPage.Content.ReadAsStringWithEncondingAsync(this._encoding)));

            return new DPFResponse<IReadOnlyCollection<DPFCity>>(listCities, sessionData);
        }

        public async Task<DPFResponse<IReadOnlyCollection<DPFOffice>>> GetAvailbleOffices(SessionData sessionData, string cityId)
        {
            var dpfGetOfficesAct = new DPFGetOfficesAct(this._httpClient, this._encoding);
            var responseGetLoadOfficesPage = await dpfGetOfficesAct.GetLoadOfficesPage(sessionData, cityId);

            var listOffices = this._dpfOfficeFinder.Find((await responseGetLoadOfficesPage.Content.ReadAsStringWithEncondingAsync(this._encoding)));

            return new DPFResponse<IReadOnlyCollection<DPFOffice>>(listOffices, sessionData);
        }

        public async Task<DPFResponse<IReadOnlyCollection<string>>> GetAppointmentAlertsFromOffice(SessionData sessionData, string uf, string cityId, string officeId)
        {
            var dpfCheckAvaibleDatesAct = new DPFCheckAvaibleDatesAct(this._httpClient, this._encoding);
            var responsePostDoSchedulePage = await dpfCheckAvaibleDatesAct.PostDoSchedulePage(sessionData, uf, cityId, officeId);

            var listAlerts = this._dpfApoitmentAlertsFinder.Find((await responsePostDoSchedulePage.Content.ReadAsStringWithEncondingAsync(this._encoding)));

            return new DPFResponse<IReadOnlyCollection<string>>(listAlerts, sessionData);
        }

        private static async Task randomDelay()
        {
            await Task.Delay(new Random().Next(20000, 30000));
        }
    }
}
