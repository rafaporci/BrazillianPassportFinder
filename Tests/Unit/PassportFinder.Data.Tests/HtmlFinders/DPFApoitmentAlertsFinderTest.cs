using FluentAssertions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Tests.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace PassportFinder.Data.Tests.HtmlFinders
{
    public class DPFApoitmentAlertsFinderTest
    {
        private readonly DPFApoitmentAlertsFinder dpfApoitmentAlertsFinder = null;
        public DPFApoitmentAlertsFinderTest()
        {
            this.dpfApoitmentAlertsFinder = new DPFApoitmentAlertsFinder();
        }

        [Fact]
        public async Task HaveValidHtmlWithAlerts_ShouldReturnAlert()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_ApoitmentPageScheduleWithAlerts.html");
            var expected = new List<string>() { 
                "Nenhum horário disponível. Consulte outro posto."
            };

            // Act
            var result = this.dpfApoitmentAlertsFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveValidHtmlWithoutAlerts_ShouldNotReturnAlert()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_ApoitmentPageScheduleWithOutAlerts.html");
            var expected = new List<string>();

            // Act
            var result = this.dpfApoitmentAlertsFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveInvalidHTML_ShouldThrowException()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_ApoitmentPageScheduleInvalidHTML.html");

            // Act
            try
            {
                this.dpfApoitmentAlertsFinder.Find(html);
            }
            catch (Exceptions.NotExpectedHtmlException)
            {
                // That's fine
                return;
            }

            throw new XunitException("Did not detected the error");
        }
    }
}
