using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class FileModule
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("fingerprint")]
        public uint Fingerprint { get; set; }
    }
}
