using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScaleoConnector
{
    // ScaleoClient is responsible for communicating with the Scaleo API.
    public class ScaleoClient
    {
        private readonly HttpClient _http;   // HTTP client used to send requests
        private readonly string _baseUrl;    // Base URL of the Scaleo API
        private readonly string _apiKey;     // API key for authentication

        public ScaleoClient()
        {
            _http = new HttpClient();

            // Read API configuration from environment variables.
            // If not set, default values are used.
            _baseUrl = Environment.GetEnvironmentVariable("SCALEO_API_URL") ?? "https://api.scaleo.io";
            _apiKey = Environment.GetEnvironmentVariable("SCALEO_API_KEY") ?? "";

            // Add API key to request headers if available.
            if (!string.IsNullOrEmpty(_apiKey))
                _http.DefaultRequestHeaders.Add("api-key", _apiKey);
        }

        // FetchReportsAsync retrieves reports from the Scaleo API within a given date range.
        // Parameters:
        //   from - start date
        //   to   - end date
        //   ct   - cancellation token
        // Returns:
        //   A list of JsonElement objects representing the report data.
        public async Task<List<JsonElement>> FetchReportsAsync(DateTime from, DateTime to, CancellationToken ct)
        {
            // Construct the request URL with date parameters.
            var url = $"{_baseUrl}/reports?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

            // Send GET request and read response as string.
            var resp = await _http.GetStringAsync(url, ct);

            // Parse JSON response.
            using var doc = JsonDocument.Parse(resp);
            var list = new List<JsonElement>();

            // Case 1: Root element is an array.
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in doc.RootElement.EnumerateArray())
                    list.Add(el);
            }
            // Case 2: Root element contains "data" property with an array.
            else if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in data.EnumerateArray())
                    list.Add(el);
            }

            // Return the list of report elements.
            return list;
        }
    }
}