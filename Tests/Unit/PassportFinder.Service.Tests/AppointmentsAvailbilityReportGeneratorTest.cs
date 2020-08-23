using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PassportFinder.Data.Abstractions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Model;
using PassportFinder.Service;
using PassportFinder.Service.Abstractions;
using PassportFinder.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace PassportFinder.Data.Tests.HtmlFinders
{
    public class AppointmentsAvailbilityReportGeneratorTest
    {
        private readonly AppointmentsAvailbilityReportGenerator appointmentsAvailbilityReportGenerator = null;
        private readonly Mock<ILogger<AppointmentsAvailbilityReportGenerator>> loggerMock = null;
        private readonly Mock<IDPFGateway> dpfGatewayMock = null;
        private readonly Mock<IAppointmentsAvailbilityReportSender> appointmentsAvailbilityReportSenderMock = null;

        public AppointmentsAvailbilityReportGeneratorTest()
        {
            this.loggerMock = new Mock<ILogger<AppointmentsAvailbilityReportGenerator>>();
            this.dpfGatewayMock = new Mock<IDPFGateway>();
            this.appointmentsAvailbilityReportSenderMock = new Mock<IAppointmentsAvailbilityReportSender>();
            this.appointmentsAvailbilityReportGenerator = new AppointmentsAvailbilityReportGenerator(this.dpfGatewayMock.Object, this.appointmentsAvailbilityReportSenderMock.Object, this.loggerMock.Object);            
        }

        [Fact]
        public async Task HaveOfficesWithOutAlerts_ShouldBePresentInFinalDictionary()
        {
            // Arrange
            var sessionData = new Model.SessionData() { Cookie = Guid.NewGuid().ToString() };
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            var alerts = new List<string>() { "No dates availble" };
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleCities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFCity>>(cities, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleOffices(It.IsAny<Model.SessionData>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFOffice>>(offices, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAppointmentAlertsFromOffice(It.IsAny<Model.SessionData>(), It.IsAny<string>(), It.IsAny<string>(), offices.FirstOrDefault().Id))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<string>>(new List<string>(), sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAppointmentAlertsFromOffice(It.IsAny<Model.SessionData>(), It.IsAny<string>(), It.IsAny<string>(), offices.LastOrDefault().Id))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<string>>(alerts, sessionData)
                ));
            Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>> result = null;
            this.appointmentsAvailbilityReportSenderMock
                .Setup(a => a.Send(It.IsAny<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>()))
                .Callback<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>((obj) => result = obj)
                .Returns(true);            
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a,
                                                                    a.Id == offices.LastOrDefault().Id ? 
                                                                        alerts :
                                                                        a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> expected = generateExpectedDictionary(cities, officesTuple);            

            // Act
            await this.appointmentsAvailbilityReportGenerator.Generate(null, null, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveOfficesJustWithAlerts_ShouldBePresentInFinalDictionary()
        {
            // Arrange
            var sessionData = new Model.SessionData() { Cookie = Guid.NewGuid().ToString() };
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            var alerts = new List<string>() { "No dates availble" };
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleCities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFCity>>(cities, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleOffices(It.IsAny<Model.SessionData>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFOffice>>(offices, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAppointmentAlertsFromOffice(It.IsAny<Model.SessionData>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<string>>(alerts, sessionData)
                ));
            Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>> result = null;
            this.appointmentsAvailbilityReportSenderMock
                .Setup(a => a.Send(It.IsAny<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>()))
                .Callback<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>((obj) => result = obj)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? alerts : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> expected = generateExpectedDictionary(cities, officesTuple);

            // Act
            await this.appointmentsAvailbilityReportGenerator.Generate(null, null, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveOfficesJustWithOutAlerts_ShouldBePresentInFinalDictionary()
        {
            // Arrange
            var sessionData = new Model.SessionData() { Cookie = Guid.NewGuid().ToString() };
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();            
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleCities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFCity>>(cities, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleOffices(It.IsAny<Model.SessionData>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFOffice>>(offices, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAppointmentAlertsFromOffice(It.IsAny<Model.SessionData>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<string>>(new List<string>(), sessionData)
                ));
            Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>> result = null;
            this.appointmentsAvailbilityReportSenderMock
                .Setup(a => a.Send(It.IsAny<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>()))
                .Callback<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>((obj) => result = obj)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> expected = generateExpectedDictionary(cities, officesTuple);

            // Act
            await this.appointmentsAvailbilityReportGenerator.Generate(null, null, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveOfficesNoApointmentNeedWithOutAlerts_ShouldBePresentInFinalDictionary()
        {
            // Arrange
            var sessionData = new Model.SessionData() { Cookie = Guid.NewGuid().ToString() };
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleCities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFCity>>(cities, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAvailbleOffices(It.IsAny<Model.SessionData>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<Model.DPFOffice>>(offices, sessionData)
                ));
            this.dpfGatewayMock
                .Setup(a => a.GetAppointmentAlertsFromOffice(It.IsAny<Model.SessionData>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Model.DPFResponse<IReadOnlyCollection<string>>(new List<string>(), sessionData)
                ));
            Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>> result = null;
            this.appointmentsAvailbilityReportSenderMock
                .Setup(a => a.Send(It.IsAny<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>()))
                .Callback<Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>>((obj) => result = obj)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> expected = generateExpectedDictionary(cities, officesTuple);

            // Act
            await this.appointmentsAvailbilityReportGenerator.Generate(null, null, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        private static Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> generateExpectedDictionary(List<Model.DPFCity> cities, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple)
        {
            var dictionaryResult = new Dictionary<Model.DPFCity, List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>>>();

            foreach (var c in cities)
            {                
                dictionaryResult.Add(c, new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>());
                foreach (var o in officesTuple)
                {
                    dictionaryResult[c].Add(o);
                }
            }

            return dictionaryResult;
        }
    }
}
