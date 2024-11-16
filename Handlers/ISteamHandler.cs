using Steamworks;

namespace AsaModCleaner.Handlers
{
    public interface ISteamHandler
    {
        bool IsInitialized();
        AppId_t GetAppId();
    }
}
