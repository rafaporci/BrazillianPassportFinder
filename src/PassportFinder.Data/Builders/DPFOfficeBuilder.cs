using HtmlAgilityPack;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PassportFinder.Data.Builders
{
    public class DPFOfficeBuilder
    {
        public DPFOffice Build(HtmlNode node, bool isAppointmentMandatory)
        {
            var office = new DPFOffice();

            office.Id = getId(node);            
            office.IsAppointmentMandatory = isAppointmentMandatory;
            var nodeA = node.SelectSingleNode("a");
            if (nodeA == null)
                return office;

            office.Name = nodeA.FirstChild.InnerText.Replace("&nbsp;", " ");
            if (nodeA.LastChild.Name == "font")
                office.Alerts = nodeA.LastChild.InnerText;

            return office;
        }

        private string getId(HtmlNode node)
        {
            var javascriptLinkWithId = node.SelectSingleNode("a")?.Attributes["href"]?.Value;
            if (javascriptLinkWithId == null)
                return String.Empty;

            var m = new Regex("'(?<Id>[0-9]+)'").Match(javascriptLinkWithId);
            if (m.Success)
                return m.Groups["Id"]?.Value;
            else
                return String.Empty;
        }
    }
}
