using System.Windows;

using Rystrap.UI.Elements.Base;
using Rystrap.UI.ViewModels.Dialogs;

namespace Rystrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportModPackDialog.xaml
    /// </summary>
    public partial class ImportModPackDialog : WpfUiWindow
    {
        private readonly ImportModPackViewModel _viewModel;

        public bool Imported { get; private set; } = false;

        public ImportModPackDialog(string zipPath)
        {
            _viewModel = new ImportModPackViewModel(zipPath);
            DataContext = _viewModel;

            _viewModel.InstallCompleted += OnInstallCompleted;

            InitializeComponent();
        }

        private void OnInstallCompleted(object? sender, bool success)
        {
            if (success)
            {
                Imported = true;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }

            Close();
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
