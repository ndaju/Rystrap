using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Rystrap.Models.Mods;
using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class BetterModsPage
    {
        private BetterModsViewModel? _viewModel;
        private bool _suppressToggle;

        public BetterModsPage()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new BetterModsViewModel();
            DataContext = _viewModel;
            await _viewModel.InitializeAsync();
        }

        private void OnCategoryClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string categoryStr
                && Enum.TryParse<ModCategory>(categoryStr, out var category))
            {
                _viewModel!.SelectedCategory = category;
            }
        }

        private void OnModChecked(object sender, RoutedEventArgs e)
        {
            if (_suppressToggle) return;
            if (sender is CheckBox cb && cb.DataContext is ModEntry mod)
                _viewModel?.ToggleModCommand.Execute(mod);
        }

        private void OnModUnchecked(object sender, RoutedEventArgs e)
        {
            if (_suppressToggle) return;
            if (sender is CheckBox cb && cb.DataContext is ModEntry mod)
                _viewModel?.ToggleModCommand.Execute(mod);
        }

        private void OnCheckBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is ModEntry mod)
            {
                _suppressToggle = true;
                cb.IsChecked = mod.IsEnabled;
                _suppressToggle = false;
            }
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ModEntry mod)
            {
                _viewModel?.BrowseCustomFileCommand.Execute(mod);
            }
        }
    }

    public class ZeroToVisibilityConverter : IValueConverter
    {
        public static readonly ZeroToVisibilityConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int count && count == 0 ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class FilePathDisplayConverter : IValueConverter
    {
        public static readonly FilePathDisplayConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string path && !string.IsNullOrEmpty(path) ? $"Using: {Path.GetFileName(path)}" : "No custom file";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
