using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using Rystrap.Models.Mods;
using Rystrap.Mods;

namespace Rystrap.UI.ViewModels.Dialogs
{
    public class ImportModPackViewModel : NotifyPropertyChangedViewModel
    {
        private readonly string _zipPath;
        private readonly BetterModManager _modManager;
        private ModPackManifest? _manifest;
        private bool _customizeMode = false;

        public event EventHandler<bool>? InstallCompleted;

        public ICommand InstallAllCommand => new RelayCommand(InstallAll);
        public ICommand CustomizeCommand => new RelayCommand(ToggleCustomize);

        public ObservableCollection<Models.Mods.ModEntry> Mods { get; } = new();

        private string _packTitle = "";
        public string PackTitle
        {
            get => _packTitle;
            set
            {
                if (_packTitle != value)
                {
                    _packTitle = value;
                    OnPropertyChanged(nameof(PackTitle));
                }
            }
        }

        private string _packDescription = "";
        public string PackDescription
        {
            get => _packDescription;
            set
            {
                if (_packDescription != value)
                {
                    _packDescription = value;
                    OnPropertyChanged(nameof(PackDescription));
                }
            }
        }

        private string _author = "";
        public string Author
        {
            get => _author;
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        private string _version = "";
        public string Version
        {
            get => _version;
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }

        private string _createdDate = "";
        public string CreatedDate
        {
            get => _createdDate;
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    OnPropertyChanged(nameof(CreatedDate));
                }
            }
        }

        private int _modCount = 0;
        public int ModCount
        {
            get => _modCount;
            set
            {
                if (_modCount != value)
                {
                    _modCount = value;
                    OnPropertyChanged(nameof(ModCount));
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

        public Visibility StatusVisibility => string.IsNullOrEmpty(StatusMessage) ? Visibility.Collapsed : Visibility.Visible;

        private string _statusText = "Ready to install";
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        private Visibility _isCustomizeVisible = Visibility.Collapsed;
        public Visibility IsCustomizeVisible
        {
            get => _isCustomizeVisible;
            set
            {
                if (_isCustomizeVisible != value)
                {
                    _isCustomizeVisible = value;
                    OnPropertyChanged(nameof(IsCustomizeVisible));
                }
            }
        }

        public bool CustomizeMode
        {
            get => _customizeMode;
            set
            {
                if (_customizeMode != value)
                {
                    _customizeMode = value;
                    OnPropertyChanged(nameof(CustomizeMode));
                }
            }
        }

        public ImportModPackViewModel(string zipPath)
        {
            _zipPath = zipPath;
            _modManager = new BetterModManager();

            LoadManifest();
        }

        private void LoadManifest()
        {
            const string LOG_IDENT = "ImportModPackViewModel::LoadManifest";

            string tempDir = Path.Combine(Path.GetTempPath(), "Rystrap", "ModImport_" + Guid.NewGuid().ToString("N"));

            try
            {
                ZipFile.ExtractToDirectory(_zipPath, tempDir);

                string manifestPath = Path.Combine(tempDir, "modpack.json");

                if (!File.Exists(manifestPath))
                {
                    StatusMessage = "Invalid mod pack: modpack.json not found";
                    return;
                }

                string manifestJson = File.ReadAllText(manifestPath);
                _manifest = JsonSerializer.Deserialize<ModPackManifest>(manifestJson);

                if (_manifest is null)
                {
                    StatusMessage = "Failed to read mod pack manifest";
                    return;
                }

                PackTitle = _manifest.Title;
                PackDescription = _manifest.Description;
                Author = _manifest.Author;
                Version = _manifest.Version;
                CreatedDate = _manifest.CreatedAt.ToString("MMM dd, yyyy");
                ModCount = _manifest.Mods.Count;

                Mods.Clear();
                foreach (var mod in _manifest.Mods)
                {
                    mod.IsEnabled = true;
                    Mods.Add(mod);
                }

                StatusText = $"Ready to install {ModCount} mod(s)";

                if (Mods.Count > 1)
                    IsCustomizeVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                StatusMessage = $"Failed to load mod pack: {ex.Message}";
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); }
                    catch { }
                }
            }
        }

        private void InstallAll()
        {
            const string LOG_IDENT = "ImportModPackViewModel::InstallAll";

            if (_manifest is null)
            {
                StatusMessage = "No mod pack loaded";
                return;
            }

            StatusText = "Installing mods...";

            try
            {
                // Import the mod pack (copies files + applied files to Modifications)
                var importedManifest = _modManager.ImportModPack(_zipPath);

                if (importedManifest is null)
                {
                    StatusMessage = "Failed to import mod pack";
                    StatusText = "Installation failed";
                    return;
                }

                StatusText = "Installation complete!";
                InstallCompleted?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                StatusMessage = $"Installation failed: {ex.Message}";
                StatusText = "Installation failed";
            }
        }

        private void ToggleCustomize()
        {
            CustomizeMode = !CustomizeMode;
        }
    }
}
