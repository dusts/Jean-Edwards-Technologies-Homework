using Blazored.LocalStorage;
using SharedModels;
using System.Text.Json;

namespace Blazor_WebAssembly_App.ApiServices
{
    public class BlazorBackendApiServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BlazorBackendApiServices(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("BlazorBackendApi");
            _localStorage = localStorage;
        }

        public async Task<Movie> GetMovieByIdAsync(string imdbId)
        {
            var response = await _httpClient.GetAsync($"api/omdb/{imdbId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Backend Response: {json}");
            var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (movie?.Response == "True")
            {
                await SaveLastApiQueryAsync(movie.QueryWithoutApiKey);
                return movie;
            }
            throw new Exception($"API Error: {movie?.Error ?? "Unknown error"}");
        }

        public async Task<Movie> GetMovieByTitleAsync(string imdbTitle)
        {
            var response = await _httpClient.GetAsync($"api/omdb/title/{imdbTitle}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Backend Response: {json}");
            var movie = JsonSerializer.Deserialize<Movie>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (movie?.Response == "True")
            {
                await SaveLastApiQueryAsync(movie.QueryWithoutApiKey);
                return movie;
            }
            throw new Exception($"API Error: {movie?.Error ?? "Unknown error"}");
        }

        // Search movies by title
        public async Task<List<Movie>> SearchMoviesAsync(string title)
        {
            var response = await _httpClient.GetAsync($"api/omdb/search/{Uri.EscapeDataString(title)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var movies = JsonSerializer.Deserialize<List<Movie>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<Movie>();

            if(movies.Count > 0)
            {
                await SaveLastApiQueryAsync(movies[0].QueryWithoutApiKey);
            }

            return movies;
        }

        public async Task SaveLastApiQueryAsync(string movieQuery)
        {
            var lastQueries = await GetLastApiQueriesAsync();
            if (!lastQueries.Any(m => m == movieQuery))
            {
                lastQueries.Add(movieQuery);
                if(lastQueries.Count > 5)
                {
                    lastQueries.RemoveAt(0); // removes first entry (to maintain 5 movie search list thing
                }
                await _localStorage.SetItemAsync("LastQueries", lastQueries);
            }
        }

        public async Task<List<string>> GetLastApiQueriesAsync()
        {
            var lastQueries = await _localStorage.GetItemAsync<List<string>>("LastQueries");
            return lastQueries ?? new List<string>();
        }
    }
}