namespace PathFinder_Plus.Models
{
    public class Route(List<Coordinate> points, double totalDistance)
    {
        public List<Coordinate> Points { get; set; } = points;
        public double TotalDistance { get; set; } = totalDistance;
    }

    public class RouteWithStartAndEndPoint : Route
    {
        public Coordinate StartPoint { get; set; }
        public Coordinate EndPoint { get; set; }

        public RouteWithStartAndEndPoint(List<Coordinate> points, double totalDistance, Coordinate startPoint, Coordinate endPoint)
            : base(points, totalDistance)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }

    public class RouteWithStartAndEndPointAndBuffer : RouteWithStartAndEndPoint
    {
        public int Buffer { get; set; }

        public RouteWithStartAndEndPointAndBuffer(List<Coordinate> points, double totalDistance, Coordinate startPoint, Coordinate endPoint, int buffer)
            : base(points, totalDistance, startPoint, endPoint)
        {
            Buffer = buffer;
        }
    }
}
