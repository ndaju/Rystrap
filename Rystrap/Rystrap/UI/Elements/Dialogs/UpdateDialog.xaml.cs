using System.ComponentModel;
using System.Windows;

using Rystrap.UI.ViewModels.Dialogs;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class UpdateDialog
    {
        public UpdateDialog()
        {
            var viewModel = new UpdateViewModel();
            viewModel.CloseRequested += (_, _) => Close();

            DataContext = viewModel;

            InitializeComponent();
        }

        private void WpfUiWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (DataContext is UpdateViewModel vm && vm.IsDownloading)
            {
                var result = Frontend.ShowMessageBox(
                    Strings.Update_CancelDownloadConfirm,
                    MessageBoxImage.Question,
                    MessageBoxButton.YesNo
                );

                e.Cancel = result != MessageBoxResult.Yes;
            }
        }
    }
}
