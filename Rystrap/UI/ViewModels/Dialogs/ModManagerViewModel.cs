using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

using Rystrap.Mods;
using Rystrap.Models;
using Rystrap.Models.Mods;

namespace Rystrap.UI.ViewModels.Dialogs
{
    public class ModManagerViewModel : NotifyPropertyChangedViewModel
    {
        private readonly ModManager _modManager;

        public ICommand AddModCommand => new RelayCommand(AddMod);
        public ICommand RemoveModCommand => new RelayCommand(RemoveMod);
        public ICommand ToggleModCommand => new RelayCommand(ToggleMod);
        public ICommand EnableAllCommand => new RelayCommand(EnableAll);
        public ICommand DisableAllCommand => new RelayCommand(DisableAll);
        public ICommand ImportModPackCommand => new RelayCommand(ImportModPack);
        public ICommand CreatePresetCommand => new RelayCommand(CreatePreset);
        public ICommand LoadPresetCommand => new RelayCommand(LoadPreset);
        public ICommand DeletePresetCommand => new RelayCommand(DeletePreset);
        public ICommand OpenModsFolderCommand => new RelayCommand(OpenModsFolder);
        public ICommand DetectConflictsCommand => new RelayCommand(DetectConflicts);
        public ICommand RefreshModsCommand => new RelayCommand(RefreshMods);

        public ObservableCollection<ModEntry> Mods { get; } = new();
        public ObservableCollection<ModPreset> Presets { get; } = new();
        public ObservableCollection<ModConflictInfo> Conflicts { get; } = new();
        public ObservableCollection<ModCategory> Categories { get; } = new();

        private ModEntry? _selectedMod;
        public ModEntry? SelectedMod
        {
            get => _selectedMod;
            set
            {
                if (_selectedMod != value)
                {
                    _selectedMod = value;
                    OnPropertyChanged(nameof(SelectedMod));
                    OnPropertyChanged(nameof(HasSelectedMod));
                    OnPropertyChanged(nameof(IsModEnabled));
                    OnPropertyChanged(nameof(ModDetails));
                }
            }
        }

        private ModPreset? _selectedPreset;
        public ModPreset? SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (_selectedPreset != value)
                {
                    _selectedPreset = value;
                    OnPropertyChanged(nameof(SelectedPreset));
                    OnPropertyChanged(nameof(HasSelectedPreset));
                }
            }
        }

        private ModCategory _selectedCategory = ModCategory.General;
        public ModCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    RefreshMods();
                }
            }
        }

        private string _newPresetName = "";
        public string NewPresetName
        {
            get => _newPresetName;
            set
            {
                if (_newPresetName != value)
                {
                    _newPresetName = value;
                    OnPropertyChanged(nameof(NewPresetName));
                }
            }
        }

        private string _newPresetDescription = "";
        public string NewPresetDescription
        {
            get => _newPresetDescription;
            set
            {
                if (_newPresetDescription != value)
                {
                    _newPresetDescription = value;
                    OnPropertyChanged(nameof(NewPresetDescription));
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
                }
            }
        }

        private string _conflictStatus = "";
        public string ConflictStatus
        {
            get => _conflictStatus;
            set
            {
                if (_conflictStatus != value)
                {
                    _conflictStatus = value;
                    OnPropertyChanged(nameof(ConflictStatus));
                    OnPropertyChanged(nameof(ConflictStatusVisibility));
                }
            }
        }

        public bool HasSelectedMod => SelectedMod is not null;
        public bool HasSelectedPreset => SelectedPreset is not null;
        public bool IsModEnabled => SelectedMod?.IsEnabled ?? false;
        public Visibility StatusVisibility => string.IsNullOrEmpty(StatusMessage) ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ConflictStatusVisibility => string.IsNullOrEmpty(ConflictStatus) ? Visibility.Collapsed : Visibility.Visible;

        public string ModDetails
        {
            get
            {
                if (SelectedMod is null) return "";

                return $"Name: {SelectedMod.Name}\n" +
                       $"Author: {SelectedMod.Author}\n" +
                       $"Version: {SelectedMod.Version}\n" +
                       $"Category: {SelectedMod.Category}\n" +
                       $"Target: {SelectedMod.TargetPath}\n" +
                       $"Status: {(SelectedMod.IsEnabled ? "Enabled" : "Disabled")}";
            }
        }

        public ModManagerViewModel()
        {
            _modManager = new ModManager();

            foreach (ModCategory cat in Enum.GetValues<ModCategory>())
                Categories.Add(cat);

            RefreshMods();
            RefreshPresets();
        }

        public void RefreshMods()
        {
            Mods.Clear();

            var filteredMods = SelectedCategory == ModCategory.General
                ? _modManager.Mods
                : _modManager.Mods.Where(m => m.Category == SelectedCategory);

            foreach (var mod in filteredMods)
                Mods.Add(mod);
        }

        private void RefreshPresets()
        {
            Presets.Clear();

            foreach (var preset in _modManager.Presets)
                Presets.Add(preset);
        }

        private string ModsDirectory => Path.Combine(Paths.Base, "Mods");

        private void OpenModsFolder()
        {
            Process.Start("explorer.exe", ModsDirectory);
        }

        private void AddMod()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All Supported Files|*.png;*.jpg;*.jpeg;*.mp3;*.ogg;*.wav;*.ttf;*.otf;*.rbxl;*.rbxm;*.lua;*.luac|Sound Files|*.mp3;*.ogg;*.wav|Image Files|*.png;*.jpg;*.jpeg|Font Files|*.ttf;*.otf|Roblox Files|*.rbxl;*.rbxm|Lua Scripts|*.lua;*.luac|All Files|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            string ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();

            ModCategory category = ext switch
            {
                ".mp3" or ".ogg" or ".wav" => ModCategory.Sound,
                ".png" or ".jpg" or ".jpeg" => ModCategory.Texture,
                ".ttf" or ".otf" => ModCategory.Font,
                ".rbxl" or ".rbxm" => ModCategory.General,
                ".lua" or ".luac" => ModCategory.General,
                _ => ModCategory.General
            };

            string targetPath = category switch
            {
                ModCategory.Sound => $@"content\sounds\{Path.GetFileName(dialog.FileName)}",
                ModCategory.Texture => $@"content\textures\{Path.GetFileName(dialog.FileName)}",
                ModCategory.Font => $@"content\fonts\{Path.GetFileName(dialog.FileName)}",
                _ => $@"content\mods\{Path.GetFileName(dialog.FileName)}"
            };

            try
            {
                var mod = _modManager.AddMod(
                    Path.GetFileNameWithoutExtension(dialog.FileName),
                    dialog.FileName,
                    category,
                    targetPath
                );

                RefreshMods();
                StatusMessage = $"Added mod: {mod.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add mod: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::AddMod", ex);
            }
        }

        private void RemoveMod()
        {
            if (SelectedMod is null)
                return;

            string name = SelectedMod.Name;

            try
            {
                _modManager.RemoveMod(SelectedMod.Id);
                RefreshMods();
                StatusMessage = $"Removed mod: {name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to remove mod: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::RemoveMod", ex);
            }
        }

        private void ToggleMod()
        {
            if (SelectedMod is null)
                return;

            try
            {
                if (SelectedMod.IsEnabled)
                    _modManager.DisableMod(SelectedMod.Id);
                else
                    _modManager.EnableMod(SelectedMod.Id);

                RefreshMods();
                StatusMessage = $"{(SelectedMod.IsEnabled ? "Enabled" : "Disabled")} mod: {SelectedMod.Name}";
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to toggle mod: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::ToggleMod", ex);
            }
        }

        private void EnableAll()
        {
            try
            {
                _modManager.EnableAllMods();
                RefreshMods();
                StatusMessage = "All mods enabled.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to enable all mods: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::EnableAll", ex);
            }
        }

        private void DisableAll()
        {
            _modManager.DisableAllMods();
            RefreshMods();
            StatusMessage = "All mods disabled.";
        }

        private void ImportModPack()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Rystrap Mod Pack (*.Rystrap)|*.Rystrap|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _modManager.ImportModPack(dialog.FileName);
                RefreshMods();
                StatusMessage = "Mod pack imported successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Import failed: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::ImportModPack", ex);
            }
        }

        private void CreatePreset()
        {
            if (string.IsNullOrWhiteSpace(NewPresetName))
            {
                StatusMessage = "Please enter a preset name.";
                return;
            }

            try
            {
                var preset = _modManager.CreatePreset(NewPresetName.Trim(), NewPresetDescription.Trim());
                RefreshPresets();
                NewPresetName = "";
                NewPresetDescription = "";
                StatusMessage = $"Created preset: {preset.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to create preset: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::CreatePreset", ex);
            }
        }

        private void LoadPreset()
        {
            if (SelectedPreset is null)
                return;

            try
            {
                _modManager.LoadPreset(SelectedPreset);
                RefreshMods();
                StatusMessage = $"Loaded preset: {SelectedPreset.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load preset: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::LoadPreset", ex);
            }
        }

        private void DeletePreset()
        {
            if (SelectedPreset is null)
                return;

            string name = SelectedPreset.Name;

            try
            {
                _modManager.DeletePreset(SelectedPreset);
                RefreshPresets();
                StatusMessage = $"Deleted preset: {name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to delete preset: {ex.Message}";
                App.Logger.WriteException("ModManagerViewModel::DeletePreset", ex);
            }
        }

        private void DetectConflicts()
        {
            Conflicts.Clear();

            var conflicts = _modManager.GetAllConflicts();

            foreach (var conflict in conflicts)
                Conflicts.Add(conflict);

            if (conflicts.Any())
                ConflictStatus = $"Found {conflicts.Count} conflict(s).";
            else
                ConflictStatus = "No conflicts detected.";
        }
    }
}
