using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class InstalledMod
    {
        [JsonProperty("dateInstalled")]
        public string? DateInstalled { get; set; }

        [JsonProperty("dateUpdated")]
        public string? DateUpdated { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("pathOnDisk")]
        public string? PathOnDisk { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("details")]
        public ModDetails? Details { get; set; }

        public string DateInstalledLocal
        {
            get
            {
                if (DateTime.TryParseExact(DateInstalled, "yyyy.MM.dd-HH.mm.ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out var dateTime))
                {
                    // Convert to local time and return just the date in the user's local format
                    return dateTime.ToLocalTime().ToString("d"); // "D" for long date pattern
                }
                return DateInstalled ?? "Unknown";
            }
        }

        public bool IsSelected { get; set; }
    }
}
