using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

using Rystrap.Themes;

namespace Rystrap.UI.ViewModels.Dialogs
{
    public class ThemeManagerViewModel : NotifyPropertyChangedViewModel
    {
        private readonly ThemeManager _themeManager;

        public ICommand ApplyThemeCommand => new RelayCommand(ApplyTheme);
        public ICommand ImportThemeCommand => new RelayCommand(ImportTheme);
        public ICommand ExportThemeCommand => new RelayCommand(ExportTheme);
        public ICommand DeleteThemeCommand => new RelayCommand(DeleteTheme);
        public ICommand CreateThemeCommand => new RelayCommand(CreateTheme);
        public ICommand RefreshThemesCommand => new RelayCommand(RefreshThemes);

        public ObservableCollection<ThemePack> Themes { get; } = new();

        private ThemePack? _selectedTheme;
        public ThemePack? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged(nameof(SelectedTheme));
                    OnPropertyChanged(nameof(HasSelectedTheme));
                    OnPropertyChanged(nameof(CanDelete));
                    UpdatePreview();
                }
            }
        }

        private string _newThemeName = "";
        public string NewThemeName
        {
            get => _newThemeName;
            set
            {
                if (_newThemeName != value)
                {
                    _newThemeName = value;
                    OnPropertyChanged(nameof(NewThemeName));
                }
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                    OnPropertyChanged(nameof(StatusVisibility));
                }
            }
        }

        private Brush? _previewWindowBackground;
        public Brush? PreviewWindowBackground
        {
            get => _previewWindowBackground;
            set { _previewWindowBackground = value; OnPropertyChanged(nameof(PreviewWindowBackground)); }
        }

        private Brush? _previewWindowForeground;
        public Brush? PreviewWindowForeground
        {
            get => _previewWindowForeground;
            set { _previewWindowForeground = value; OnPropertyChanged(nameof(PreviewWindowForeground)); }
        }

        private Brush? _previewButtonBackground;
        public Brush? PreviewButtonBackground
        {
            get => _previewButtonBackground;
            set { _previewButtonBackground = value; OnPropertyChanged(nameof(PreviewButtonBackground)); }
        }

        private Brush? _previewButtonForeground;
        public Brush? PreviewButtonForeground
        {
            get => _previewButtonForeground;
            set { _previewButtonForeground = value; OnPropertyChanged(nameof(PreviewButtonForeground)); }
        }

        private Brush? _previewCardBackground;
        public Brush? PreviewCardBackground
        {
            get => _previewCardBackground;
            set { _previewCardBackground = value; OnPropertyChanged(nameof(PreviewCardBackground)); }
        }

        private string _previewDescription = "";
        public string PreviewDescription
        {
            get => _previewDescription;
            set { _previewDescription = value; OnPropertyChanged(nameof(PreviewDescription)); }
        }

        public bool HasSelectedTheme => SelectedTheme is not null;
        public Visibility StatusVisibility => string.IsNullOrEmpty(StatusMessage) ? Visibility.Collapsed : Visibility.Visible;
        public bool CanDelete => SelectedTheme is not null && !SelectedTheme.Metadata.IsBuiltIn;

        public ThemeManagerViewModel()
        {
            _themeManager = new ThemeManager();
            RefreshThemeList();
        }

        public void RefreshThemeList()
        {
            Themes.Clear();

            foreach (var theme in _themeManager.AllThemes)
                Themes.Add(theme);
        }

        private void RefreshThemes()
        {
            RefreshThemeList();
            StatusMessage = "Theme list refreshed.";
        }

        private void ApplyTheme()
        {
            if (SelectedTheme is null)
                return;

            _themeManager.ApplyTheme(SelectedTheme);
            StatusMessage = $"Applied theme: {SelectedTheme.Metadata.Name}";
        }

        private void ImportTheme()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Theme Pack (*.zip)|*.zip|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _themeManager.ImportTheme(dialog.FileName);
                RefreshThemeList();
                StatusMessage = "Theme imported successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Import failed: {ex.Message}";
                App.Logger.WriteException("ThemeManagerViewModel::ImportTheme", ex);
            }
        }

        private void ExportTheme()
        {
            if (SelectedTheme is null)
                return;

            var dialog = new SaveFileDialog
            {
                FileName = $"{SelectedTheme.Metadata.Name}.zip",
                Filter = "Theme Pack (*.zip)|*.zip"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _themeManager.ExportTheme(SelectedTheme, dialog.FileName);
                StatusMessage = $"Theme exported to: {dialog.FileName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
                App.Logger.WriteException("ThemeManagerViewModel::ExportTheme", ex);
            }
        }

        private void DeleteTheme()
        {
            if (SelectedTheme is null || SelectedTheme.Metadata.IsBuiltIn)
                return;

            string name = SelectedTheme.Metadata.Name;

            try
            {
                _themeManager.RemoveCustomTheme(name);
                RefreshThemeList();
                StatusMessage = $"Deleted theme: {name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Delete failed: {ex.Message}";
                App.Logger.WriteException("ThemeManagerViewModel::DeleteTheme", ex);
            }
        }

        private void CreateTheme()
        {
            if (string.IsNullOrWhiteSpace(NewThemeName))
            {
                StatusMessage = "Please enter a theme name.";
                return;
            }

            try
            {
                var pack = _themeManager.CreateTheme(NewThemeName.Trim());
                RefreshThemeList();
                NewThemeName = "";
                StatusMessage = $"Created theme: {pack.Metadata.Name}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Create failed: {ex.Message}";
                App.Logger.WriteException("ThemeManagerViewModel::CreateTheme", ex);
            }
        }

        private void UpdatePreview()
        {
            if (SelectedTheme is null)
            {
                PreviewWindowBackground = null;
                PreviewWindowForeground = null;
                PreviewButtonBackground = null;
                PreviewButtonForeground = null;
                PreviewCardBackground = null;
                PreviewDescription = "";
                return;
            }

            var preview = _themeManager.PreviewTheme(SelectedTheme);

            PreviewWindowBackground = ConvertToBrush(preview.WindowBackground);
            PreviewWindowForeground = ConvertToBrush(preview.WindowForeground);
            PreviewButtonBackground = ConvertToBrush(preview.ButtonBackground);
            PreviewButtonForeground = ConvertToBrush(preview.ButtonForeground);
            PreviewCardBackground = ConvertToBrush(preview.CardBackground);
            PreviewDescription = preview.Description;
        }

        private static Brush? ConvertToBrush(string colorValue)
        {
            try
            {
                return new BrushConverter().ConvertFromString(colorValue) as Brush;
            }
            catch
            {
                return null;
            }
        }
    }
}
