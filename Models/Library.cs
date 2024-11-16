using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class Library
    {
        [JsonProperty("installedMods")]
        public List<InstalledMod>? InstalledMods { get; set; }
    }
}
