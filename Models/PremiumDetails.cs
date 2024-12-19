using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class PremiumDetails
    {
        [JsonProperty("isPremium")]
        public bool IsPremium { get; set; }

        [JsonProperty("isFreemium")]
        public bool IsFreemium { get; set; }

        [JsonProperty("tierPrice")]
        public decimal TierPrice { get; set; }

        [JsonProperty("currencySymbol")]
        public string? CurrencySymbol { get; set; }

        [JsonProperty("platformData")]
        public PlatformData? PlatformData { get; set; }

        [JsonProperty("discountData")]
        public DiscountData? DiscountData { get; set; }
    }
}
