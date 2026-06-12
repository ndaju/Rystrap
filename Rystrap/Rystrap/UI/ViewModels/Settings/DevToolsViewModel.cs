using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Rystrap.UI.ViewModels.Settings
{
    public class DevToolsViewModel : NotifyPropertyChangedViewModel
    {
        private void OpenLogViewer()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new Elements.Dialogs.LogViewerDialog().ShowDialog();
            });
        }

        private void OpenNetworkMonitor()
        {
            try
            {
                Process.Start("resmon.exe");
            }
            catch
            {
                Frontend.ShowMessageBox("Could not open Resource Monitor.", MessageBoxImage.Error);
            }
        }

        private void OpenPlace()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Enter Place ID (type in the filename box)",
                Filter = "All files (*.*)|*.*",
                CheckFileExists = false,
                FileName = "Place ID here"
            };

            if (dialog.ShowDialog() != true)
                return;

            string placeId = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);

            if (string.IsNullOrWhiteSpace(placeId))
            {
                Frontend.ShowMessageBox("Enter a valid Place ID.", MessageBoxImage.Warning);
                return;
            }

            string trimmed = placeId.Trim();
            if (!trimmed.All(char.IsDigit))
            {
                Frontend.ShowMessageBox("Place ID must contain only digits.", MessageBoxImage.Warning);
                return;
            }

            try
            {
                Utilities.ShellExecute($"roblox://placeId={trimmed}");
            }
            catch
            {
                Frontend.ShowMessageBox("Failed to open Roblox place.", MessageBoxImage.Error);
            }
        }

        private void OpenLogsFolder()
        {
            string logsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "logs");

            if (Directory.Exists(logsDir))
                Process.Start("explorer.exe", logsDir);
            else
                Frontend.ShowMessageBox("Roblox logs directory not found.", MessageBoxImage.Information);
        }

        private void ClearCookies()
        {
            var result = Frontend.ShowMessageBox(
                "This will delete all Roblox saved cookies and log you out of your account on the Roblox app.\n\nContinue?",
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            int deleted = 0;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] targets = new[]
            {
                Path.Combine(localAppData, "Roblox", "Cookies"),
                Path.Combine(localAppData, "Roblox", "Local Storage", "leveldb"),
                Path.Combine(localAppData, "Packages", "ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr", "LocalCache", "Roblox", "Cookies"),
                Path.Combine(localAppData, "Packages", "ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr", "LocalCache", "Roblox", "Local Storage", "leveldb"),
            };

            foreach (string target in targets)
            {
                if (Directory.Exists(target))
                {
                    try
                    {
                        Directory.Delete(target, true);
                        deleted++;
                    }
                    catch { }
                }
                else if (File.Exists(target))
                {
                    try
                    {
                        File.Delete(target);
                        deleted++;
                    }
                    catch { }
                }
            }

            Frontend.ShowMessageBox(
                deleted > 0
                    ? $"Roblox cookies cleared ({deleted} location{(deleted > 1 ? "s" : "")} cleaned). You are now logged out."
                    : "No Roblox cookie storage found. You may already be logged out.",
                MessageBoxImage.Information);
        }

        public ICommand OpenLogViewerCommand => new RelayCommand(OpenLogViewer);
        public ICommand OpenNetworkMonitorCommand => new RelayCommand(OpenNetworkMonitor);
        public ICommand OpenPlaceCommand => new RelayCommand(OpenPlace);
        public ICommand OpenLogsFolderCommand => new RelayCommand(OpenLogsFolder);
        public ICommand ClearCookiesCommand => new RelayCommand(ClearCookies);
        public ICommand OpenBanHistoryCommand => new RelayCommand(OpenBanHistory);
        public ICommand OpenGameSettingsCommand => new RelayCommand(OpenGameSettings);

        private void OpenBanHistory()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new Rystrap.UI.Elements.Dialogs.BanHistoryDialog().ShowDialog();
            });
        }

        private void OpenGameSettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new Rystrap.UI.Elements.Dialogs.GameSettingsDialog().ShowDialog();
            });
        }
    }
}
