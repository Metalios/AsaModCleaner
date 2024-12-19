using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class InstalledMod : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        [JsonProperty("installedFile")]
        public ModFile? InstalledFile { get; set; }

        [JsonProperty("latestUpdatedFile")]
        public ModFile? LatestUpdatedFile { get; set; }

        [JsonProperty("dynamicContent")]
        public bool DynamicContent { get; set; }

        public string DateInstalledLocal
        {
            get
            {
                if (DateTime.TryParseExact(DateInstalled, "yyyy.MM.dd-HH.mm.ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out var dateTime))
                {
                    // Convert to local time and return just the date in the user's local format
                    return dateTime.ToLocalTime().ToString("G"); // "D" for long date pattern
                }
                return DateInstalled ?? "Unknown";
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        // Notify UI about property changes
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
