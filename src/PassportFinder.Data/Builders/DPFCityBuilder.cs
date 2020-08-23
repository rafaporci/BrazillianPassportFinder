using HtmlAgilityPack;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.Builders
{
    public class DPFCityBuilder
    {
        public DPFCity Build(HtmlNode node)
        {
            var office = new DPFCity();

            office.Id = node.Attributes["value"]?.Value;
            office.Name = node.InnerText;

            return office;
        }
    }
}
