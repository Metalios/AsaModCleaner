using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class ModDetails
    {
        [JsonProperty("iD")]
        public int Id { get; set; }

        [JsonProperty("gameId")]
        public int GameId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("slug")]
        public string? Slug { get; set; }

        [JsonProperty("summary")]
        public string? Summary { get; set; }

        [JsonProperty("links")]
        public ModLinks? Links { get; set; }
    }
}
