using System;
using System.Windows;
using Rystrap.Extensions;
using Rystrap.UI.ViewModels.Dialogs;
using Rystrap.UI.ViewModels.Installer;

namespace Rystrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for LaunchMenuDialog.xaml
    /// </summary>
    public partial class LaunchMenuDialog
    {
        public NextAction CloseAction = NextAction.Terminate;

        public LaunchMenuDialog()
        {
            var viewModel = new LaunchMenuViewModel();
            viewModel.CloseWindowRequest += (_, closeAction) =>
            {
                CloseAction = closeAction;
                Close();
            };

            DataContext = viewModel;

            InitializeComponent();

            AppLogo.Source = IconEx.GetImageSourceFromAppResource("pack://application:,,,/Rystrap.ico");
        }
    }
}
