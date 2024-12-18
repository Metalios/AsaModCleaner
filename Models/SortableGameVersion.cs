using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class SortableGameVersion
    {
        [JsonProperty("gameVersionName")]
        public string? GameVersionName { get; set; }

        [JsonProperty("gameVersionPadded")]
        public string? GameVersionPadded { get; set; }

        [JsonProperty("gameVersion")]
        public string? GameVersion { get; set; }

        [JsonProperty("gameVersionReleaseDate")]
        public string? GameVersionReleaseDate { get; set; }

        [JsonProperty("gameVersionTypeId")]
        public int GameVersionTypeId { get; set; }
    }
}
