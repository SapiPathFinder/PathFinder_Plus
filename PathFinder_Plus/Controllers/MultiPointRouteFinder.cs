namespace PathFinder_Plus.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;

    public class MultiPointRouteFinder
    {
        private readonly HttpClient _client;

        private readonly string ORS_KEY = EnvironmentalVariables.ORS_KEY;

        private readonly string openRouteServiceBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";

        public MultiPointRouteFinder()
        {
            if (ORS_KEY is null)
            {
                throw new Exception("Cannot fetch OpenRouteService API key!");
            }

            _client = new HttpClient();
        }

        public async Task<List<Route>> FindMinimumDistanceRoutes(List<Coordinate> pois)
        {
            if (pois.Count < 2)
            {
                throw new ArgumentException("At least 2 POIs are required.");
            }

            var points = new List<Coordinate>(pois);

            var current = points[0];
            points.RemoveAt(0);

            var route = new List<Coordinate>() { current };

            while (points.Count > 0)
            {
                var nearestIndex = -1;
                var minDistance = double.MaxValue;

                foreach ( var point in points)
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

        private static List<List<Coordinate>> GetPermutations(List<Coordinate> points)
        {
            if (points.Count == 1)
            {
                return [points];
            }

            var permutations = new List<List<Coordinate>>();

            foreach (var point in points)
            {
                var remaining = new List<Coordinate>(points);
                remaining.Remove(point);
                var subPermutations = GetPermutations(remaining);
                foreach (var permutation in subPermutations)
                {
                    permutation.Insert(0, point);
                    permutations.Add(permutation);
                }
            }
            return permutations;
        }

        private static double ParseDistanceFromResponse(string responseString)
        {
            // TODO (KB): Refactor this

            JsonDocument responseDocument = JsonDocument.Parse(responseString);
            JsonElement root = responseDocument.RootElement;

            if (root.TryGetProperty("features", out JsonElement featuresElement))
            {
                if (featuresElement.ValueKind == JsonValueKind.Array && featuresElement.GetArrayLength() > 0)
                {
                    JsonElement firstFeature = featuresElement[0];

                    if (firstFeature.TryGetProperty("properties", out JsonElement propertiesElement) &&
                        propertiesElement.TryGetProperty("summary", out JsonElement summaryElement))
                    {
                        if (summaryElement.TryGetProperty("distance", out JsonElement distanceElement))
                        {
                            if (distanceElement.ValueKind == JsonValueKind.Number)
                            {
                                return distanceElement.GetDouble();
                            }
                            else
                            {
                                Console.WriteLine("Distance property is not a number.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Distance property not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Properties or Summary property not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Features property is not an array or is empty.");
                }
            }
            else
            {
                Console.WriteLine("Features property not found.");
            }

            throw new Exception("Failed to parse distance from Openrouteservice response.");
        }
    }

    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Route(List<Coordinate> points, double totalDistance)
    {
        public List<Coordinate> Points { get; set; } = points;
        public double TotalDistance { get; set; } = totalDistance;
    }

}
