using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class Author
    {
        [JsonProperty("iD")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("uRL")]
        public string? Url { get; set; }
    }
}
