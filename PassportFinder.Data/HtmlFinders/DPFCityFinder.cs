using PassportFinder.Data.Builders;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.HtmlFinders
{
    public class DPFCityFinder
    {
        public IReadOnlyCollection<DPFCity> Find(string html)
        {
            var listOffices = new List<DPFCity>();

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodeSelect = htmlDocument.DocumentNode.SelectSingleNode("//select[@name='cidadePosto']");

            if (nodeSelect == null)
                throw new Exceptions.NotExpectedHtmlException("select[@name='cidadePosto']", html);

            if (nodeSelect.SelectNodes("option[@value!='']") == null)
                return listOffices;

            foreach (var node in nodeSelect.SelectNodes("option[@value!='']"))
            {                
                listOffices.Add(new DPFCityBuilder().Build(node));
            }

            return listOffices;
        }
    }
}
