namespace PathFinder_Plus.Models
{
    public class RequestBody
    {
        public required List<Coordinate> Pois { get; set; }
        public required Coordinate Start { get; set; }
        public required Coordinate End { get; set; }
    }
}
