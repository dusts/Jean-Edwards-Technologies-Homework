using SharedModels;
using SharedModels.Services;
using System.Text.Json;

namespace Blazor_WebAssembly_App.ApiServices
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlazorBackendApi");
        }

        public async Task<Movie> GetMovieDetailsAsync(string imdbId)
        {
            var response = await _httpClient.GetAsync($"api/movies/details/{imdbId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Backend Response: {json}");
            var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (movie?.Response == "True")
            {
                return movie;
            }
            throw new Exception($"API Error: {movie?.Error ?? "Unknown error"}");
        }

        public async Task<MovieSearchResult> SearchMoviesAsync(string query)
        {
            var response = await _httpClient.GetAsync($"api/movies/search?query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var movieSearchResult = JsonSerializer.Deserialize<MovieSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new MovieSearchResult();

            return movieSearchResult;
        }
    }
}