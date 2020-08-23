using PassportFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PassportFinder.Data.Builders
{
    public class SessionDataBuilder
    {
        public SessionData Build(HttpResponseMessage message)
        {
            var session = new SessionData();

            if (message.Headers.Contains("Set-Cookie"))
                session.Cookie = message.Headers.GetValues("Set-Cookie").FirstOrDefault();

            return session;
        }
    }
}
