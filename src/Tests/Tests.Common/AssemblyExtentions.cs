using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PassportFinder.Tests.Common
{
    public static class AssemblyExtentions
    {
        public static Task<string> ReadResourceAsString(this Assembly assembly, string path) 
        {
            var fileStream = assembly.GetManifestResourceStream(path);
            return new StreamReader(fileStream).ReadToEndAsync();
        }
    }
}
