using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class DevToolsPage
    {
        public DevToolsPage()
        {
            DataContext = new DevToolsViewModel();
            InitializeComponent();
        }
    }
}
