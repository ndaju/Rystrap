using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Rystrap.UI.ViewModels.Settings
{
    public class BehaviourViewModel : NotifyPropertyChangedViewModel
    {
        private string _bootstrapperStartSoundDisplay = "";
        private string _bootstrapperErrorSoundDisplay = "";
        private string _bootstrapperCompleteSoundDisplay = "";

        public string BootstrapperStartSoundDisplay
        {
            get => _bootstrapperStartSoundDisplay;
            set { _bootstrapperStartSoundDisplay = value; OnPropertyChanged(nameof(BootstrapperStartSoundDisplay)); }
        }
        public string BootstrapperErrorSoundDisplay
        {
            get => _bootstrapperErrorSoundDisplay;
            set { _bootstrapperErrorSoundDisplay = value; OnPropertyChanged(nameof(BootstrapperErrorSoundDisplay)); }
        }
        public string BootstrapperCompleteSoundDisplay
        {
            get => _bootstrapperCompleteSoundDisplay;
            set { _bootstrapperCompleteSoundDisplay = value; OnPropertyChanged(nameof(BootstrapperCompleteSoundDisplay)); }
        }

        public bool HasBootstrapperStartSound => !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperStartSound);
        public bool HasBootstrapperErrorSound => !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperErrorSound);
        public bool HasBootstrapperCompleteSound => !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperCompleteSound);

        public Visibility ShowClearStartSound => HasBootstrapperStartSound ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowClearErrorSound => HasBootstrapperErrorSound ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowClearCompleteSound => HasBootstrapperCompleteSound ? Visibility.Visible : Visibility.Collapsed;

        public ICommand BrowseStartSoundCommand => new RelayCommand(BrowseStartSound);
        public ICommand BrowseErrorSoundCommand => new RelayCommand(BrowseErrorSound);
        public ICommand BrowseCompleteSoundCommand => new RelayCommand(BrowseCompleteSound);
        public ICommand ClearStartSoundCommand => new RelayCommand(ClearStartSound);
        public ICommand ClearErrorSoundCommand => new RelayCommand(ClearErrorSound);
        public ICommand ClearCompleteSoundCommand => new RelayCommand(ClearCompleteSound);

        public BehaviourViewModel()
        {
            BootstrapperStartSoundDisplay = !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperStartSound) ? Path.GetFileName(App.Settings.Prop.BootstrapperStartSound) : "";
            BootstrapperErrorSoundDisplay = !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperErrorSound) ? Path.GetFileName(App.Settings.Prop.BootstrapperErrorSound) : "";
            BootstrapperCompleteSoundDisplay = !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperCompleteSound) ? Path.GetFileName(App.Settings.Prop.BootstrapperCompleteSound) : "";
            BootstrapperBackgroundImageDisplay = !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperBackgroundImage) ? Path.GetFileName(App.Settings.Prop.BootstrapperBackgroundImage) : "";
        }

        private void BrowseStartSound()
        {
            var dialog = new OpenFileDialog { Filter = "Audio Files (*.mp3;*.ogg;*.wav)|*.mp3;*.ogg;*.wav|All Files (*.*)|*.*", Title = "Select bootstrapper start sound" };
            if (dialog.ShowDialog() == true)
            {
                App.Settings.Prop.BootstrapperStartSound = dialog.FileName;
                BootstrapperStartSoundDisplay = Path.GetFileName(dialog.FileName);
                OnPropertyChanged(nameof(HasBootstrapperStartSound));
                OnPropertyChanged(nameof(ShowClearStartSound));
            }
        }

        private void BrowseErrorSound()
        {
            var dialog = new OpenFileDialog { Filter = "Audio Files (*.mp3;*.ogg;*.wav)|*.mp3;*.ogg;*.wav|All Files (*.*)|*.*", Title = "Select bootstrapper error sound" };
            if (dialog.ShowDialog() == true)
            {
                App.Settings.Prop.BootstrapperErrorSound = dialog.FileName;
                BootstrapperErrorSoundDisplay = Path.GetFileName(dialog.FileName);
                OnPropertyChanged(nameof(HasBootstrapperErrorSound));
                OnPropertyChanged(nameof(ShowClearErrorSound));
            }
        }

        private void BrowseCompleteSound()
        {
            var dialog = new OpenFileDialog { Filter = "Audio Files (*.mp3;*.ogg;*.wav)|*.mp3;*.ogg;*.wav|All Files (*.*)|*.*", Title = "Select bootstrapper complete sound" };
            if (dialog.ShowDialog() == true)
            {
                App.Settings.Prop.BootstrapperCompleteSound = dialog.FileName;
                BootstrapperCompleteSoundDisplay = Path.GetFileName(dialog.FileName);
                OnPropertyChanged(nameof(HasBootstrapperCompleteSound));
                OnPropertyChanged(nameof(ShowClearCompleteSound));
            }
        }

        private void ClearStartSound() { App.Settings.Prop.BootstrapperStartSound = ""; BootstrapperStartSoundDisplay = ""; OnPropertyChanged(nameof(HasBootstrapperStartSound)); OnPropertyChanged(nameof(ShowClearStartSound)); }
        private void ClearErrorSound() { App.Settings.Prop.BootstrapperErrorSound = ""; BootstrapperErrorSoundDisplay = ""; OnPropertyChanged(nameof(HasBootstrapperErrorSound)); OnPropertyChanged(nameof(ShowClearErrorSound)); }
        private void ClearCompleteSound() { App.Settings.Prop.BootstrapperCompleteSound = ""; BootstrapperCompleteSoundDisplay = ""; OnPropertyChanged(nameof(HasBootstrapperCompleteSound)); OnPropertyChanged(nameof(ShowClearCompleteSound)); }

        public bool ConfirmLaunches
        {
            get => App.Settings.Prop.ConfirmLaunches;
            set => App.Settings.Prop.ConfirmLaunches = value;
        }

        public bool BackgroundUpdates
        {
            get => App.Settings.Prop.BackgroundUpdatesEnabled;
            set => App.Settings.Prop.BackgroundUpdatesEnabled = value;
        }

        public bool IsRobloxInstallationMissing => !App.IsPlayerInstalled && !App.IsStudioInstalled;

        public bool ForceRobloxReinstallation
        {
            get => App.State.Prop.ForceReinstall || IsRobloxInstallationMissing;
            set => App.State.Prop.ForceReinstall = value;
        }

        public string BootstrapperProgressColor
        {
            get => App.Settings.Prop.BootstrapperProgressColor;
            set
            {
                App.Settings.Prop.BootstrapperProgressColor = value;
                OnPropertyChanged(nameof(BootstrapperProgressColor));
            }
        }

        private string _bootstrapperBackgroundImageDisplay = "";
        public string BootstrapperBackgroundImageDisplay
        {
            get => _bootstrapperBackgroundImageDisplay;
            set { _bootstrapperBackgroundImageDisplay = value; OnPropertyChanged(nameof(BootstrapperBackgroundImageDisplay)); }
        }
        public bool HasBootstrapperBackgroundImage => !string.IsNullOrEmpty(App.Settings.Prop.BootstrapperBackgroundImage);
        public Visibility ShowClearBootstrapperBackgroundImage => HasBootstrapperBackgroundImage ? Visibility.Visible : Visibility.Collapsed;

        public ICommand BrowseBootstrapperBackgroundImageCommand => new RelayCommand(BrowseBootstrapperBackgroundImage);
        public ICommand ClearBootstrapperBackgroundImageCommand => new RelayCommand(ClearBootstrapperBackgroundImage);

        private void BrowseBootstrapperBackgroundImage()
        {
            var dialog = new OpenFileDialog { Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All Files (*.*)|*.*", Title = "Select bootstrapper background image" };
            if (dialog.ShowDialog() == true)
            {
                App.Settings.Prop.BootstrapperBackgroundImage = dialog.FileName;
                BootstrapperBackgroundImageDisplay = Path.GetFileName(dialog.FileName);
                OnPropertyChanged(nameof(HasBootstrapperBackgroundImage));
                OnPropertyChanged(nameof(ShowClearBootstrapperBackgroundImage));
            }
        }

        private void ClearBootstrapperBackgroundImage()
        {
            App.Settings.Prop.BootstrapperBackgroundImage = "";
            BootstrapperBackgroundImageDisplay = "";
            OnPropertyChanged(nameof(HasBootstrapperBackgroundImage));
            OnPropertyChanged(nameof(ShowClearBootstrapperBackgroundImage));
        }
    }
}
