using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class DiscordSettingsPage
    {
        public DiscordSettingsPage()
        {
            DataContext = new DiscordSettingsViewModel();
            InitializeComponent();
        }
    }
}
