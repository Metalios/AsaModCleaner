using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class Hash
    {
        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("algo")]
        public string? Algorithm { get; set; }
    }
}
