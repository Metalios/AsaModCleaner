using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steamworks;

namespace AsaModCleaner.Handlers
{
    public class SteamHandler : BackgroundService, ISteamHandler
    {
        private readonly ILogger<SteamHandler> _logger;
        private readonly bool _isInitialized;
        private readonly AppId_t _appId;

        public SteamHandler(IConfiguration configuration, ILogger<SteamHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Retrieve the AppId from the configuration
            var appId = configuration.GetValue<uint>("SteamSettings:AppId");
            _appId = new AppId_t(appId);

            try
            {
                // Initialize Steam using the provided AppId
                Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());

                if (!SteamAPI.Init())
                {
                    throw new Exception("SteamAPI initialization failed. Steam client must be running to initialize Steamworks.");
                }

                _isInitialized = true;
                _logger.LogInformation("Steam initialized successfully with AppId {AppId}", appId);
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                _logger.LogCritical(ex, "Failed to initialize Steam");
                throw; // Re-throw to ensure this is caught appropriately by the caller
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SteamHandler is starting the callback loop.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Run Steam callbacks periodically
                    SteamAPI.RunCallbacks();
                    await Task.Delay(100, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while running Steam callbacks.");
                }

                await Task.Delay(100, stoppingToken); // Adjust the delay as necessary
            }

            _logger.LogInformation("SteamHandler callback loop is stopping.");
        }

        public bool IsInitialized() => _isInitialized;

        public AppId_t GetAppId() => _appId;
        
        public override void Dispose()
        {
            base.Dispose();
            if (!_isInitialized) return;
            SteamAPI.Shutdown();
            _logger.LogInformation("Steam has been shut down.");
        }
    }
}
