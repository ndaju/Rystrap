using System.Windows;
using System.Windows.Input;
using Rystrap.UI.Elements.About;
using CommunityToolkit.Mvvm.Input;

namespace Rystrap.UI.ViewModels.Settings
{
    public class MainWindowViewModel : NotifyPropertyChangedViewModel
    {
        public ICommand OpenAboutCommand => new RelayCommand(OpenAbout);
        
        public ICommand SaveSettingsCommand => new RelayCommand(SaveSettings);
        
        public ICommand CloseWindowCommand => new RelayCommand(CloseWindow);

        public EventHandler? RequestSaveNoticeEvent;
        
        public EventHandler? RequestCloseWindowEvent;

        public bool TestModeEnabled
        {
            get => App.LaunchSettings.TestModeFlag.Active;
            set
            {
                if (value)
                {
                    var result = Frontend.ShowMessageBox(Strings.Menu_TestMode_Prompt, MessageBoxImage.Information, MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes)
                        return;
                }

                App.LaunchSettings.TestModeFlag.Active = value;
            }
        }

        private void OpenAbout() => new MainWindow().ShowDialog();

        private void CloseWindow() => RequestCloseWindowEvent?.Invoke(this, EventArgs.Empty);

        private static void CopyCustomSound(string settingsPath, string targetRelativePath)
        {
            if (string.IsNullOrEmpty(settingsPath) || !File.Exists(settingsPath))
            {
                string destPath = Path.Combine(Paths.Modifications, targetRelativePath);
                if (File.Exists(destPath))
                {
                    Filesystem.AssertReadOnly(destPath);
                    File.Delete(destPath);
                }
                return;
            }

            string fullDestPath = Path.Combine(Paths.Modifications, targetRelativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullDestPath)!);
            Filesystem.AssertReadOnly(fullDestPath);
            File.Copy(settingsPath, fullDestPath, true);
        }

        private void SaveSettings()
        {
            const string LOG_IDENT = "MainWindowViewModel::SaveSettings";

            CopyCustomSound(App.Settings.Prop.CustomJumpSoundPath, @"content\sounds\action_jump.mp3");
            CopyCustomSound(App.Settings.Prop.CustomDeathSoundPath, @"content\sounds\action_death.mp3");
            CopyCustomSound(App.Settings.Prop.CustomWalkSoundPath, @"content\sounds\action_footsteps_plastic.mp3");

            App.Settings.Save();
            App.State.Save();
            App.FastFlags.Save();

            foreach (var pair in App.PendingSettingTasks)
            {
                var task = pair.Value;

                if (task.Changed)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Executing pending task '{task}'");
                    task.Execute();
                }
            }

            App.PendingSettingTasks.Clear();

            RequestSaveNoticeEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
