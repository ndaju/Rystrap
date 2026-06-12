using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class PerformancePage
    {
        public PerformancePage()
        {
            DataContext = new PerformanceViewModel();
            InitializeComponent();
        }
    }
}
