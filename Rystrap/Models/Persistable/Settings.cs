using System.Collections.ObjectModel;
using Rystrap.Models.Discord;

namespace Rystrap.Models.Persistable
{
    public class Settings
    {
        // Rystrap configuration
        public BootstrapperStyle BootstrapperStyle { get; set; } = BootstrapperStyle.FluentDialog;
        public BootstrapperIcon BootstrapperIcon { get; set; } = BootstrapperIcon.IconRystrap;
        public string BootstrapperTitle { get; set; } = App.ProjectName;
        public string BootstrapperIconCustomLocation { get; set; } = "";
        public string BootstrapperProgressColor { get; set; } = "#4CAF50";
        public string BootstrapperBackgroundImage { get; set; } = "";
        public string CustomCursorPath { get; set; } = "";
        public Theme Theme { get; set; } = Theme.Default;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool DeveloperMode { get; set; } = false;
        public bool CheckForUpdates { get; set; } = true;
        public bool ConfirmLaunches { get; set; } = false;
        public string Locale { get; set; } = "nil";
        public bool UseFastFlagManager { get; set; } = true;
        public bool WPFSoftwareRender { get; set; } = false;
        public bool BackgroundUpdatesEnabled { get; set; } = false;
        public bool DebugDisableVersionPackageCleanup { get; set; } = false;
        public string? SelectedCustomTheme { get; set; } = null;
        public string? SkippedUpdateVersion { get; set; } = null;
        public string? LastSeenWhatsNewVersion { get; set; } = null;
        public bool StartupPerformanceMode { get; set; } = false;
        public bool ProxyEnabled { get; set; } = false;
        public string ProxyAddress { get; set; } = "";
        public int ProxyPort { get; set; } = 8080;

        // custom sound mod paths
        public string CustomJumpSoundPath { get; set; } = "";
        public string CustomDeathSoundPath { get; set; } = "";
        public string CustomWalkSoundPath { get; set; } = "";

        // bootstrapper sounds
        public string BootstrapperStartSound { get; set; } = "";
        public string BootstrapperErrorSound { get; set; } = "";
        public string BootstrapperCompleteSound { get; set; } = "";

        // performance configuration
        public int FpsUnlockerTarget { get; set; } = 0;
        public int RenderDistance { get; set; } = 0;
        public string GraphicsQualityPreset { get; set; } = "Auto";
        public bool DisableParticles { get; set; } = false;
        public bool DisableLighting { get; set; } = false;
        public string CpuPriorityClass { get; set; } = "Normal";
        public string CpuAffinityMask { get; set; } = "";
        public string PreferredServerRegion { get; set; } = "";
        public string RobloxScreenshotsPath { get; set; } = "";

        // launch options
        public LaunchOptions LaunchOptions { get; set; } = new();
        public bool MultiInstanceEnabled { get; set; } = true;

        // discord rich presence
        public EnhancedPresenceConfig EnhancedDiscordPresence { get; set; } = new();

        // integration configuration
        public bool EnableActivityTracking { get; set; } = true;
        public bool UseDiscordRichPresence { get; set; } = true;
        public bool HideRPCButtons { get; set; } = true;
        public bool ShowAccountOnRichPresence { get; set; } = false;
        public bool ShowServerDetails { get; set; } = false;
        public ObservableCollection<CustomIntegration> CustomIntegrations { get; set; } = new();

        // mod preset configuration
        public bool UseDisableAppPatch { get; set; } = false;
    }
}
