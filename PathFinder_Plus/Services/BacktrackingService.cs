namespace PathFinder_Plus.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Text;
    using PathFinder_Plus.Models;

    public class BacktrackingService
    {
        private readonly HttpClient _client;
        private readonly string ORS_KEY = EnvironmentalVariables.ORS_KEY;
        private readonly string matrixBaseUrl = "https://api.openrouteservice.org/v2/matrix/driving-car";

        private readonly POIService _poiService = new();

        private class DistanceMatrix
        {
            public double[][]? distances { get; set; }
        }

        public BacktrackingService()
        {
            if (ORS_KEY is null)
            {
                throw new Exception("Cannot fetch OpenRouteService API key!");
            }

            _client = new HttpClient();
        }

        public async Task<Route> FindMinimumDistanceRouteBt(Coordinate start, Coordinate end, int buffer)
        {
            var standardPois = await _poiService.GetPOIsStandard(start, end, buffer);

            var pois = ExtractPointsFromResponse(standardPois);

            pois.Add(new Coordinate(start.Longitude, start.Latitude));
            pois.Add(new Coordinate(end.Longitude, end.Latitude));

            if (buffer > 250)
            {
                return new Route(pois, 0);
            }

            var payloadPois = pois.Select(poi => new double[] { poi.Longitude, poi.Latitude }).ToList();

            using StringContent jsonContent = new(
             JsonSerializer.Serialize(new
             {
                 locations = payloadPois,
                 metrics = new string[] { "distance" }
             }),
             Encoding.UTF8,
             "application/json");

            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(matrixBaseUrl),
                Headers = { { "Authorization", ORS_KEY } },
                Content = jsonContent
            };

            using HttpResponseMessage response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var distanceMatrix = new double[pois.Count][];

            try
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonSerializer.Deserialize<DistanceMatrix>(jsonResponse);

                if (jsonObject != null && jsonObject.distances != null)
                {
                    distanceMatrix = jsonObject.distances;
                    for (int i = 0; i < distanceMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < distanceMatrix.GetLength(0); j++)
                        {
                            Console.Write($"{distanceMatrix[i][j]} ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Deserialization resulted in null object.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }

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

        private List<Coordinate> ExtractPointsFromResponse(string jsonString)
        {
            var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(jsonString);
            var points = new List<Coordinate>();

            foreach (var feature in featureCollection.Features)
            {
                var coordinates = feature.Geometry.Coordinates;
                points.Add(new Coordinate(coordinates[1], coordinates[0]));
            }

            return points;
        }

        private static double CalculateRouteDistance(List<Coordinate> route, double[][] distanceMatrix)
        {
            var totalDistance = 0.0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                totalDistance += distanceMatrix[i][i + 1];
            }
            return totalDistance;
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
    }
}