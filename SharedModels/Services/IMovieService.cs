namespace SharedModels.Services
{
    public interface IMovieService
    {
        Task<MovieSearchResult> SearchMoviesAsync(string query);
        Task<Movie> GetMovieDetailsAsync(string id);
    }
}
