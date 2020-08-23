using PassportFinder.Data.Extentions;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Data.DPFPagesActs
{
    public class DPFGetOfficesAct
    {
        private readonly HttpClient _httpClient = null;
        private readonly Encoding _encoding = null;

        public DPFGetOfficesAct(HttpClient httpClient, Encoding encoding)
        {
            this._httpClient = httpClient;
            this._encoding = encoding;
        }

        public async Task<HttpResponseMessage> GetLoadOfficesPage(SessionData sessionData, string officeId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://servicos.dpf.gov.br/sinpa/realizarAgendamento.do?dispatcher=carregaPostos&dummy='${DateTime.Now.Ticks}&municipio={officeId}&exibePostoSimplificado=S");

            requestMessage.Headers.Add("Referer", "https://servicos.dpf.gov.br/sinpa/realizarAgendamentoPrincipal.do");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "cors");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-origin");
            requestMessage.Headers.Add("Sec-Fetch-User", "?1");
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36");
            requestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

            sessionData.FillRequest(requestMessage);

            var response = await this._httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
