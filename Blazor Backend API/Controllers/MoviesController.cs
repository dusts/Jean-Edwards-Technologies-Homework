using Microsoft.AspNetCore.Mvc;
using SharedModels;
using SharedModels.Services;

namespace Blazor_Backend_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<MovieSearchResult>> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var response = await _movieService.SearchMoviesAsync(query);
            if (response == null || response.Response == "False")
                return NotFound("Movie not found!");

            return Ok(response);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<Movie>> GetMovieDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("ID is required.");

            var response = await _movieService.GetMovieDetailsAsync(id);
            if (response == null || response.Response == "False")
                return NotFound("Movie not found.");

            return Ok(response);
        }
    }
}
