using PassportFinder.Data.Builders;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.HtmlFinders
{
    public class DPFOfficeFinder
    {
        public IReadOnlyCollection<DPFOffice> Find(string html)
        {
            var listOffices = new List<DPFOffice>();

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml($"<html>{html}</html>");

            var nodesH2 = htmlDocument.DocumentNode.SelectNodes("//h2");

            if (nodesH2 == null)
                return listOffices;

            foreach (var node in nodesH2)
            {
                bool isApointmentMandatory = node.InnerText.ToLower().Contains("clique sobre ele para prosseguir com seu agendamento");
                var nodeDiv = node.NextSibling;
                while (nodeDiv?.Name == "div")
                {
                    listOffices.Add(new DPFOfficeBuilder().Build(nodeDiv, isApointmentMandatory));
                    nodeDiv = nodeDiv.NextSibling;
                }                
            }

            return listOffices;
        }
    }
}
