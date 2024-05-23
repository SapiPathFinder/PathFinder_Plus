using Microsoft.AspNetCore.Mvc;
using PathFinder_Plus.Models;
using PathFinder_Plus.Services;

namespace PathFinder_Plus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController() : ControllerBase
    {
        private readonly GreedyService _greedy = new();
        private readonly BacktrackingService _backtracking = new();
        private readonly POIService Api = new();

        [HttpPost]
        [Route("routeAndPoi")]
        public async Task<IActionResult> FindRoute([FromBody] List<Coordinate> pois)
        {
            if (pois == null || pois.Count < 2)
            {
                return BadRequest("Please provide at least 2 POIs in the request body.");
            }

            var routes = await _greedy.FindMinimumDistanceRoute(pois);
            return Ok(routes);
        }

        [HttpPost]
        [Route("routeAndPoiBt")]
        public async Task<IActionResult> FindRouteBt([FromBody] RequestBody request)
        {
            if (request.Pois == null || request.Pois.Count < 2)
            {
                return BadRequest("Please provide at least 2 POIs in the request body.");
            }

            var routes = await _backtracking.FindMinimumDistanceRouteBt(request.Pois, request.Start);
            return Ok(routes);
        }

        [HttpPost]
        [Route("poisStandard")]
        public async Task<IActionResult> GetPOIsStandard([FromBody] RequestBodyWithEndpoint request)
        {
            var pois = await Api.GetPOIsStandard(request.Start, request.End);
            return Ok(pois);
        }

        [HttpPost]
        [Route("poisBuffer")]
        public async Task<IActionResult> GetPOIsBuffer([FromBody] RequestBodyWithBuffer request)
        {
            var pois = await Api.GetPOIsStandard(request.Start, request.End, request.Buffer);
            return Ok(pois);
        }

        [HttpGet]
        [Route("categoriesList")]
        public async Task<IActionResult> GetPOICategories()
        {
            var pois = await Api.GetPOICategories();
            return Ok(pois);
        }
    }
}
