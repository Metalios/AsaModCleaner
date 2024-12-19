using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class FileHash
    {
        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("algo")]
        public string? Algo { get; set; }
    }
}
