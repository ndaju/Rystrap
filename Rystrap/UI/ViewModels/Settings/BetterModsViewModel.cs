using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

using Rystrap.Models.Mods;
using Rystrap.Mods;
using Rystrap.UI.Elements.Dialogs;

namespace Rystrap.UI.ViewModels.Settings
{
    public class BetterModsViewModel : NotifyPropertyChangedViewModel
    {
        private BetterModManager _modManager = null!;

        public ICommand ToggleModCommand => new RelayCommand<Models.Mods.ModEntry>(ToggleMod);
        public ICommand BrowseCustomFileCommand => new RelayCommand<Models.Mods.ModEntry>(BrowseCustomFile);
        public ICommand EnableAllInCategoryCommand => new RelayCommand(EnableAllInCategory);
        public ICommand DisableAllInCategoryCommand => new RelayCommand(DisableAllInCategory);
        public ICommand ExportPackCommand => new RelayCommand(ExportPack);
        public ICommand ImportPackCommand => new RelayCommand(ImportPack);
        public ICommand ApplyModsCommand => new RelayCommand(ApplyMods);
        public ICommand ResetModsCommand => new RelayCommand(ResetMods);

        public ObservableCollection<Models.Mods.ModCategory> Categories { get; } = new();
        public ObservableCollection<Models.Mods.ModEntry> DisplayedMods { get; } = new();

        private Models.Mods.ModCategory _selectedCategory = ModCategory.Cursor;
        public Models.Mods.ModCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    RefreshDisplayedMods();
                }
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    RefreshDisplayedMods();
                }
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                    OnPropertyChanged(nameof(StatusVisibility));
                    OnPropertyChanged(nameof(StatusForeground));
                }
            }
        }

        private string _statusForeground = "White";
        public string StatusForeground
        {
            get => _statusForeground;
            set
            {
                if (_statusForeground != value)
                {
                    _statusForeground = value;
                    OnPropertyChanged(nameof(StatusForeground));
                }
            }
        }

        public Visibility StatusVisibility => string.IsNullOrEmpty(StatusMessage) ? Visibility.Collapsed : Visibility.Visible;

        private string _modCountText = "";
        public string ModCountText
        {
            get => _modCountText;
            set
            {
                if (_modCountText != value)
                {
                    _modCountText = value;
                    OnPropertyChanged(nameof(ModCountText));
                }
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public BetterModsViewModel()
        {
            Categories.Add(ModCategory.Cursor);
            Categories.Add(ModCategory.Sound);
            Categories.Add(ModCategory.Texture);
            Categories.Add(ModCategory.Font);
            Categories.Add(ModCategory.Animation);
            Categories.Add(ModCategory.UI);
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                _modManager = new BetterModManager();
                _modManager.LoadBuiltInMods();
                _modManager.LoadCustomMods();
                _modManager.LoadEnabledState();
            });

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RefreshDisplayedMods();
                IsLoading = false;
            }));
        }

        public void RefreshDisplayedMods()
        {
            if (_modManager is null)
                return;

            DisplayedMods.Clear();

            List<Models.Mods.ModEntry> mods;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                mods = _modManager.GetAllMods().Where(m =>
                    m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    m.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                mods = _modManager.GetModsByCategory(SelectedCategory);
            }

            foreach (var mod in mods)
                DisplayedMods.Add(mod);

            UpdateModCount();
        }

        private void UpdateModCount()
        {
            if (_modManager is null)
                return;

            int enabled = _modManager.GetEnabledCount();
            int total = _modManager.GetTotalCount();
            ModCountText = $"{enabled} of {total} mods enabled";
        }

        private void ShowStatus(string message, bool isError = false)
        {
            StatusMessage = message;
            StatusForeground = isError ? "#FF6B6B" : "#4ECB71";
        }

        private void ToggleMod(Models.Mods.ModEntry? mod)
        {
            if (mod is null || _modManager is null)
                return;

            try
            {
                if (!mod.IsEnabled && string.IsNullOrEmpty(mod.CustomFilePath))
                {
                    ShowStatus($"You must click Browse to modify \"{mod.Name}\" before enabling it", true);
                    RefreshDisplayedMods();
                    return;
                }

                _modManager.ToggleMod(mod.Id);

                bool isEnabled = _modManager.GetModById(mod.Id)?.IsEnabled ?? false;
                ShowStatus($"{(isEnabled ? "Enabled" : "Disabled")} mod: {mod.Name}", false);

                RefreshDisplayedMods();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to toggle mod: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::ToggleMod", ex);
            }
        }

        private void BrowseCustomFile(Models.Mods.ModEntry? mod)
        {
            if (mod is null || _modManager is null)
                return;

            string filter = mod.Category switch
            {
                ModCategory.Sound => "Audio Files (*.mp3;*.ogg;*.wav)|*.mp3;*.ogg;*.wav|All Files (*.*)|*.*",
                ModCategory.Cursor => "Image Files (*.png;*.bmp;*.cur)|*.png;*.bmp;*.cur|All Files (*.*)|*.*",
                ModCategory.Texture => "Texture Files (*.png;*.jpg;*.bmp;*.tga)|*.png;*.jpg;*.bmp;*.tga|All Files (*.*)|*.*",
                ModCategory.Font => "Font Files (*.ttf;*.otf)|*.ttf;*.otf|All Files (*.*)|*.*",
                ModCategory.Animation => "Animation Files (*.rbxanim)|*.rbxanim|All Files (*.*)|*.*",
                _ => "All Files (*.*)|*.*"
            };

            var dialog = new OpenFileDialog
            {
                Title = $"Select custom file for {mod.Name}",
                Filter = filter
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _modManager.SetCustomFile(mod.Id, dialog.FileName);
                ShowStatus($"Set file for {mod.Name}: {Path.GetFileName(dialog.FileName)}. Now toggle to enable.", false);
                RefreshDisplayedMods();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to set custom file: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::BrowseCustomFile", ex);
            }
        }

        private void EnableAllInCategory()
        {
            if (_modManager is null)
                return;

            var modsInCategory = _modManager.GetModsByCategory(SelectedCategory);

            foreach (var mod in modsInCategory)
            {
                if (!mod.IsEnabled && string.IsNullOrEmpty(mod.CustomFilePath))
                {
                    ShowStatus($"You must click Browse to modify \"{mod.Name}\" before enabling it", true);
                    return;
                }
            }

            try
            {
                _modManager.EnableAllModsInCategory(SelectedCategory);
                ShowStatus($"Enabled all {SelectedCategory} mods", false);
                RefreshDisplayedMods();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to enable mods: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::EnableAllInCategory", ex);
            }
        }

        private void DisableAllInCategory()
        {
            if (_modManager is null)
                return;

            try
            {
                _modManager.DisableAllModsInCategory(SelectedCategory);
                ShowStatus($"Disabled all {SelectedCategory} mods", false);
                RefreshDisplayedMods();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to disable mods: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::DisableAllInCategory", ex);
            }
        }

        private void ExportPack()
        {
            if (_modManager is null)
                return;

            var enabledMods = _modManager.GetEnabledMods();
            if (enabledMods.Count == 0)
            {
                ShowStatus("No enabled mods to export. Enable some mods first.", true);
                return;
            }

            var dialog = new ExportModPackDialog();
            if (dialog.ShowDialog() != true)
                return;

            var saveDialog = new SaveFileDialog
            {
                Filter = "Rystrap Mod Pack (*.Rystrap)|*.Rystrap",
                FileName = $"{dialog.PackName}.Rystrap"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                _modManager.ExportModPack(
                    dialog.PackName,
                    dialog.Description,
                    dialog.Author,
                    saveDialog.FileName
                );

                ShowStatus($"Exported {enabledMods.Count} mod(s) to: {Path.GetFileName(saveDialog.FileName)}", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to export: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::ExportPack", ex);
            }
        }

        private void ImportPack()
        {
            if (_modManager is null)
                return;

            var dialog = new OpenFileDialog
            {
                Filter = "Rystrap Mod Pack (*.Rystrap)|*.Rystrap"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _modManager.ImportModPack(dialog.FileName);
                RefreshDisplayedMods();
                var (success, failed, errors) = _modManager.ApplyMods();
                if (failed == 0 && success > 0)
                    ShowStatus($"Imported and applied {success} mod(s)!", false);
                else if (failed > 0)
                    ShowStatus($"Imported. {success} applied, {failed} failed: {errors.First()}", true);
                else
                    ShowStatus($"Imported mod pack: {Path.GetFileName(dialog.FileName)}", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to import: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::ImportPack", ex);
            }
        }

        private void ApplyMods()
        {
            if (_modManager is null)
                return;

            try
            {
                var (success, failed, errors) = _modManager.ApplyMods();

                if (failed == 0 && success > 0)
                    ShowStatus($"Applied {success} mod(s) successfully!", false);
                else if (success == 0 && failed > 0)
                    ShowStatus($"Failed: {errors.First()}", true);
                else if (success > 0 && failed > 0)
                    ShowStatus($"Applied {success}, failed {failed}: {errors.First()}", true);
                else
                    ShowStatus("No enabled mods to apply. Browse for files and enable mods first.", true);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to apply mods: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::ApplyMods", ex);
            }
        }

        private void ResetMods()
        {
            if (_modManager is null)
                return;

            var result = MessageBox.Show(
                "This will reset ALL mods and delete the Modifications folder.\n\nRoblox will return to its default icons, sounds, cursors, animations, etc.\n\nAre you sure?",
                "Reset All Mods",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _modManager.ResetAllMods();
                ShowStatus("All mods reset to defaults. Roblox will use its original icons, sounds, cursors, etc.", false);
                RefreshDisplayedMods();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to reset mods: {ex.Message}", true);
                App.Logger.WriteException("BetterModsViewModel::ResetMods", ex);
            }
        }
    }
}
