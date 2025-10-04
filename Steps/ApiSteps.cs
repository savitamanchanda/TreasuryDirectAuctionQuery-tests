using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;
using TreasuryDirect.Reqnroll.Support;

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

        [When(
            @"I make a GET request with JSON format to the securities search endpoint with CUSIP ""(.*)"""
        )]
        public async Task WhenIGetRequestWithCusip(string cusip)
        {
            var query = new Dictionary<string, string> { { "cusip", cusip }, { "format", "json" } };
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

        //Search for securities by type and date range
        [When(
            @"I make a GET request with JSON format to search for ""(.*)"" securities between ""(.*)"" and ""(.*)"""
        )]
        public async Task WhenISearchByTypeAndDate(string type, string startDate, string endDate)
        {
            var query = new Dictionary<string, string>
            {
                { "type", type },
                { "startDate", startDate },
                { "endDate", endDate },
                { "dateFieldName", "issueDate" },
                { "format", "json" },
            };

            _response = await _client.CallApi(query);
        }

        [Then(@"every security's ""(.*)"" should be within the requested date range")]
        public async Task ThenIssueDateShouldBeWithinRange(string field)
        {
            var body = await _response.Content.ReadAsStringAsync();
            var json = JArray.Parse(body);

            foreach (var sec in json)
            {
                var issueDate = DateTime.Parse(sec[field]?.ToString());
                Assert.IsTrue(
                    issueDate >= DateTime.Parse("01/01/2024")
                        && issueDate <= DateTime.Parse("03/31/2024"),
                    $"Issue date {issueDate} is outside expected range."
                );
            }
        }

        //Request data in XHTML format

        [When(
            @"I make a GET request to search for ""(.*)"" securities with the format set to ""(.*)"""
        )]
        public async Task WhenISearchWithFormat(string type, string format)
        {
            var query = new Dictionary<string, string> { { "type", type }, { "format", format } };
            _response = await _client.CallApi(query);
        }

        [Then(@"the response ""Content-Type"" header should contain ""(.*)""")]
        public void ThenContentTypeHeaderShouldContain(string expectedType)
        {
            var contentType = _response.Content.Headers.ContentType?.MediaType;
            Assert.IsTrue(
                contentType != null && contentType.Contains(expectedType),
                $"Expected Content-Type to contain {expectedType}, but got {contentType}"
            );
        }

        [Then(@"the response body should be valid XHTML")]
        public async Task ThenResponseShouldBeValidXHTML()
        {
            var body = await _response.Content.ReadAsStringAsync();
            Assert.IsTrue(
                body.TrimStart().StartsWith("<!DOCTYPE html") || body.Contains("<html"),
                "Expected XHTML content but found invalid or non-HTML body."
            );
        }

        //Invalid CUSIP

        [Then(@"the response body should be an empty JSON array ""(.*)""")]
        public async Task ThenResponseShouldBeAnEmptyJsonArray(string expectedBody)
        {
            var body = await _response.Content.ReadAsStringAsync();
            Assert.AreEqual(
                expectedBody.Trim(),
                body.Trim(),
                $"Expected an empty JSON array {expectedBody}, but got: {body}"
            );
        }

        //Invalid date format
        [When(
            @"I make a GET request to search for ""(.*)"" securities with a start date of ""(.*)"""
        )]
        public async Task WhenISearchWithInvalidDate(string type, string invalidDate)
        {
            var query = new Dictionary<string, string>
            {
                { "type", type },
                { "startDate", invalidDate },
                { "format", "json" },
            };
            _response = await _client.CallApi(query);
        }

        [Then(
            @"the response body should contain an error message regarding the invalid date format"
        )]
        public async Task ThenInvalidDateErrorMessageIsReturned()
        {
            var body = await _response.Content.ReadAsStringAsync();

            Assert.IsTrue(
                body.Contains("Bad Request", StringComparison.OrdinalIgnoreCase)
                    || body.Contains("400", StringComparison.OrdinalIgnoreCase),
                $"Expected a 400 Bad Request error message, but got: {body}"
            );
        }

        //Original Note Securities
        [When(
            @"I search for ""(.*)"" securities between ""(.*)"" and ""(.*)"" with reopening ""(.*)"""
        )]
        public async Task WhenISearchForOriginalSecurities(
            string type,
            string startDate,
            string endDate,
            string reopening
        )
        {
            var query = new Dictionary<string, string>
            {
                { "type", type },
                { "startDate", startDate },
                { "endDate", endDate },
                { "reopening", reopening },
            };
            _response = await _client.CallApi(query);
        }

        //Verify pagination parameters for distinct results
        private JArray _page1results;
        private JArray _page2results;

        [When(
            @"I make a GET request with JSON format to search for ""(.*)"" securities using pagesize ""(.*)"" and pagenum ""(.*)"""
        )]
        public async Task WhenIVerifyPaginationParameters(
            string type,
            string pagesize,
            string pagenum
        )
        {
            var query = new Dictionary<string, string>
            {
                { "type", type },
                { "pagesize", pagesize },
                { "pagenum", pagenum },
            };
            _response = await _client.CallApi(query);
            var body = await _response.Content.ReadAsStringAsync();

            if (pagenum == "1")
                _page1results = JArray.Parse(body);
            else
                _page2results = JArray.Parse(body);
        }

        [Then(@"the response from page 1 and page 2 should not be identical")]
        public async Task ThenResponseShouldNotBeIdentical()
        {
            Assert.AreNotEqual(
                _page1results.ToString(),
                _page2results.ToString(),
                "Expected page 1 and page 2 responses to differ, but they were identical."
            );
        }
    }
}
