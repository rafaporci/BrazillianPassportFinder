using FluentAssertions;
using PassportFinder.Data;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PassportFinder.Component.Tests
{
    public class DPFGatewayTest
    {
        private readonly DPFGateway dpfGateway = null;
        public DPFGatewayTest()
        {
            this.dpfGateway = new DPFGateway(   new Data.HtmlFinders.DPFCityFinder(), 
                                                new Data.HtmlFinders.DPFOfficeFinder(), 
                                                new Data.HtmlFinders.DPFApoitmentAlertsFinder(), 
                                                new Data.HtmlFinders.FormActionFinder(), 
                                                new HttpClient());
        }

        [Fact]
        public async Task HaveValidCPFAndProtocol_ShouldHaveCitiesAndOfficesAndAlertsListed()
        {
            // Arrange
            var cpf = ""; // provide here a valid CPF
            var protocol = ""; // provide here a valid protocol
            var birthDate = new DateTime(1986, 6, 6);

            var expectedCities = new List<DPFCity>() {
                new DPFCity() { Id = "7107", Name = "SÃO PAULO" },
                new DPFCity() { Id = "7115", Name = "SÃO SEBASTIÃO" },
                new DPFCity() { Id = "7145", Name = "SOROCABA" }
            };

            var expectedOffices = new List<DPFOffice>() {
                new DPFOffice() { Id = "207", Name = "DELEGACIA ESPECIAL DE POLICIA FEDERAL NO AEROPORTO INTERNACIONAL DE SÃO PAULO/GUARULHOS - DEAIN/SR/PF/SP - DEAIN/SR/PF/SP - CUMBICA - GUARULHOS - SP", IsAppointmentMandatory = true },
            };

            var expectedAlerts = new List<string>() {
                "Nenhum horário disponível. Consulte outro posto."
            };

            // Act
            var resultCities = await this.dpfGateway.GetAvailbleCities(cpf, protocol, birthDate);            
            await randomDelay();
            var resultOffices = await this.dpfGateway.GetAvailbleOffices(resultCities.SessionData, "6477");
            await randomDelay();
            var resultAlerts = await this.dpfGateway.GetAppointmentAlertsFromOffice(resultCities.SessionData, "SP", "6477", "207");            

            // Assert
            (from d in resultCities.Data
             join c in expectedCities on d.Id equals c.Id
             select d).ToList().Should().BeEquivalentTo(expectedCities);

            (from d in resultOffices.Data
             join c in expectedOffices on d.Id equals c.Id
             select d).ToList().Should().BeEquivalentTo(expectedOffices);

            resultAlerts.Data.Should().BeEquivalentTo(expectedAlerts);
        }

        private static async Task randomDelay()
        {
            await Task.Delay(new Random().Next(20000, 30000));
        }
    }
}
