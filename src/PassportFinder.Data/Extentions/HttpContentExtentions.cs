using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Data.Extentions
{
    public static class HttpContentExtentions
   {
        public static async Task<string> ReadAsStringWithEncondingAsync(this HttpContent httpContent, Encoding encoding) 
        {
            var array = await httpContent.ReadAsByteArrayAsync();
            return encoding.GetString(array);
        }
    }
}
