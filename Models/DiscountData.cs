using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class DiscountData
    {
        [JsonProperty("discountPrice")]
        public double DiscountPrice { get; set; }

        [JsonProperty("percent")]
        public double Percent { get; set; }

        [JsonProperty("endDate")]
        public string? EndDate { get; set; }
    }
}
