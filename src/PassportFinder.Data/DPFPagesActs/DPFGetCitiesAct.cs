using PassportFinder.Data.Extentions;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Data.DPFPagesActs
{
    public class DPFGetCitiesAct
    {
        private readonly HttpClient _httpClient = null;
        private readonly Encoding _encoding = null;

        public DPFGetCitiesAct(HttpClient httpClient, Encoding encoding)
        {
            this._httpClient = httpClient;
            this._encoding = encoding;
        }

        public async Task<HttpResponseMessage> GetStartSchedulePage()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://servicos.dpf.gov.br/sinpa/paginaInicialAgendamento.do?fbclid=IwAR3dXg_he4ocQXMQIcXcpf2p6XzHQpxkWp6D5GXaeG3JUG_vryJpHvBvn54");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "document");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-origin");
            requestMessage.Headers.Add("Sec-Fetch-User", "?1");
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36");
            requestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

            var response = await this._httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostStartDoSchedulePage(SessionData sessionData, string action)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://servicos.dpf.gov.br{action}")
            {
                Content = new StringContent("dispatcher=exibirSolicitacaoAgendamento&validate=false&postoId=&qtdAgendamentos=1", this._encoding, "application/x-www-form-urlencoded"),
            };

            requestMessage.Headers.Add("Referer", "https://servicos.dpf.gov.br/sinpa/paginaInicialAgendamento.do?fbclid=IwAR3dXg_he4ocQXMQIcXcpf2p6XzHQpxkWp6D5GXaeG3JUG_vryJpHvBvn54");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "document");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-origin");
            requestMessage.Headers.Add("Sec-Fetch-User", "?1");
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36");
            requestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

            sessionData.FillRequest(requestMessage);

            var response = await this._httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostDoSchedulePrincipalPage(string cpf, string protocol, DateTime birthDate, string referer, string action, SessionData sessionData)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://servicos.dpf.gov.br{action}")
            {
                Content = new StringContent($"dispatcher=processarConsultaAgendamento&validate=true&origem=exibirSolicitacaoAgendamento&operacao=agendar&cpf={cpf}&protocolo={protocol}&dataNascimento={birthDate.ToString("dd-MM-yyyy").Replace("-", "%2F")}&email1=&email2=&url=", this._encoding, "application/x-www-form-urlencoded"),
            };

            requestMessage.Headers.Add("Referer", referer);
            requestMessage.Headers.Add("Sec-Fetch-Dest", "document");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
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
