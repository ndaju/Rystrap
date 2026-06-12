using Rystrap.UI.Elements.Base;
using Rystrap.UI.ViewModels.Dialogs;
using Rystrap.Models;
using System.Windows;
using System.Windows.Controls;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class ModManagerDialog : WpfUiWindow
    {
        private readonly ModManagerViewModel _viewModel;

        public ModManagerDialog()
        {
            _viewModel = new ModManagerViewModel();
            DataContext = _viewModel;

            InitializeComponent();
        }

        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.RefreshMods();
        }

        private void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.RefreshMods();
        }
    }
}
