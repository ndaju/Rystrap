using System.Linq;
using System.Text.Json;
using Rystrap.Models.Persistable;

namespace Rystrap.AppData
{
    public static class StartupManager
    {
        private const string StartupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "Rystrap";

        public static void SetAutoLaunch(bool enabled)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupKey, true);
                if (key is null) return;
                if (enabled)
                    key.SetValue(ValueName, $"\"{Paths.Process}\" -menu");
                else
                {
                    if (key.GetValue(ValueName) is not null)
                        key.DeleteValue(ValueName);
                }
            }
            catch { }
        }

        public static bool IsAutoLaunchEnabled()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupKey, false);
                return key?.GetValue(ValueName) is not null;
            }
            catch { return false; }
        }

        // Build launch args for region selection
        public static string GetRegionArg(string region)
        {
            return region switch
            {
                "US" => " --rloc US",
                "EU" => " --rloc EU",
                "Asia" => " --rloc Asia",
                "Australia" => " --rloc Oceania",
                _ => ""
            };
        }
    }
}
