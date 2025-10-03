using NUnit.Framework;
using Reqnroll;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TreasuryDirect.Reqnroll.Support;
using System.Collections.Generic;

namespace TreasuryDirect.Reqnroll.Steps
{
    [Binding]
    public class ApiSteps
    {
        private readonly ApiClient _client;
        private HttpResponseMessage _response;

        public ApiSteps()
        {
            //Base URL for Treasury Direct API
            _client = new ApiClient("https://treasurydirect.gov/TA_WS/securities/search");
        }

        //Retrieve by CUSIP

        [When(@"I make a GET request with JSON format to the securities search endpoint with CUSIP ""(.*)""")]
        public async Task WhenIGetRequestWithCusip(string cusip)
        {
            var query = new Dictionary<string, string>
            {
                { "cusip", cusip },
                { "format", "json" }
            };
            _response = await _client.CallApi(query);
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenStatusCodeShouldBe(int expectedStatus)
        {
            Assert.AreEqual(expectedStatus, (int)_response.StatusCode);
        }

        [Then(@"the response body should be a non-empty JSON array")]
        public async Task ThenResponseShouldBeNonEmptyArray()
        {
            var body = await _response.Content.ReadAsStringAsync();
            var json = JArray.Parse(body);
            Assert.IsTrue(json.Count > 0, "Expected non-empty JSON array, but got empty");
        }

        [Then(@"every security in the response array should have a ""(.*)"" of ""(.*)""")]
        public async Task ThenAllSecuritiesShouldMatch(string field, string value)
        {
            var body = await _response.Content.ReadAsStringAsync();
            var json = JArray.Parse(body);

            foreach (var sec in json)
            {
                Assert.AreEqual(value, sec[field]?.ToString());
            }
        }
    }
}
