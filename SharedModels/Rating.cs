using System.Text.Json.Serialization;

namespace SharedModels
{
    public class Rating
    {
        [JsonPropertyName("Source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;
    }
}
