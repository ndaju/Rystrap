using Rystrap.UI.Elements.Base;
using Rystrap.UI.ViewModels.Dialogs;
using System.Windows;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class ThemeManagerDialog : WpfUiWindow
    {
        private readonly ThemeManagerViewModel _viewModel;

        public bool Applied { get; private set; } = false;

        public ThemeManagerDialog()
        {
            _viewModel = new ThemeManagerViewModel();
            DataContext = _viewModel;

            InitializeComponent();
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ApplyThemeCommand.Execute(null);
            Applied = true;
            Close();
        }
    }
}
