using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class DiscountData
    {
        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }

        [JsonProperty("percent")]
        public int Percent { get; set; }

        [JsonProperty("endDate")]
        public string? EndDate { get; set; }

        [JsonProperty("platformData")]
        public PlatformData? PlatformData { get; set; }
    }
}
