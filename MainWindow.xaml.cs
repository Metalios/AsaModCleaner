using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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
        private readonly Queue<InstalledMod> _modsToCleanQueue = new();
        
        private string? _selectedSortCriterion = "InstallDate"; // Default criterion
        private ListSortDirection _selectedSortDirection = ListSortDirection.Ascending; // Default direction

        private ObservableCollection<InstalledMod> InstalledMods { get; set; } = [];
        private readonly ISettingsService _settingsService;

        public MainWindow(GameService gameService, ISettingsService settingsService, ILogger<MainWindow> logger)
        {
            _gameService = gameService;
            _logger = logger;
            _settingsService = settingsService;
            InitializeComponent();

            // Subscribe to the Loaded event
            Loaded += MainWindow_Loaded;
        }

        private void EnableButtons(bool enabled)
        {
            SelectAllButton.IsEnabled = enabled;
            RefreshButton.IsEnabled = enabled;
            CleanButton.IsEnabled = enabled;
        }

        // Loaded Event Handler
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyWindowSettings();
                EnableButtons(false);

                // Run the initialization logic in a separate thread to avoid blocking the UI
                var success = await Task.Run(() => _gameService.Initialize());

                if (success)
                {
                    // Proceed with the follow-up task if initialization was successful
                    LoadModList();
                    EnableButtons(true);
                }
                else
                {
                    // Handle the scenario where initialization failed
                    MessageBox.Show("Initialization failed. Please ensure Steam is running and ARK is installed.", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyWindowSettings()
        {
            var settings = _settingsService.LoadWindowSettings();

            // Apply position and size if not maximized
            if (settings.IsMaximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = settings.Left;
                Top = settings.Top;
                Width = settings.Width;
                Height = settings.Height;
            }
        }

        // Refresh Button Click Event Handler
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadModList();
            ScrollToTop(ModList);
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mod in InstalledMods)
            {
                mod.IsSelected = true; // Set each mod's IsSelected property to true
            }
        }

        // Clean Button Click Event Handler
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            // Get all selected mods from the InstalledMods collection
            var selectedMods = InstalledMods.Where(mod => mod.IsSelected).ToList();

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

        private async void ProcessCleaningQueue()
        {
            try
            {
                // Show the progress bar and reset its value
                CleaningProgressBar.Visibility = Visibility.Visible;
                CleaningProgressBar.Minimum = 0;
                CleaningProgressBar.Maximum = _modsToCleanQueue.Count;
                CleaningProgressBar.Value = 0;

                var totalMods = _modsToCleanQueue.Count;
                var processedMods = 0;

                var installDir = _gameService.GetArkInstallDir();
                var modPath = _gameService.GetModsPath(installDir!);

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

                        // Remove the mod from the ObservableCollection
                        InstalledMods.Remove(modToClean);

                        // Save changes to the library
                        var changedLibrary = new Library { InstalledMods = InstalledMods.ToList() };
                        _gameService.SaveModLibraryChanges(changedLibrary, installDir!);

                        // Update the progress bar
                        processedMods++;
                        CleaningProgressBar.Value = processedMods;

                        // Optional: Update status or show feedback
                        StatusLabel.Text =
                            $"{modToClean.Details?.Name} cleaned successfully. ({processedMods}/{totalMods})";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to delete {modToClean.Details?.Name}: {ex.Message}");
                    }

                    // Adding a small delay to visually represent progress
                    await Task.Delay(100); // Simulate delay to show progress, adjust or remove in real implementation
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the cleaning queue: {ex.Message}");
                MessageBox.Show($"An error occurred while processing the queue: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Once the queue is processed, hide the progress bar
                CleaningProgressBar.Visibility = Visibility.Collapsed;
                StatusLabel.Text = "Cleaning process completed.";
            }
        }

        private string FindFolderRecursively(string rootPath, string targetPath)
        {
            try
            {
                return Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                    .FirstOrDefault(dir => dir.Equals(targetPath, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while searching for folder: {ex.Message}");
                return string.Empty;
            }
        }

        private void LoadModList()
        {
            var installDir = _gameService.GetArkInstallDir();
            if (string.IsNullOrWhiteSpace(installDir)) return;

            var modLibrary = _gameService.DeserializeModLibrary(installDir);
            if (modLibrary?.InstalledMods == null) return;

            InstalledMods.Clear();
            foreach (var mod in modLibrary.InstalledMods)
            {
                mod.IsSelected = false; // Reset selections
                InstalledMods.Add(mod);
            }

            ModList.ItemsSource = InstalledMods; // Bind ObservableCollection
            StatusLabel.Text = $"{InstalledMods.Count} mods loaded.";
        }

        private static void ScrollToTop(ListView listView)
        {
            if (VisualTreeHelper.GetChildrenCount(listView) <= 0) return;
            var border = (Border?)VisualTreeHelper.GetChild(listView, 0);
            if (border is null || VisualTreeHelper.GetChildrenCount(border) <= 0) return;
            var scrollViewer = (ScrollViewer?)VisualTreeHelper.GetChild(border, 0);
            scrollViewer?.ScrollToTop(); // Ensure the scrollbar starts at the top
        }

        private void SortCriteriaDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortCriteriaDropdown.SelectedItem is not ComboBoxItem selectedItem) return;
            _selectedSortCriterion = selectedItem.Tag.ToString();
            ApplySorting();
        }

        private void SortDirectionDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortDirectionDropdown.SelectedItem is not ComboBoxItem selectedItem) return;
            _selectedSortDirection = selectedItem.Tag.ToString() == "Ascending"
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            ApplySorting();
        }

        private void ApplySorting()
        {
            var collectionView = (CollectionView)CollectionViewSource.GetDefaultView(InstalledMods);
            collectionView.SortDescriptions.Clear();

            // Apply sorting based on selected criterion and direction
            switch (_selectedSortCriterion)
            {
                case "InstallDate":
                    collectionView.SortDescriptions.Add(new SortDescription(nameof(InstalledMod.DateInstalled), _selectedSortDirection));
                    break;

                case "Name":
                    collectionView.SortDescriptions.Add(new SortDescription("Details.Name", _selectedSortDirection));
                    break;

                case "Free":
                    collectionView.SortDescriptions.Add(new SortDescription(nameof(InstalledMod.Details.PremiumDetails.IsPremium), ListSortDirection.Ascending));
                    break;

                case "Premium":
                    collectionView.SortDescriptions.Add(new SortDescription(nameof(InstalledMod.Details.PremiumDetails.IsPremium), ListSortDirection.Descending));
                    break;

                case "Author":
                    collectionView.SortDescriptions.Add(new SortDescription("Details.Authors[0].Name", _selectedSortDirection));
                    break;
            }
        }

        // Save window settings when the application closes
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var settings = new WindowSettings
            {
                Left = Left,
                Top = Top,
                Width = Width,
                Height = Height,
                IsMaximized = WindowState == WindowState.Maximized
            };

            _settingsService.SaveWindowSettings(settings);
        }
    }
}
