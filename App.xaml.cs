using Microsoft.Extensions.Hosting;
using System.Windows;
using AsaModCleaner.Handlers;
using AsaModCleaner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AsaModCleaner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            // Determine the environment
            var environment = IsDebug() ? "Development" : "Production";

            // Set up the logger first, based on the environment
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"logs\\{environment}_application_log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog() // Integrate Serilog into the Host for logging
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<ISteamHandler, SteamHandler>(); // Register the SteamHandler for use via ISteamHandler
                    services.AddSingleton<GameService>(); // Register GameService
                    services.AddHostedService<SteamHandler>(); // Register SteamHandler as a hosted background service
                    services.AddSingleton<MainWindow>(); // Register your main window with DI
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Show the main window using DI
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            await Log.CloseAndFlushAsync(); // Make sure logs are flushed before exit
            base.OnExit(e);
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
