using PassportFinder.Data.Builders;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.HtmlFinders
{
    public class DPFApoitmentAlertsFinder
    {
        public IReadOnlyCollection<string> Find(string html)
        {
            var listAlerts = new List<string>();

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml($"{html}");

            var nodeAlerts = htmlDocument.DocumentNode.SelectSingleNode("//div[@role='alert']");

            if (nodeAlerts == null)
                throw new Exceptions.NotExpectedHtmlException("div[@role='alert']", html);

            if (String.IsNullOrEmpty(nodeAlerts.InnerText?.Trim()))
                return listAlerts;

            return new string[1] { nodeAlerts.InnerText?.Trim() };
        }
    }
}
