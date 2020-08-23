using PassportFinder.Data.HtmlFinders;
using Xunit;
using PassportFinder.Tests.Common;
using System.Reflection;
using System.Threading.Tasks;
using PassportFinder.Model;
using System.Collections.Generic;
using FluentAssertions;
using Xunit.Sdk;

namespace PassportFinder.Data.Tests.HtmlFinders
{
    public class DPFCityFinderTest
    {
        private readonly DPFCityFinder dpfOfficeFinder = null;
        public DPFCityFinderTest()
        {
            this.dpfOfficeFinder = new DPFCityFinder();
        }

        [Fact]
        public async Task HaveValidHtmlWith3Offices_ShouldReturn3Offices()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_3Cities.html");
            var expected = new List<DPFCity>() { 
                new DPFCity() { Id = "7107", Name = "SÃO PAULO" },
                new DPFCity() { Id = "7115", Name = "SÃO SEBASTIÃO" },
                new DPFCity() { Id = "7145", Name = "SOROCABA" }
            };

            // Act
            var result = this.dpfOfficeFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveValidHtmlWith0Offices_ShouldReturn0Offices()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_0Cities.html");
            var expected = new List<DPFCity>();

            // Act
            var result = this.dpfOfficeFinder.Find(html);

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
                this.dpfOfficeFinder.Find(html);
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
