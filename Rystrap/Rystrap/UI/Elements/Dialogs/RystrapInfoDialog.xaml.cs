using System.Windows;
using Rystrap.Extensions;
using Rystrap.UI.Elements.Base;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class RystrapInfoDialog : WpfUiWindow
    {
        public RystrapInfoDialog()
        {
            InitializeComponent();
            AppIcon.Source = IconEx.GetImageSourceFromAppResource("pack://application:,,,/Rystrap.ico");
        }

        private void OnGetStartedClick(object sender, RoutedEventArgs e)
        {
            App.State.Prop.FirstRunCompleted = true;
            Close();
        }
    }
}
