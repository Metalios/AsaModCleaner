using System.Diagnostics;
using System.IO;
using AsaModCleaner.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steamworks;

namespace AsaModCleaner.Services
{
    public class GameService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GameService> _logger;

        private readonly AppId_t _appId;

        private string? _installDir;

        public GameService(IConfiguration configuration, ILogger<GameService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var appId = configuration.GetValue<uint>("SteamSettings:AppId");
            _appId = new AppId_t(appId);
        }

        public bool Initialize()
        {
            try
            {
                // Set the Steam App ID environment variable
                Environment.SetEnvironmentVariable("SteamAppId", _appId.ToString());

                // Attempt to initialize Steam API
                if (!SteamAPI.Init())
                {
                    _logger.LogError("SteamAPI initialization failed. Steam client must be running to initialize Steamworks.");
                    return false; // Return false if initialization fails
                }

                // Check if ARK is installed
                var isInstalled = SteamApps.BIsAppInstalled(_appId);
                if (!isInstalled)
                {
                    _logger.LogError("ARK: Survival Ascended is not installed.");
                    return false; // Return false if ARK is not installed
                }

                // Attempt to get the installation directory
                const int bufferSize = 260; // Standard buffer size for Windows file paths
                if (SteamApps.GetAppInstallDir(_appId, out var installDir, bufferSize) <= 0)
                {
                    _logger.LogWarning("Unable to determine the installation directory for ARK: Survival Ascended, although it appears to be installed.");
                    return false; // Return false if we can't determine the installation directory
                }

                _installDir = installDir;
                _logger.LogInformation("Successfully initialized ARK: Survival Ascended with installation directory at: {InstallDir}", _installDir);

                return true; // Return true if everything succeeds
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to initialize SteamAPI: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during SteamAPI initialization: {Message}", ex.Message);
            }
            finally
            {
                // Ensure Steam API is properly shut down
                SteamAPI.Shutdown();
            }

            return false; // Return false if any exception occurred
        }

        private string GetModsUserDataPath(string installDir)
        {
            return Path.Combine(installDir, "ShooterGame", "Binaries", "Win64", "ShooterGame", "ModsUserData");
        }

        public string GetModsPath(string installDir)
        {
            return Path.Combine(installDir, "ShooterGame", "Binaries", "Win64", "ShooterGame", "Mods");
        }

        public bool IsArkRunning()
        {
            // Process names without the .exe extension
            string[] processNames = ["ArkAscended", "ArkAscended_BE"];

            try
            {
                if (processNames.Select(Process.GetProcessesByName).Any(processes => processes.Length > 0))
                {
                    _logger.LogWarning("ARK: Survival Ascended is currently running. Please exit the game before continuing.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if ARK: Survival Ascended is running.");
            }

            return false;
        }

        public string? GetArkInstallDir() => _installDir;

        public Library? DeserializeModLibrary(string installDir)
        {
            try
            {
                var modUserData = GetModsUserDataPath(installDir);
                // Get all library.json files in the specified directory and its subdirectories
                var files = Directory.GetFiles(modUserData, "library.json", SearchOption.AllDirectories);
        
                if (files.Length == 0)
                {
                    _logger.LogWarning("No library.json file found in directory: {modUserData}", modUserData);
                    return null;
                }

                // Read the content of the first found library.json file
                var libraryFile = files[0];
                var jsonContent = File.ReadAllText(libraryFile);

                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    NullValueHandling = NullValueHandling.Include
                };

                var modLibrary = JsonConvert.DeserializeObject<Library>(jsonContent, settings);

                return modLibrary;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deserializing the mod library from directory: {InstallDir}", installDir);
                throw;
            }
        }

        public void SaveModLibraryChanges(Library modLibrary, string installDir)
        {
            try
            {
                var modUserData = GetModsUserDataPath(installDir);
                // Get all library.json files in the specified directory and its subdirectories
                var files = Directory.GetFiles(modUserData, "library.json", SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    _logger.LogWarning("No library.json file found in directory: {modUserData}", modUserData);
                    return;
                }

                // Read the content of the first found library.json file
                var libraryFile = files[0];
                var serializedData = JsonConvert.SerializeObject(modLibrary);
                File.WriteAllText(libraryFile, serializedData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, @"An error occurred while serializing the mod library");
                throw;
            }
        }
    }
}
