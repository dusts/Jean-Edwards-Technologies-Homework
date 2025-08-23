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
        public async Task<ActionResult<Movie>> GetMovie(string imdbId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?i={imdbId}&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movie?.Response == "True")
                    return Ok(movie);
                return BadRequest(new { Error = movie?.Error ?? "Unknown error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Failed to fetch movie: {ex.Message}" });
            }
        }
    }
}
