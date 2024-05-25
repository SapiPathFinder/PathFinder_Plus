namespace PathFinder_Plus.Models
{
    public class RequestBody
    {
        public required List<Coordinate> Pois { get; set; }
        public required Coordinate Start { get; set; }
    }

    public class RequestBodyWithEndpoint : RequestBody
    {
        public required Coordinate End { get; set; }
    }

    public class RequestBodyWithBuffer : RequestBodyWithEndpoint
    {
        public int Buffer { get; set; }
    }

    public class RequestBodyBBox
    {
        public required Coordinate Start { get; set; }
        public required Coordinate End { get; set; }

        public required int Buffer { get; set; }
    }
}
