using System.IO;
using System.Text.Json;
using AsaModCleaner.Models;
using Microsoft.Extensions.Configuration;

namespace AsaModCleaner.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IConfiguration _configuration;

        public SettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public WindowSettings LoadWindowSettings()
        {
            // Create a new instance of WindowSettings for binding
            var settings = new WindowSettings();

            // Bind the "WindowSettings" section to the WindowSettings object
            var section = _configuration.GetSection("WindowSettings");
            section.Bind(settings);

            return settings; // Return the bound settings object
        }

        public void SaveWindowSettings(WindowSettings settings)
        {
            // Determine the appropriate file (environment-specific or default)
            var environment = IsDebug() ? "Development" : "Production";
            var appSettingsFile = environment == "Production" ? "appsettings.json" : $"appsettings.{environment}.json";

            // Load existing JSON content
            var json = File.ReadAllText(appSettingsFile);
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            // Update the "WindowSettings" section
            if (jsonObject == null) return;
            jsonObject["WindowSettings"] = settings;

            // Save the updated JSON back to the file
            File.WriteAllText(appSettingsFile,
                JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
