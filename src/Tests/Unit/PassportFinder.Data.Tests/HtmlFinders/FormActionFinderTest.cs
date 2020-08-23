using FluentAssertions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Tests.Common;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace PassportFinder.Data.Tests.HtmlFinders
{
    public class FormActionFinderTest
    {
        private readonly FormActionFinder formActionFinder = null;
        public FormActionFinderTest()
        {
            this.formActionFinder = new FormActionFinder();
        }

        [Fact]
        public async Task HaveValidHtmlWithAction_ShouldReturnAction()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_3Cities.html");
            var expected = "/sinpa/realizarAgendamento.do";

            // Act
            var result = this.formActionFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveInvalidHTML_ShouldThrowException()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_0Cities_InvalidHTML.html");

            // Act
            try
            {
                this.formActionFinder.Find(html);
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
