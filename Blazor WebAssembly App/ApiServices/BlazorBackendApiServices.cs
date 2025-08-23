using SharedModels;
using System.Text.Json;

namespace Blazor_WebAssembly_App.ApiServices
{
    public class BlazorBackendApiServices
    {
        private readonly HttpClient _httpClient;

        public BlazorBackendApiServices(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlazorBackendApi");
        }

        public async Task<Movie> GetMovieByIdAsync(string imdbId)
        {
            var response = await _httpClient.GetAsync($"api/omdb/{imdbId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Backend Response: {json}");
            var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (movie?.Response == "True")
                return movie;
            throw new Exception($"API Error: {movie?.Error ?? "Unknown error"}");
        }
    }
}