using FluentAssertions;
using PassportFinder.Data.HtmlFinders;
using PassportFinder.Model;
using PassportFinder.Tests.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace PassportFinder.Data.Tests.HtmlFinders
{
    public class DPFOfficeFinderTest
    {
        private readonly DPFOfficeFinder dpfOfficeFinder = null;
        public DPFOfficeFinderTest()
        {
            this.dpfOfficeFinder = new DPFOfficeFinder();
        }

        [Fact]
        public async Task HaveValidHtmlWithOfficesWithAndWithoutApoint_ShouldReturn2Offices()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_ListOffices_WithOfficesWithAndWithoutApoint.html");
            var expected = new List<DPFOffice>() { 
                new DPFOffice() { Id = "207", Name = "DELEGACIA ESPECIAL DE POLICIA FEDERAL NO AEROPORTO INTERNACIONAL DE SÃO PAULO/GUARULHOS - DEAIN/SR/PF/SP - DEAIN/SR/PF/SP - CUMBICA - GUARULHOS - SP", IsAppointmentMandatory = true },
                new DPFOffice() { Id = "1381", Name = "SHOPPING INTERNACIONAL DE GUARULHOS - PEP - SHOPPING GUARULHOS - SP - GUARULHOS / SP", Alerts = " (Temporariamente fechado - Emergência -->E-mail: migracao.sp@dpf.gov.br)" },                
            };

            // Act
            var result = this.dpfOfficeFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HaveValidHtmlJusWithOfficesWithApoint_ShouldReturn2Offices()
        {
            // Arrange
            var html = await Assembly.GetExecutingAssembly().ReadResourceAsString("PassportFinder.Data.Tests.Resources.DPFOfficesPage_ListOffices_JusWithOfficesWithApoint.html");
            var expected = new List<DPFOffice>() {
                new DPFOffice() { Id = "207", Name = "DELEGACIA ESPECIAL DE POLICIA FEDERAL NO AEROPORTO INTERNACIONAL DE SÃO PAULO/GUARULHOS - DEAIN/SR/PF/SP - DEAIN/SR/PF/SP - CUMBICA - GUARULHOS - SP", IsAppointmentMandatory = true },
                new DPFOffice() { Id = "1381", Name = "SHOPPING INTERNACIONAL DE GUARULHOS - PEP - SHOPPING GUARULHOS - SP - GUARULHOS / SP", Alerts = " (Temporariamente fechado - Emergência -->E-mail: migracao.sp@dpf.gov.br)", IsAppointmentMandatory = true },
            };

            // Act
            var result = this.dpfOfficeFinder.Find(html);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
