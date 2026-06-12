using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace Rystrap.UI.ViewModels.Dialogs
{
    public class UpdateViewModel : NotifyPropertyChangedViewModel
    {
        private const string LOG_IDENT = "UpdateViewModel";

        private readonly Updates.UpdateManager _updateManager;
        private readonly Updates.UpdateDownloader _downloader;
        private Updates.UpdateInfo? _updateInfo;

        private string _title = "";
        private string _changelog = "";
        private string _currentVersion = "";
        private string _latestVersion = "";
        private string _statusText = "";
        private string _skipVersionText = "";
        private double _downloadProgress;
        private bool _isDownloading;
        private bool _isChecking = true;
        private bool _hasUpdate;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Changelog
        {
            get => _changelog;
            set { _changelog = value; OnPropertyChanged(nameof(Changelog)); }
        }

        public string CurrentVersion
        {
            get => _currentVersion;
            set { _currentVersion = value; OnPropertyChanged(nameof(CurrentVersion)); }
        }

        public string LatestVersion
        {
            get => _latestVersion;
            set { _latestVersion = value; OnPropertyChanged(nameof(LatestVersion)); }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        public string SkipVersionText
        {
            get => _skipVersionText;
            set { _skipVersionText = value; OnPropertyChanged(nameof(SkipVersionText)); }
        }

        public double DownloadProgress
        {
            get => _downloadProgress;
            set { _downloadProgress = value; OnPropertyChanged(nameof(DownloadProgress)); }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set { _isDownloading = value; OnPropertyChanged(nameof(IsDownloading)); }
        }

        public bool IsChecking
        {
            get => _isChecking;
            set { _isChecking = value; OnPropertyChanged(nameof(IsChecking)); }
        }

        public bool HasUpdate
        {
            get => _hasUpdate;
            set { _hasUpdate = value; OnPropertyChanged(nameof(HasUpdate)); }
        }

        public bool ShowChangelog => HasUpdate && !IsChecking;

        public bool ShowProgress => IsDownloading;

        public ICommand UpdateNowCommand => new RelayCommand(UpdateNow);
        public ICommand SkipVersionCommand => new RelayCommand(SkipVersion);
        public ICommand RemindLaterCommand => new RelayCommand(RemindLater);

        public event EventHandler? UpdateRequested;
        public event EventHandler? CloseRequested;

        public UpdateViewModel()
        {
            _updateManager = new Updates.UpdateManager();
            _downloader = new Updates.UpdateDownloader();

            CurrentVersion = App.Version;

            _downloader.ProgressChanged += OnProgressChanged;
            _downloader.StatusChanged += OnStatusChanged;
            _downloader.DownloadCompleted += OnDownloadCompleted;
            _downloader.DownloadFailed += OnDownloadFailed;

            _ = CheckForUpdatesAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                IsChecking = true;
                StatusText = Strings.Update_Checking;

                var updateInfo = await _updateManager.CheckForUpdateAsync();

                if (updateInfo is not null && updateInfo.IsNewerThanCurrent)
                {
                    string skippedVersion = App.Settings.Prop.SkippedUpdateVersion;

                    if (!string.IsNullOrEmpty(skippedVersion) && updateInfo.Version.ToString() == skippedVersion)
                    {
                        App.Logger.WriteLine(LOG_IDENT, $"Skipping update {updateInfo.Version} (user skipped)");
                        IsChecking = false;
                        HasUpdate = false;
                        StatusText = Strings.Update_Skipped;
                        OnPropertyChanged(nameof(ShowChangelog));
                        return;
                    }

                    _updateInfo = updateInfo;

                    Title = string.Format(Strings.Update_Available, updateInfo.Version);
                    Changelog = updateInfo.Changelog;
                    LatestVersion = updateInfo.Version.ToString();
                    SkipVersionText = string.Format(Strings.Update_SkipVersion, updateInfo.Version);
                    StatusText = Strings.Update_Ready;

                    HasUpdate = true;
                    OnPropertyChanged(nameof(ShowChangelog));
                }
                else
                {
                    HasUpdate = false;
                    StatusText = Strings.Update_UpToDate;
                    OnPropertyChanged(nameof(ShowChangelog));
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                HasUpdate = false;
                StatusText = Strings.Update_Error;
                OnPropertyChanged(nameof(ShowChangelog));
            }
            finally
            {
                IsChecking = false;
                OnPropertyChanged(nameof(ShowChangelog));
            }
        }

        private async void UpdateNow()
        {
            if (_updateInfo is null)
                return;

            IsDownloading = true;
            StatusText = Strings.Update_Downloading;

            await _downloader.DownloadUpdateAsync(_updateInfo);
        }

        private void SkipVersion()
        {
            if (_updateInfo is null)
                return;

            App.Settings.Prop.SkippedUpdateVersion = _updateInfo.Version.ToString();
            App.Settings.Save();

            App.Logger.WriteLine(LOG_IDENT, $"Skipped version {_updateInfo.Version}");

            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RemindLater()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnProgressChanged(object? sender, double progress)
        {
            DownloadProgress = progress;
        }

        private void OnStatusChanged(object? sender, string status)
        {
            StatusText = status;
        }

        private void OnDownloadCompleted(object? sender, string filePath)
        {
            IsDownloading = false;
            Updates.UpdateManager.ApplyUpdate(filePath);
        }

        private void OnDownloadFailed(object? sender, Exception ex)
        {
            IsDownloading = false;
            StatusText = Strings.Update_DownloadFailed;

            MessageBoxResult result = Frontend.ShowMessageBox(
                string.Format(Strings.Update_DownloadFailedDetail, ex.Message),
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result == MessageBoxResult.Yes)
                UpdateNow();
        }
    }
}
