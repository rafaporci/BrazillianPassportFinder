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
    public class AppointmentsAvailbilityReportSenderTest
    {
        private readonly AppointmentsAvailbilityReportSender appointmentsAvailbilityReportSender = null;
        private readonly Mock<ILogger<AppointmentsAvailbilityReportSender>> loggerMock = null;
        private readonly Mock<IEmailNotifier> emailNotifierMock = null;
        
        public AppointmentsAvailbilityReportSenderTest()
        {
            this.loggerMock = new Mock<ILogger<AppointmentsAvailbilityReportSender>>();            
            this.emailNotifierMock = new Mock<IEmailNotifier>();
            this.appointmentsAvailbilityReportSender = new AppointmentsAvailbilityReportSender(this.emailNotifierMock.Object, this.loggerMock.Object);            
        }

        [Fact]
        public void HaveOfficesWithOutAlerts_ShouldBePresentInFinalEmail()
        {
            // Arrange            
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var input = new string[1] { "email@email.com" };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            var alerts = new List<string>() { "No dates availble" };
            string result = null;
            this.emailNotifierMock
                .Setup(a => a.Send(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string[], string, string>((to, subject, message) => result = message)
                .Returns(true);            
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a,
                                                                    a.Id == offices.LastOrDefault().Id ? 
                                                                        alerts :
                                                                        a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult = generateExpectedDictionary(cities, officesTuple);

            var expectedPhrases = new List<string>()
            {
                "SÃO PAULO-SÃO PAULO OFFICE A <font color='blue'>necessita de agendamento e está sem alertas</font>",
                "SÃO PAULO-SÃO PAULO OFFICE B não necessita de agendamento <font color='red'>Closed</font>",
                "SÃO PAULO-SÃO PAULO OFFICE C  necessita de agendamento  <font color='red'>No dates availble</font>",
            };
            var nonExpectedPhrases = new List<string>()
            {
                "Não há destaques no relatório de hoje"
            };

            // Act
            this.appointmentsAvailbilityReportSender.Send(input, dictionaryResult);

            // Assert
            expectedPhrases.ForEach(m => Assert.Contains(m, result));
            nonExpectedPhrases.ForEach(m => Assert.DoesNotContain(m, result));
        }

        [Fact]
        public void HaveOfficesJustWithAlerts_ShouldBePresentInFinalEmail()
        {
            // Arrange            
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var input = new string[1] { "email@email.com" };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            var alerts = new List<string>() { "No dates availble" };
            string result = null;
            this.emailNotifierMock
                .Setup(a => a.Send(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string[], string, string>((to, subject, message) => result = message)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? alerts : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult = generateExpectedDictionary(cities, officesTuple);

            var expectedPhrases = new List<string>()
            {
                "SÃO PAULO-SÃO PAULO OFFICE A  necessita de agendamento  <font color='red'>No dates availble</font>",
                "SÃO PAULO-SÃO PAULO OFFICE B não necessita de agendamento <font color='red'>Closed</font>",
                "SÃO PAULO-SÃO PAULO OFFICE C  necessita de agendamento  <font color='red'>No dates availble</font>",
                "Não há destaques no relatório de hoje"
            };

            // Act
            this.appointmentsAvailbilityReportSender.Send(input, dictionaryResult);

            // Assert
            expectedPhrases.ForEach(m => Assert.Contains(m, result));
        }

        [Fact]
        public void HaveOfficesJustWithOutAlerts_ShouldBePresentInFinalEmail()
        {
            // Arrange            
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var input = new string[1] { "email@email.com" };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false, Alerts = "Closed" },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();
            string result = null;
            this.emailNotifierMock
                .Setup(a => a.Send(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string[], string, string>((to, subject, message) => result = message)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult = generateExpectedDictionary(cities, officesTuple);

            var expectedPhrases = new List<string>()
            {
                "SÃO PAULO-SÃO PAULO OFFICE A <font color='blue'>necessita de agendamento e está sem alertas</font>",
                "SÃO PAULO-SÃO PAULO OFFICE B não necessita de agendamento <font color='red'>Closed</font>",
                "SÃO PAULO-SÃO PAULO OFFICE C <font color='blue'>necessita de agendamento e está sem alertas</font>",
            };
            var nonExpectedPhrases = new List<string>()
            {
                "Não há destaques no relatório de hoje"
            };

            // Act
            this.appointmentsAvailbilityReportSender.Send(input, dictionaryResult);

            // Assert
            expectedPhrases.ForEach(m => Assert.Contains(m, result));
            nonExpectedPhrases.ForEach(m => Assert.DoesNotContain(m, result));
        }

        [Fact]
        public void HaveOfficesNoApointmentNeedWithOutAlerts_ShouldBePresentInFinalEmail()
        {
            // Arrange
            var sessionData = new Model.SessionData() { Cookie = Guid.NewGuid().ToString() };
            var cities = new List<Model.DPFCity>() { new Model.DPFCity() { Id = "1", Name = "SÃO PAULO" } };
            var input = new string[1] { "email@email.com" };
            var offices = new List<Model.DPFOffice>() {
                                new Model.DPFOffice() { Id = "2025", Name = "SÃO PAULO OFFICE A", IsAppointmentMandatory = true  },
                                new Model.DPFOffice() { Id = "2026", Name = "SÃO PAULO OFFICE B", IsAppointmentMandatory = false },
                                new Model.DPFOffice() { Id = "2027", Name = "SÃO PAULO OFFICE C", IsAppointmentMandatory = true }};
            List<Tuple<Model.DPFOffice, IReadOnlyCollection<string>>> officesTuple = new List<Tuple<DPFOffice, IReadOnlyCollection<string>>>();            
            string result = null;
            this.emailNotifierMock
                .Setup(a => a.Send(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string[], string, string>((to, subject, message) => result = message)
                .Returns(true);
            offices.ForEach(a => officesTuple.Add(new Tuple<DPFOffice, IReadOnlyCollection<string>>(a, a.IsAppointmentMandatory ? new List<string>() : null)));
            Dictionary<DPFCity, List<Tuple<DPFOffice, IReadOnlyCollection<string>>>> dictionaryResult = generateExpectedDictionary(cities, officesTuple);

            var expectedPhrases = new List<string>()
            {
                "SÃO PAULO-SÃO PAULO OFFICE A <font color='blue'>necessita de agendamento e está sem alertas</font>",
                "SÃO PAULO-SÃO PAULO OFFICE B <font color='blue'>não necessita de agendamento e está sem alertas</font>",
                "SÃO PAULO-SÃO PAULO OFFICE C <font color='blue'>necessita de agendamento e está sem alertas</font>",
            };
            var nonExpectedPhrases = new List<string>()
            {
                "Não há destaques no relatório de hoje"
            };

            // Act
            this.appointmentsAvailbilityReportSender.Send(input, dictionaryResult);

            // Assert
            expectedPhrases.ForEach(m => Assert.Contains(m, result));
            nonExpectedPhrases.ForEach(m => Assert.DoesNotContain(m, result));
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
