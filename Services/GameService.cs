using System.Diagnostics;
using System.IO;
using AsaModCleaner.Handlers;
using AsaModCleaner.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steamworks;

namespace AsaModCleaner.Services
{
    public class GameService
    {
        private readonly ISteamHandler _steamHandler;
        private readonly ILogger<GameService> _logger;

        public GameService(ISteamHandler steamHandlerStatus, ILogger<GameService> logger)
        {
            _steamHandler = steamHandlerStatus;
            _logger = logger;
        }

        public bool IsArkInstalled()
        {
            if (!_steamHandler.IsInitialized())
            {
                _logger.LogWarning("SteamHandler is not initialized. Cannot perform game operations.");
                return false;
            }

            var appId = _steamHandler.GetAppId();

            // Check if ARK: Survival Ascended is installed
            var isInstalled = SteamApps.BIsAppInstalled(appId);
            if (!isInstalled)
            {
                _logger.LogWarning("ARK: Survival Ascended is not installed.");
            }

            return isInstalled;
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

        public string? GetArkInstallDir()
        {
            if (!_steamHandler.IsInitialized())
            {
                _logger.LogWarning("SteamHandler is not initialized. Cannot perform game operations.");
                return null;
            }

            try
            {
                var appId = _steamHandler.GetAppId();

                // Attempt to get the installation directory
                const int bufferSize = 260; // Standard buffer size for Windows file paths
                if (SteamApps.GetAppInstallDir(appId, out var installDir, bufferSize) > 0)
                {
                    return installDir;
                }

                _logger.LogWarning("Unable to determine the installation directory for ARK: Survival Ascended, although it appears to be installed.");
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogWarning(ioe, "Operation failed: {Message}", ioe.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred while attempting to retrieve the installation directory.");
            }

            return null;
        }

        public Library? DeserializeModLibrary(string installDir)
        {
            try
            {
                var modPath = Path.Combine(installDir, "ShooterGame", "Binaries", "Win64", "ShooterGame", "ModsUserData");
                // Get all library.json files in the specified directory and its subdirectories
                var files = Directory.GetFiles(modPath, "library.json", SearchOption.AllDirectories);
        
                if (files.Length == 0)
                {
                    _logger.LogWarning("No library.json file found in directory: {modPath}", modPath);
                    return null;
                }

                // Read the content of the first found library.json file
                var libraryFile = files[0];
                var jsonContent = File.ReadAllText(libraryFile);
                var library = JsonConvert.DeserializeObject<Library>(jsonContent);

                if (library == null)
                {
                    _logger.LogWarning("Deserialization of library.json returned null. File may be empty or malformed: {LibraryFile}", libraryFile);
                }

                return library;
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
                var modPath = Path.Combine(installDir, "ShooterGame", "Binaries", "Win64", "ShooterGame", "ModsUserData");
                // Get all library.json files in the specified directory and its subdirectories
                var files = Directory.GetFiles(modPath, "library.json", SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    _logger.LogWarning("No library.json file found in directory: {modPath}", modPath);
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
