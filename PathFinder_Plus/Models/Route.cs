namespace PathFinder_Plus.Models
{
    public class Route(List<Coordinate> points, double totalDistance)
    {
        public List<Coordinate> Points { get; set; } = points;
        public double TotalDistance { get; set; } = totalDistance;
    }
}
