using Microsoft.AspNet.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    // This class contains methods to make easier to read responses in different formats
    // until there is a built-in easier way to do it.
    public static class HttpResponseHelpers
    {
        public static async Task<string> ReadBodyAsStringAsync(this HttpResponse response)
        {
            using (var streamReader = new StreamReader(response.Body))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}