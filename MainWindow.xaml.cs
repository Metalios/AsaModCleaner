using System.IO;
using System.Windows;
using System.Windows.Controls;
using AsaModCleaner.Models;
using AsaModCleaner.Services;
using Microsoft.Extensions.Logging;

namespace AsaModCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly GameService _gameService;
        private readonly ILogger<MainWindow> _logger;
        private readonly Queue<InstalledMod> _modsToCleanQueue = new Queue<InstalledMod>();


        public MainWindow(GameService gameService, ILogger<MainWindow> logger)
        {
            _gameService = gameService;
            _logger = logger;
            InitializeComponent();

            // Subscribe to the Loaded event
            Loaded += MainWindow_Loaded;
        }

        // Loaded Event Handler
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadModList();
        }

        // Refresh Button Click Event Handler
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadModList();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.ItemsSource is not List<InstalledMod> installedMods) return;
            foreach (var mod in installedMods)
            {
                mod.IsSelected = true; // Set each mod's IsSelected property to true
            }

            // Refresh the ListView to reflect the changes
            ModList.ItemsSource = null; // Disconnect the ItemsSource temporarily
            ModList.ItemsSource = installedMods; // Re-assign to refresh the UI
        }

        // Clean Button Click Event Handler
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            // Get all selected items from the ListView
            var selectedMods = ModList.SelectedItems.Cast<InstalledMod>().ToList();

            if (selectedMods.Count == 0)
            {
                MessageBox.Show("Please select at least one mod to clean.");
                return;
            }

            // Add selected mods to the cleaning queue
            foreach (var mod in selectedMods)
            {
                _modsToCleanQueue.Enqueue(mod);
            }

            // Start processing the cleaning queue
            ProcessCleaningQueue();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox { DataContext: InstalledMod selectedMod }) return;
            // Add the selected item to the ListView's SelectedItems if not already present
            if (!ModList.SelectedItems.Contains(selectedMod))
            {
                ModList.SelectedItems.Add(selectedMod);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox { DataContext: InstalledMod selectedMod }) return;
            // Remove the item from the ListView's SelectedItems when unchecked
            if (ModList.SelectedItems.Contains(selectedMod))
            {
                ModList.SelectedItems.Remove(selectedMod);
            }
        }

        private async void ProcessCleaningQueue()
        {
            // Show the progress bar and reset its value
            CleaningProgressBar.Visibility = Visibility.Visible;
            CleaningProgressBar.Minimum = 0;
            CleaningProgressBar.Maximum = _modsToCleanQueue.Count;
            CleaningProgressBar.Value = 0;

            var totalMods = _modsToCleanQueue.Count;
            var processedMods = 0;

            var installDir = _gameService.GetArkInstallDir(); 
            var modPath = Path.Combine(installDir!, "ShooterGame", "Binaries", "Win64", "ShooterGame", "Mods");

            while (_modsToCleanQueue.Count > 0)
            {
                // Dequeue the next mod from the queue
                var modToClean = _modsToCleanQueue.Dequeue();

                try
                {
                    // Find the folder corresponding to PathOnDisk in modPath recursively
                    if (!string.IsNullOrWhiteSpace(modToClean.PathOnDisk) && Directory.Exists(modPath))
                    {
                        var folderToDelete = FindFolderRecursively(modPath, modToClean.PathOnDisk);

                        if (!string.IsNullOrEmpty(folderToDelete) && Directory.Exists(folderToDelete))
                        {
                            // Delete the folder and its contents
                            Directory.Delete(folderToDelete, true);
                        }
                    }

                    // Remove the mod from the InstalledMods list and update the UI
                    var installedMods = (List<InstalledMod>)ModList.ItemsSource!;
                    installedMods.Remove(modToClean);

                    var changedLibrary = new Library{InstalledMods = installedMods};
                    _gameService.SaveModLibraryChanges(changedLibrary, installDir!);

                    // Refresh the ListView to reflect changes
                    ModList.ItemsSource = null; // Disconnect the binding temporarily
                    ModList.ItemsSource = installedMods;

                    // Update the progress bar
                    processedMods++;
                    CleaningProgressBar.Value = processedMods;

                    // Optional: Update status or show feedback
                    StatusLabel.Text = $"{modToClean.Details?.Name} cleaned successfully. ({processedMods}/{totalMods})";
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to delete {modToClean.Details?.Name}: {ex.Message}");
                }

                // Adding a small delay to visually represent progress
                await Task.Delay(100); // Simulate delay to show progress, adjust or remove in real implementation
            }

            // Once the queue is processed, hide the progress bar
            CleaningProgressBar.Visibility = Visibility.Collapsed;
            StatusLabel.Text = "Cleaning process completed.";
        }

        private string FindFolderRecursively(string rootPath, string targetPath)
        {
            try
            {
                // Get all subdirectories in the root path
                foreach (var directory in Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories))
                {
                    if (directory.Equals(targetPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // Return the path if it matches the target path
                        return directory;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while searching for folder: {ex.Message}");
            }

            // Return empty string if not found
            return string.Empty;
        }

        private void LoadModList()
        {
            var installed = _gameService.IsArkInstalled();
            if (!installed)
            {
                MessageBox.Show("ARK: Survival Ascended is not installed");
                return;
            }

            var installDir = _gameService.GetArkInstallDir();
            if (string.IsNullOrWhiteSpace(installDir)) return;
            var modLibrary = _gameService.DeserializeModLibrary(installDir);
            if (modLibrary is null) return;
            var installedMods = modLibrary.InstalledMods;

            foreach (var mod in installedMods!)
            {
                mod.IsSelected = false; // Clear all selections
            }

            // Update the ListView with the refreshed list
            ModList.ItemsSource = null; // Disconnect the ItemsSource temporarily to refresh the UI
            ModList.ItemsSource = installedMods; // Re-assign to refresh the ListView

            // Optional: Update status
            StatusLabel.Text = $"{installedMods?.Count} mods loaded.";
        }
    }
}
