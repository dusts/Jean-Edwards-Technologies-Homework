using SharedModels;
using SharedModels.Services;
using System.Net.Http;
using System.Text.Json;

namespace Blazor_Backend_API.Services
{
    public class OmdbMovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _omdbApiKey;
        private readonly string _omdbUrl;

        public OmdbMovieService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _omdbApiKey = configuration["OmdbApiKey"] ?? throw new ArgumentNullException("OmdbApiKey is missing.");
            _omdbUrl = configuration["OmdbApiBaseUrl"] ?? throw new ArgumentNullException("OmdbApiBaseUrl is missing.");
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_omdbUrl);
        }

        public async Task<MovieSearchResult> SearchMoviesAsync(string query)
        {
            return await _httpClient.GetFromJsonAsync<MovieSearchResult>($"?s={Uri.EscapeDataString(query)}&apikey={_omdbApiKey}");
        }

        public async Task<Movie> GetMovieDetailsAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Movie>($"?i={id}&apikey={_omdbApiKey}");
        }
    }
}
