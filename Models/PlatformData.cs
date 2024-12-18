using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class PlatformData
    {
        [JsonProperty("productId")]
        public string? ProductId { get; set; }
    }
}
