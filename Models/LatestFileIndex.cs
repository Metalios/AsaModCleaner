using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class LatestFileIndex
    {
        [JsonProperty("gameVersion")]
        public string? GameVersion { get; set; }

        [JsonProperty("fileId")]
        public int FileId { get; set; }

        [JsonProperty("filename")]
        public string? Filename { get; set; }

        [JsonProperty("releaseType")]
        public string? ReleaseType { get; set; }

        [JsonProperty("gameVersionTypeId")]
        public int GameVersionTypeId { get; set; }

        [JsonProperty("modLoader")]
        public string? ModLoader { get; set; }
    }
}
