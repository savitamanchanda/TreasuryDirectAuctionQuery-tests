using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TreasuryDirect.Reqnroll.Support
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _http;

        public ApiClient(string baseUrl)
        {
            //Set base API URL
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<HttpResponseMessage> CallApi(Dictionary<string, string> query)
        {
            //Build query string and send GET request
            var qs = BuildQueryString(query);
            return await _http.GetAsync(qs);
        }

        private static string BuildQueryString(Dictionary<string, string> query)
        {
            if (query == null || query.Count == 0)
                return "";
            var parts = new List<string>();
            foreach (var kv in query)
            {
                var k = Uri.EscapeDataString(kv.Key);
                var v = Uri.EscapeDataString(kv.Value ?? string.Empty); //null-safe
                parts.Add($"{k}={v}");
            }
            return "?" + string.Join("&", parts);
        }

        public void Dispose() => _http.Dispose();
    }
}
