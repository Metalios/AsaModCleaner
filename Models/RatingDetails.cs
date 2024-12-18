using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class RatingDetails
    {
        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("totalRatings")]
        public int TotalRatings { get; set; }

        [JsonProperty("positiveRatings")]
        public int PositiveRatings { get; set; }

        [JsonProperty("score")]
        public string? Score { get; set; }
    }
}
