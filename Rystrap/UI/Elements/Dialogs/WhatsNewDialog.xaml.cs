using System.Windows;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class WhatsNewDialog
    {
        public WhatsNewDialog(string version, string changelog)
        {
            InitializeComponent();
            Title = $"What's New in Rystrap {version}";
            ChangelogText.Text = changelog;
            Owner = App.Current.MainWindow;
        }

        private void GotIt_Click(object sender, RoutedEventArgs e)
        {
            App.Settings.Prop.LastSeenWhatsNewVersion = App.Version;
            App.Settings.Save();
            Close();
        }

        public static void ShowIfNew()
        {
            string currentVersion = App.Version;
            string? lastSeen = App.Settings.Prop.LastSeenWhatsNewVersion;

            if (lastSeen == currentVersion)
                return;

            string changelog = GetChangelog(currentVersion);
            if (string.IsNullOrEmpty(changelog))
                return;

            var dialog = new WhatsNewDialog(currentVersion, changelog);
            dialog.ShowDialog();
        }

        private static string GetChangelog(string version)
        {
            return version switch
            {
                "2.11.3" => "- Forked to ndaju/Rystrap\n- Fixed blurry icon in bootstrapper & launch menu\n- Fixed XAML build errors\n- Removed dead Rystraplabs.com links\n- Updated all GitHub references\n- Cleaned up launch menu (removed wiki/community links)\n- Added plugin system, theme engine, mod manager",
                "2.11.5" => "- Removed analytics and WebEnvironment\n- Added launch-time account selection dialog\n- Moved Discord Rich Presence settings to Discord Settings tab\n- Removed Better Mods tab\n- Added mod preset tasks (sound mods, cursor, emoji, font)\n- Added bootstrapper sound customization (start, error, complete)\n- Added bootstrapper progress bar color picker (defaults to green #4CAF50)\n- Added bootstrapper background image support\n- Added Reset to Default buttons for Mods and Bootstrapper tabs\n- Progress bar color now applies to FluentDialog & ClassicFluentDialog\n- Fixed build errors with nested source exclusions",
                _ => "",
            };
        }
    }
}
