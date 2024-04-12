using Microsoft.AspNetCore.Mvc;

namespace PathFinder_Plus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController(MultiPointRouteFinder routeFinder) : ControllerBase
    {
        private readonly MultiPointRouteFinder _routeFinder = routeFinder;

        [HttpPost]
        [Route("routeAndPoi")]
        public async Task<IActionResult> FindRoutes([FromBody] List<Coordinate> pois)
        {
            if (pois == null || pois.Count < 2)
            {
                return BadRequest("Please provide at least 2 POIs in the request body.");
            }

            var routes = await _routeFinder.FindMinimumDistanceRoutes(pois);
            return Ok(routes);
        }
    }
}
