using PassportFinder.Data.Builders;
using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.HtmlFinders
{
    public class FormActionFinder
    {
        public string Find(string html)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var formNode = htmlDocument.DocumentNode.SelectSingleNode("//form");
            if (formNode == null)
                throw new Exceptions.NotExpectedHtmlException("form", html);

            return formNode.Attributes["action"].Value;
        }
    }
}
