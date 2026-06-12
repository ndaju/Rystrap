using System.Windows;
using System.Windows.Media;

namespace Rystrap.Themes
{
    public static class ThemeResources
    {
        private static readonly Dictionary<string, ResourceDictionary> _cache = new();

        public static ResourceDictionary BuildResourceDictionary(ThemePack pack)
        {
            string cacheKey = pack.Metadata.Name;

            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;

            var dict = new ResourceDictionary();

            ApplyWindowTheme(dict, pack.Elements.Window);
            ApplyTitleBarTheme(dict, pack.Elements.TitleBar);
            ApplyButtonTheme(dict, pack.Elements.Buttons);
            ApplyTextBlockTheme(dict, pack.Elements.TextBlocks);
            ApplyCardTheme(dict, pack.Elements.Cards);
            ApplyInputTheme(dict, pack.Elements.Inputs);

            _cache[cacheKey] = dict;
            return dict;
        }

        public static void ApplyThemeToApplication(ThemePack pack)
        {
            var dict = BuildResourceDictionary(pack);

            const int customThemeIndex = 2;

            if (Application.Current.Resources.MergedDictionaries.Count > customThemeIndex)
                Application.Current.Resources.MergedDictionaries[customThemeIndex] = dict;
            else
                Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public static void ClearThemeCache()
        {
            _cache.Clear();
        }

        private static void ApplyWindowTheme(ResourceDictionary dict, WindowTheme? theme)
        {
            if (theme is null) return;

            if (theme.Background is not null)
                TryAddBrush(dict, "ApplicationBackgroundBrush", theme.Background);

            if (theme.Foreground is not null)
                TryAddBrush(dict, "TextFillColorPrimaryBrush", theme.Foreground);
        }

        private static void ApplyTitleBarTheme(ResourceDictionary dict, TitleBarTheme? theme)
        {
            if (theme is null) return;

            if (theme.Background is not null)
                TryAddBrush(dict, "TitleBarBackgroundBrush", theme.Background);

            if (theme.Foreground is not null)
                TryAddBrush(dict, "TitleBarForegroundBrush", theme.Foreground);
        }

        private static void ApplyButtonTheme(ResourceDictionary dict, ButtonTheme? theme)
        {
            if (theme is null) return;

            if (theme.Background is not null)
                TryAddBrush(dict, "ButtonBackgroundBrush", theme.Background);

            if (theme.Foreground is not null)
                TryAddBrush(dict, "ButtonForegroundBrush", theme.Foreground);

            if (theme.HoverBackground is not null)
                TryAddBrush(dict, "ButtonHoverBackgroundBrush", theme.HoverBackground);
        }

        private static void ApplyTextBlockTheme(ResourceDictionary dict, TextBlockTheme? theme)
        {
            if (theme is null) return;

            if (theme.PrimaryForeground is not null)
                TryAddBrush(dict, "TextFillColorPrimaryBrush", theme.PrimaryForeground);

            if (theme.SecondaryForeground is not null)
                TryAddBrush(dict, "TextFillColorSecondaryBrush", theme.SecondaryForeground);

            if (theme.TertiaryForeground is not null)
                TryAddBrush(dict, "TextFillColorTertiaryBrush", theme.TertiaryForeground);
        }

        private static void ApplyCardTheme(ResourceDictionary dict, CardTheme? theme)
        {
            if (theme is null) return;

            if (theme.Background is not null)
                TryAddBrush(dict, "CardBackgroundBrush", theme.Background);

            if (theme.BorderBrush is not null)
                TryAddBrush(dict, "CardBorderBrush", theme.BorderBrush);
        }

        private static void ApplyInputTheme(ResourceDictionary dict, InputTheme? theme)
        {
            if (theme is null) return;

            if (theme.Background is not null)
                TryAddBrush(dict, "TextBoxBackgroundBrush", theme.Background);

            if (theme.BorderBrush is not null)
                TryAddBrush(dict, "TextBoxBorderBrush", theme.BorderBrush);

            if (theme.Foreground is not null)
                TryAddBrush(dict, "TextBoxForegroundBrush", theme.Foreground);
        }

        private static void TryAddBrush(ResourceDictionary dict, string key, string colorValue)
        {
            try
            {
                var converter = new BrushConverter();
                var brush = converter.ConvertFromString(colorValue) as Brush;

                if (brush is not null)
                    dict[key] = brush;
            }
            catch
            {
                // Invalid color string, skip
            }
        }
    }
}
