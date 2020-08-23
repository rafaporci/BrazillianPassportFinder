using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Data.Exceptions
{
    public class NotExpectedHtmlException : InvalidOperationException
    {
        public NotExpectedHtmlException(string tag, string html) : base($"Tag {tag} not found") 
        {
            this.HTML = html;
        }

        public string HTML { get; }
    }
}
