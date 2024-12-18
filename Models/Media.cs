using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class Media
    {
        [JsonProperty("iD")]
        public int Id { get; set; }

        [JsonProperty("modId")]
        public int ModId { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string? ThumbnailUrl { get; set; }

        [JsonProperty("uRL")]
        public string? Url { get; set; }
    }
}
