namespace PathFinder_Plus.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using PathFinder_Plus.Models;

    public class Greedy
    {
        private readonly HttpClient _client;
        private readonly string ORS_KEY = EnvironmentalVariables.ORS_KEY;
        private readonly string openRouteServiceBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";

        public Greedy()
        {
            if (ORS_KEY is null)
            {
                throw new Exception("Cannot fetch OpenRouteService API key!");
            }

            _client = new HttpClient();
        }

        public async Task<List<Route>> FindMinimumDistanceRoute(List<Coordinate> pois)
        {
            var points = new List<Coordinate>(pois);

            var current = points[0];
            points.RemoveAt(0);

            var route = new List<Coordinate>() { current };

            while (points.Count > 0)
            {
                var nearestIndex = -1;
                var minDistance = double.MaxValue;

                foreach (var point in points)
                {
                    var distance = await GetRouteDistance(current, point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestIndex = points.IndexOf(point);
                    }
                }

                current = points[nearestIndex];
                points.RemoveAt(nearestIndex);
                route.Add(current);
            }

            route.Add(route[0]);

            return [new Route(route, await GetTotalDistance(route))];
        }

        private async Task<double> GetTotalDistance(List<Coordinate> route)
        {
            var totalDistance = 0.0;

            for (int i = 0; i < route.Count - 1; i++)
            {
                totalDistance += await GetRouteDistance(route[i], route[i + 1]);
            }
            return totalDistance;
        }

        private async Task<double> GetRouteDistance(Coordinate start, Coordinate end)
        {
            var startString = $"{start.Latitude},{start.Longitude}";
            var endString = $"{end.Latitude},{end.Longitude}";

            var directionsRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{openRouteServiceBaseUrl}?api_key={ORS_KEY}&start={startString}&end={endString}"),
                Method = HttpMethod.Get
            };

            var response = await _client.SendAsync(directionsRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var distance = ParseDistanceFromResponse(responseString);
                return distance;
            }
            else
            {
                throw new Exception($"Error getting directions: {response.StatusCode}");
            }
        }

        private static double ParseDistanceFromResponse(string responseString)
        {
            JsonDocument responseDocument = JsonDocument.Parse(responseString);
            JsonElement root = responseDocument.RootElement;

            if (!root.TryGetProperty("features", out JsonElement featuresElement))
            {
                Console.WriteLine("Features property not found.");
                throw new Exception("Failed to parse distance from OpenRouteService response.");
            }

            if (featuresElement.ValueKind != JsonValueKind.Array || featuresElement.GetArrayLength() == 0)
            {
                Console.WriteLine("Features property is not an array or is empty.");
                throw new Exception("Failed to parse distance from OpenRouteService response.");
            }

            JsonElement firstFeature = featuresElement[0];

            if (!firstFeature.TryGetProperty("properties", out JsonElement propertiesElement) ||
                !propertiesElement.TryGetProperty("summary", out JsonElement summaryElement))
            {
                Console.WriteLine("Properties or Summary property not found.");
                throw new Exception("Failed to parse distance from OpenRouteService response.");
            }

            if (!summaryElement.TryGetProperty("distance", out JsonElement distanceElement) ||
                distanceElement.ValueKind != JsonValueKind.Number)
            {
                Console.WriteLine("Distance property not found or is not a number.");
                throw new Exception("Failed to parse distance from OpenRouteService response.");
            }

            return distanceElement.GetDouble();
        }
    }
}