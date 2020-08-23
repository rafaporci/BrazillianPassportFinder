using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Model
{
    public class DPFResponse<T>
    {
        public DPFResponse(T data, SessionData sessionData)
        {
            this.Data = data;
            this.SessionData = sessionData;
        }

        public T Data { get; set; }

        public SessionData SessionData { get; set; }
    }
}
