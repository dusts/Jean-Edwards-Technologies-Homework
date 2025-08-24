using System.Text.Json.Serialization;

namespace SharedModels
{
    public class MovieSearchResult
    {
        public List<MovieSummary> Search { get; set; } = new List<MovieSummary>();
        public string TotalResults { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class MovieSummary
    {
        public string ImdbID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
