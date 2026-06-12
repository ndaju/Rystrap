using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class LaunchOptionsPage
    {
        public LaunchOptionsPage()
        {
            DataContext = new LaunchOptionsViewModel();
            InitializeComponent();
        }
    }
}
