using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Text.Json;

namespace Blazor_Backend_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OmdbController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OmdbController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.omdbapi.com/");
            _apiKey = configuration["OmdbApiKey"] ?? throw new ArgumentNullException("OmdbApiKey is missing.");
        }

        [HttpGet("{imdbId}")]
        public async Task<ActionResult<Movie>> GetMovieWithExtendedDescription(string imdbId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?i={imdbId}&type=movie&plot=full&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movie != null) 
                {
                    var queryWithoutApiKey = 
                        _httpClient?
                            .BaseAddress?
                            .ToString() + 
                        response
                            .RequestMessage?
                            .RequestUri?
                            .Query
                            .Replace($"&apikey={_apiKey}", "") ?? string.Empty;
                    movie.QueryWithoutApiKey = queryWithoutApiKey;
                }

                if (movie?.Response == "True")
                    return Ok(movie);
                return BadRequest(new { Error = movie?.Error ?? "Unknown error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Failed to fetch movie: {ex.Message}" });
            }
        }

        [HttpGet("title/{title}")]
        public async Task<ActionResult<Movie>> GetImdbRecordByTitle(string imdbTitle)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?t={Uri.EscapeDataString(imdbTitle)}&type=movie&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movie != null)
                {
                    var queryWithoutApiKey =
                        _httpClient?
                            .BaseAddress?
                            .ToString() +
                        response
                            .RequestMessage?
                            .RequestUri?
                            .Query
                            .Replace($"&apikey={_apiKey}", "") ?? string.Empty;
                    movie.QueryWithoutApiKey = queryWithoutApiKey;
                }

                if (movie?.Response == "True")
                    return Ok(movie);
                return BadRequest(new { Error = movie?.Error ?? "Unknown error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Failed to fetch movie: {ex.Message}" });
            }
        }

        // Search movies by title
        [HttpGet("search/{title}")]
        public async Task<ActionResult<List<Movie>>> SearchMovies(string title)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?s={Uri.EscapeDataString(title)}&type=movie&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<OmdbSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (searchResult?.Response == "True")
                {
                    var queryWithoutApiKey =
                        _httpClient?
                            .BaseAddress?
                            .ToString() +
                        response
                            .RequestMessage?
                            .RequestUri?
                            .Query
                            .Replace($"&apikey={_apiKey}", "") ?? string.Empty;
                    searchResult.Search.ForEach(movie => { movie.QueryWithoutApiKey = queryWithoutApiKey; });
                    return Ok(searchResult.Search);
                }

                return BadRequest(new { Error = searchResult?.Error ?? "Unknown error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Failed to search movies: {ex.Message}" });
            }
        }
    }

    // Helper class for search results
    public class OmdbSearchResult
    {
        public List<Movie> Search { get; set; } = new List<Movie>();
        // ToDo: There could be search results that exceed api list entry and we would need to use more then one page of results..so that is why total results are here
        public string TotalResults { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
