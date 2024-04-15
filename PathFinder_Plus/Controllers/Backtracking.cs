namespace PathFinder_Plus.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using PathFinder_Plus.Models;

    public class Backtracking
    {
        private readonly HttpClient _client;
        private readonly string ORS_KEY = EnvironmentalVariables.ORS_KEY;
        private readonly string openRouteServiceBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";

        public Backtracking()
        {
            if (ORS_KEY is null)
            {
                throw new Exception("Cannot fetch OpenRouteService API key!");
            }

            _client = new HttpClient();
        }

        public async Task<Route> FindMinimumDistanceRouteBt(List<Coordinate> pois, Coordinate start)
        {
          pois.Insert(0, start);

          var distanceMatrix = await CalculateDistanceMatrix(pois);

          var permutations = GetPermutationsBt(pois);
          Route minRoute = null;
          double minDistance = double.MaxValue;

          foreach (var permutation in permutations)
          {
            var totalDistance = CalculateRouteDistance(permutation, distanceMatrix);
            if (totalDistance < minDistance)
            {
              minDistance = totalDistance;
              minRoute = new Route(permutation, totalDistance);
            }
          }

          return minRoute;
        }

        private async Task<double[,]> CalculateDistanceMatrix(List<Coordinate> points)
        {
            var distanceMatrix = new double[points.Count, points.Count];

            var tasks = new List<Task>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count - 2; j++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var distance = await GetRouteDistanceBt(points[i], points[j]);
                        distanceMatrix[i, j] = distance;
                        distanceMatrix[j, i] = distance;
                    }));
                }
            }

            await Task.WhenAll(tasks);

            return distanceMatrix;
        }

        private static double CalculateRouteDistance(List<Coordinate> route, double[,] distanceMatrix)
        {
          var totalDistance = 0.0;
          for (int i = 0; i < route.Count - 1; i++)
          {
            totalDistance += distanceMatrix[i, i + 1];
          }
          return totalDistance;
        }

        private async Task<double> GetRouteDistanceBt(Coordinate start, Coordinate end)
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

        private static List<List<Coordinate>> GetPermutationsBt(List<Coordinate> points)
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
                var subPermutations = GetPermutationsBt(remaining);
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