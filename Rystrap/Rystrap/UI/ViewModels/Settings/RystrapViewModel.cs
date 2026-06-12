using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Rystrap.AppData;

namespace Rystrap.UI.ViewModels.Settings
{
    public class RystrapViewModel : NotifyPropertyChangedViewModel
    {
        public WebEnvironment[] WebEnvironments => Enum.GetValues<WebEnvironment>();

        public bool UpdateCheckingEnabled
        {
            get => App.Settings.Prop.CheckForUpdates;
            set => App.Settings.Prop.CheckForUpdates = value;
        }

        public bool AnalyticsEnabled
        {
            get => App.Settings.Prop.EnableAnalytics;
            set => App.Settings.Prop.EnableAnalytics = value;
        }

        public WebEnvironment WebEnvironment
        {
            get => App.Settings.Prop.WebEnvironment;
            set => App.Settings.Prop.WebEnvironment = value;
        }

        public Visibility WebEnvironmentVisibility => App.Settings.Prop.DeveloperMode ? Visibility.Visible : Visibility.Collapsed;

        public bool ShouldExportConfig { get; set; } = true;

        public bool ShouldExportLogs { get; set; } = true;

        // ==== New general properties ====

        public bool StartupPerformanceMode
        {
            get => App.Settings.Prop.StartupPerformanceMode;
            set => App.Settings.Prop.StartupPerformanceMode = value;
        }

        public bool ProxyEnabled
        {
            get => App.Settings.Prop.ProxyEnabled;
            set { App.Settings.Prop.ProxyEnabled = value; OnPropertyChanged(nameof(ProxyEnabled)); }
        }

        public string ProxyAddress
        {
            get => App.Settings.Prop.ProxyAddress;
            set { App.Settings.Prop.ProxyAddress = value; OnPropertyChanged(nameof(ProxyAddress)); }
        }

        public int ProxyPort
        {
            get => App.Settings.Prop.ProxyPort;
            set { App.Settings.Prop.ProxyPort = value; OnPropertyChanged(nameof(ProxyPort)); }
        }

        public ICommand ExportDataCommand => new RelayCommand(ExportData);
        public ICommand ImportSettingsCommand => new RelayCommand(ImportSettings);
        public ICommand ExportSettingsCommand => new RelayCommand(ExportSettings);
        public ICommand UninstallCommand => new RelayCommand(Uninstall);

        private void Uninstall()
        {
            var result = Frontend.ShowMessageBox(
                Strings.Uninstaller_Title + "\n\n" + Strings.Uninstaller_Text,
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result != MessageBoxResult.Yes)
                return;

            var keepDataResult = Frontend.ShowMessageBox(
                Strings.Uninstaller_KeepData_Label + "\n\n" + Strings.Uninstaller_KeepData_Description,
                MessageBoxImage.Question,
                MessageBoxButton.YesNo
            );

            bool keepData = keepDataResult == MessageBoxResult.Yes;

            Rystrap.Installer.DoUninstall(keepData);

            Frontend.ShowMessageBox(Strings.Bootstrapper_SuccessfullyUninstalled, MessageBoxImage.Information);
            App.Terminate();
        }

        private void ExportData()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            var dialog = new SaveFileDialog 
            { 
                FileName = $"Rystrap-export-{timestamp}.Rystrap",
                Filter = "Rystrap backup (*.Rystrap)|*.Rystrap"
            };

            if (dialog.ShowDialog() != true)
                return;

            using var memStream = new MemoryStream();
            using var zipStream = new ZipOutputStream(memStream);

            if (ShouldExportConfig)
            {
                var files = new List<string>()
                {
                    App.Settings.FileLocation,
                    App.State.FileLocation
                };

                AddFilesToZipStream(zipStream, files, "Config/");
            }

            if (ShouldExportLogs && Directory.Exists(Paths.Logs))
            {
                var files = Directory.GetFiles(Paths.Logs)
                    .Where(x => !x.Equals(App.Logger.FileLocation, StringComparison.OrdinalIgnoreCase));

                AddFilesToZipStream(zipStream, files, "Logs/");
            }

            zipStream.CloseEntry();
            zipStream.Finish();
            memStream.Position = 0;

            using var outputStream = File.OpenWrite(dialog.FileName);
            memStream.CopyTo(outputStream);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        private void ImportSettings()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Rystrap backup (*.Rystrap)|*.Rystrap",
                Title = "Select a Rystrap backup file to import"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using var memStream = new MemoryStream(File.ReadAllBytes(dialog.FileName));
                using var zipStream = new ZipInputStream(memStream);

                string extractDir = Path.Combine(Paths.Base, "ImportBackup");
                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);

                ZipEntry entry;
                while ((entry = zipStream.GetNextEntry()) != null)
                {
                    string destFile = Path.Combine(extractDir, entry.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                    using var fs = File.Create(destFile);
                    zipStream.CopyTo(fs);
                }

                // Restore config files
                string configDir = Path.Combine(extractDir, "Config");
                if (Directory.Exists(configDir))
                {
                    foreach (var file in Directory.GetFiles(configDir))
                    {
                        string dest = Path.Combine(Paths.Base, Path.GetFileName(file));
                        File.Copy(file, dest, true);
                    }
                    Frontend.ShowMessageBox("Settings imported successfully. Please restart Rystrap for all changes to take effect.", MessageBoxImage.Information);
                }
                else
                {
                    Frontend.ShowMessageBox("No configuration files found in the backup.", MessageBoxImage.Warning);
                }

                Directory.Delete(extractDir, true);
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Failed to import settings: {ex.Message}", MessageBoxImage.Error);
            }
        }

        private void ExportSettings()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            var dialog = new SaveFileDialog
            {
                FileName = $"Rystrap-settings-{timestamp}.Rystrap",
                Filter = "Rystrap backup (*.Rystrap)|*.Rystrap"
            };

            if (dialog.ShowDialog() != true)
                return;

            using var memStream = new MemoryStream();
            using var zipStream = new ZipOutputStream(memStream);

            var files = new List<string>()
            {
                App.Settings.FileLocation,
                App.State.FileLocation
            };

            AddFilesToZipStream(zipStream, files, "Config/");

            zipStream.CloseEntry();
            zipStream.Finish();
            memStream.Position = 0;

            using var outputStream = File.OpenWrite(dialog.FileName);
            memStream.CopyTo(outputStream);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        private void AddFilesToZipStream(ZipOutputStream zipStream, IEnumerable<string> files, string directory)
        {
            const string LOG_IDENT = "RystrapViewModel::AddFilesToZipStream";

            foreach (string file in files)
            {
                if (!File.Exists(file))
                    continue;

                try
                {
                    using FileStream fileStream = File.OpenRead(file);

                    var entry = new ZipEntry(directory + Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;

                    zipStream.PutNextEntry(entry);

                    fileStream.CopyTo(zipStream);
                }
                catch (IOException ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to open '{file}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }
    }
}
