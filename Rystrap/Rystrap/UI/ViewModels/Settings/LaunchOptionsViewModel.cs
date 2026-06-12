using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using Rystrap.Models;

namespace Rystrap.UI.ViewModels.Settings
{
    public class LaunchOptionsViewModel : NotifyPropertyChangedViewModel
    {
        private const string LOG_IDENT = "LaunchOptionsViewModel";

        private Models.LaunchOptions _options;

        private string _customLaunchArgs = "";
        private long _memoryLimitMB;
        private GpuPreference _gpuPreference;
        private bool _multiInstanceEnabled;
        private string _workingDirectory = "";
        private string _previewCommandLine = "";
        private EnvironmentVariableOverride? _selectedEnvOverride;
        private int _selectedEnvOverrideIndex = -1;

        public ObservableCollection<EnvironmentVariableOverride> EnvironmentOverrides { get; set; } = new();

        public ICommand BrowseWorkingDirectoryCommand => new RelayCommand(BrowseWorkingDirectory);
        public ICommand AddEnvironmentOverrideCommand => new RelayCommand(AddEnvironmentOverride);
        public ICommand RemoveEnvironmentOverrideCommand => new RelayCommand(RemoveEnvironmentOverride);
        public ICommand ResetDefaultsCommand => new RelayCommand(ResetDefaults);

        public IEnumerable<GpuPreference> GpuPreferences { get; } = Enum.GetValues<GpuPreference>();

        public string CustomLaunchArgs
        {
            get => _customLaunchArgs;
            set
            {
                _customLaunchArgs = value;
                OnPropertyChanged(nameof(CustomLaunchArgs));
                UpdatePreview();
            }
        }

        public long MemoryLimitMB
        {
            get => _memoryLimitMB;
            set
            {
                _memoryLimitMB = value;
                OnPropertyChanged(nameof(MemoryLimitMB));
                OnPropertyChanged(nameof(MemoryLimitDisplay));
                UpdatePreview();
            }
        }

        public string MemoryLimitDisplay => MemoryLimitMB > 0 ? $"{MemoryLimitMB} MB" : Strings.LaunchOptions_NoLimit;

        public GpuPreference GpuPreference
        {
            get => _gpuPreference;
            set
            {
                _gpuPreference = value;
                OnPropertyChanged(nameof(GpuPreference));
                UpdatePreview();
            }
        }

        public bool MultiInstanceEnabled
        {
            get => _multiInstanceEnabled;
            set
            {
                _multiInstanceEnabled = value;
                OnPropertyChanged(nameof(MultiInstanceEnabled));
                UpdatePreview();
            }
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set
            {
                _workingDirectory = value;
                OnPropertyChanged(nameof(WorkingDirectory));
                OnPropertyChanged(nameof(HasWorkingDirectory));
                UpdatePreview();
            }
        }

        public bool HasWorkingDirectory => !string.IsNullOrWhiteSpace(WorkingDirectory);

        public string PreviewCommandLine
        {
            get => _previewCommandLine;
            set { _previewCommandLine = value; OnPropertyChanged(nameof(PreviewCommandLine)); }
        }

        public EnvironmentVariableOverride? SelectedEnvOverride
        {
            get => _selectedEnvOverride;
            set
            {
                _selectedEnvOverride = value;
                OnPropertyChanged(nameof(SelectedEnvOverride));
                OnPropertyChanged(nameof(IsEnvOverrideSelected));
            }
        }

        public int SelectedEnvOverrideIndex
        {
            get => _selectedEnvOverrideIndex;
            set { _selectedEnvOverrideIndex = value; OnPropertyChanged(nameof(SelectedEnvOverrideIndex)); }
        }

        public bool IsEnvOverrideSelected => SelectedEnvOverride is not null;

        public LaunchOptionsViewModel()
        {
            _options = App.Settings.Prop.LaunchOptions?.Clone() ?? new Models.LaunchOptions();

            _customLaunchArgs = _options.CustomLaunchArgs;
            _memoryLimitMB = _options.MemoryLimitMB;
            _gpuPreference = _options.GpuPreference;
            _multiInstanceEnabled = _options.MultiInstanceEnabled;
            _workingDirectory = _options.WorkingDirectory;

            foreach (var envVar in _options.EnvironmentOverrides)
                EnvironmentOverrides.Add(new EnvironmentVariableOverride
                {
                    Name = envVar.Name,
                    Value = envVar.Value,
                    Enabled = envVar.Enabled
                });

            UpdatePreview();
        }

        public void Save()
        {
            _options.CustomLaunchArgs = CustomLaunchArgs;
            _options.MemoryLimitMB = MemoryLimitMB;
            _options.GpuPreference = GpuPreference;
            _options.MultiInstanceEnabled = MultiInstanceEnabled;
            _options.WorkingDirectory = WorkingDirectory;
            _options.EnvironmentOverrides.Clear();

            foreach (var envVar in EnvironmentOverrides)
                _options.EnvironmentOverrides.Add(new EnvironmentVariableOverride
                {
                    Name = envVar.Name,
                    Value = envVar.Value,
                    Enabled = envVar.Enabled
                });

            App.Settings.Prop.LaunchOptions = _options;
            App.Settings.Save();

            App.Logger.WriteLine(LOG_IDENT, "Launch options saved");
        }

        private void BrowseWorkingDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = Strings.LaunchOptions_SelectWorkingDirectory,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                WorkingDirectory = dialog.SelectedPath;
        }

        private void AddEnvironmentOverride()
        {
            EnvironmentOverrides.Add(new EnvironmentVariableOverride
            {
                Name = "",
                Value = "",
                Enabled = true
            });

            SelectedEnvOverrideIndex = EnvironmentOverrides.Count - 1;
        }

        private void RemoveEnvironmentOverride()
        {
            if (SelectedEnvOverride is null)
                return;

            int idx = EnvironmentOverrides.IndexOf(SelectedEnvOverride);
            EnvironmentOverrides.Remove(SelectedEnvOverride);

            if (EnvironmentOverrides.Any())
                SelectedEnvOverrideIndex = Math.Min(idx, EnvironmentOverrides.Count - 1);
            else
                SelectedEnvOverrideIndex = -1;
        }

        private void ResetDefaults()
        {
            CustomLaunchArgs = "";
            MemoryLimitMB = 0;
            GpuPreference = GpuPreference.Default;
            MultiInstanceEnabled = false;
            WorkingDirectory = "";
            EnvironmentOverrides.Clear();
        }

        private void UpdatePreview()
        {
            var options = new Models.LaunchOptions
            {
                CustomLaunchArgs = CustomLaunchArgs,
                MemoryLimitMB = MemoryLimitMB,
                GpuPreference = GpuPreference,
                MultiInstanceEnabled = MultiInstanceEnabled,
                WorkingDirectory = WorkingDirectory
            };

            string commandLine = Launch.LaunchOptionBuilder.BuildCommandLine(options);
            PreviewCommandLine = string.IsNullOrEmpty(commandLine) ? Strings.LaunchOptions_NoExtraArgs : commandLine;
        }

        public string RobloxScreenshotsPath
        {
            get => App.Settings.Prop.RobloxScreenshotsPath;
            set { App.Settings.Prop.RobloxScreenshotsPath = value; OnPropertyChanged(nameof(RobloxScreenshotsPath)); }
        }

        public ICommand OpenScreenshotsFolderCommand => new RelayCommand(OpenScreenshotsFolder);

        private void OpenScreenshotsFolder()
        {
            string folder = App.Settings.Prop.RobloxScreenshotsPath;
            if (string.IsNullOrEmpty(folder))
                folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Roblox", "Versions");
            if (Directory.Exists(folder))
                Process.Start("explorer.exe", folder);
            else
                Frontend.ShowMessageBox("Screenshots folder not found.", MessageBoxImage.Warning);
        }
    }
}
