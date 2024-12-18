using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class ModLinks
    {
        [JsonProperty("websiteUrl")]
        public string? WebsiteUrl { get; set; }

        [JsonProperty("wikiUrl")]
        public string? WikiUrl { get; set; }

        [JsonProperty("issuesUrl")]
        public string? IssuesUrl { get; set; }

        [JsonProperty("sourceUrl")]
        public string? SourceUrl { get; set; }

        [JsonProperty("modManagementUrl")]
        public string? ModManagementUrl { get; set; }
    }
}
