﻿namespace PathFinder_Plus.Services
{
    using PathFinder_Plus.Models;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class POIService
    {
        private readonly HttpClient _client;
        private readonly string ORS_KEY = EnvironmentalVariables.ORS_KEY;
        private readonly string openRouteServiceBaseUrl = "https://api.openrouteservice.org/pois";

        public POIService()
        {
            if (ORS_KEY is null)
            {
                throw new Exception("Cannot fetch OpenRouteService API key!");
            }

            _client = new HttpClient();
        }

        public async Task<string> GetPOIsStandard(Coordinate start, Coordinate end, int buffer = 250)
        {
            var startString = $"{start.Latitude},{start.Longitude}";
            var endString = $"{end.Latitude},{end.Longitude}";

            var json = $@"{{
                ""request"": ""pois"",
                ""geometry"": {{
                    ""bbox"": [
                        [{startString}],
                        [{endString}]
                    ],
                    ""geojson"": {{
                        ""type"": ""Point"",
                        ""coordinates"": [{startString}]
                    }},
                    ""buffer"": {buffer}
                }}
            }}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var directionsRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{openRouteServiceBaseUrl}?api_key={ORS_KEY}"),
                Method = HttpMethod.Post,
                Content = content
            };

            var response = await _client.SendAsync(directionsRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            else
            {
                throw new Exception($"Error getting POIs: {response.StatusCode}");
            }
        }

        public async Task<Dictionary<string, object>> GetPOICategories()
        {
            var json = $@"{{
                ""request"": ""list""
            }}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var directionsRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{openRouteServiceBaseUrl}?api_key={ORS_KEY}"),
                Method = HttpMethod.Post,
                Content = content
            };

            var response = await _client.SendAsync(directionsRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString, options);

                return result;
            }
            else
            {
                throw new Exception($"Error getting POIs: {response.StatusCode}");
            }
        }
    }
}
