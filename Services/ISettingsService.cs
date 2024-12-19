using AsaModCleaner.Models;

namespace AsaModCleaner.Services;

public interface ISettingsService
{
    WindowSettings LoadWindowSettings();
    void SaveWindowSettings(WindowSettings settings);
}