using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PassportFinder.Data.Extentions
{
    public static class SessionDataExtentions
    {
        public static void FillRequest(this SessionData sessionData, HttpRequestMessage requestMessage)
        {
            if (requestMessage.Headers.Contains("Cookie"))
                requestMessage.Headers.Remove("Cookie");
            
            requestMessage.Headers.Add("Cookie", sessionData.Cookie);
        }
    }
}
