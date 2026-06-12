namespace Rystrap.Updates
{
    public class UpdateManager
    {
        private const string LOG_IDENT = "UpdateManager";

        public async Task<UpdateInfo?> CheckForUpdateAsync()
        {
            if (!App.Settings.Prop.CheckForUpdates)
                return null;

            try
            {
                App.Logger.WriteLine(LOG_IDENT, "Checking for updates...");

                var release = await App.GetLatestRelease();

                if (release is null)
                {
                    App.Logger.WriteLine(LOG_IDENT, "No release info available");
                    return null;
                }

                var updateInfo = UpdateInfo.FromGithubRelease(release);

                if (updateInfo is null)
                {
                    App.Logger.WriteLine(LOG_IDENT, "Failed to parse update info");
                    return null;
                }

                if (!updateInfo.IsNewerThanCurrent)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Current version ({App.Version}) is up to date or newer than ({updateInfo.Version})");
                    return null;
                }

                App.Logger.WriteLine(LOG_IDENT, $"New version available: {updateInfo.Version}");

                return updateInfo;
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                return null;
            }
        }

        public static void ApplyUpdate(string downloadedFilePath)
        {
            const string LOG_IDENT = "UpdateManager::ApplyUpdate";

            if (!File.Exists(downloadedFilePath))
            {
                App.Logger.WriteLine(LOG_IDENT, $"Update file not found: {downloadedFilePath}");
                return;
            }

            App.Logger.WriteLine(LOG_IDENT, "Applying update...");

            string updaterBat = Path.Combine(Paths.Temp, "RystrapUpdater.bat");

            string batchContent = $@"
@echo off
timeout /t 2 /nobreak > nul
taskkill /f /im ""{Path.GetFileName(Paths.Process)}"" > nul 2>&1
timeout /t 1 /nobreak > nul
del /Q ""{Paths.Application}"" > nul 2>&1
move /Y ""{downloadedFilePath}"" ""{Paths.Application}"" > nul 2>&1
start """" ""{Paths.Application}""
del /Q ""%~f0""
";

            File.WriteAllText(updaterBat, batchContent);

            Process.Start(new ProcessStartInfo
            {
                FileName = updaterBat,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });

            App.Logger.WriteLine(LOG_IDENT, "Update scheduled, restarting...");
            App.SoftTerminate();
        }
    }
}
