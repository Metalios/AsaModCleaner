using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class Module
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("fingerprint")]
        public long Fingerprint { get; set; }
    }
}
