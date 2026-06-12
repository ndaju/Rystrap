namespace Rystrap.Themes
{
    public class ThemeManager
    {
        private const string LOG_IDENT = "ThemeManager";

        private readonly List<ThemePack> _builtInThemes = new();
        private readonly List<ThemePack> _customThemes = new();
        private ThemePack? _activeTheme;

        public IReadOnlyList<ThemePack> BuiltInThemes => _builtInThemes.AsReadOnly();
        public IReadOnlyList<ThemePack> CustomThemes => _customThemes.AsReadOnly();
        public ThemePack? ActiveTheme => _activeTheme;

        public IEnumerable<ThemePack> AllThemes => _builtInThemes.Concat(_customThemes);

        public ThemeManager()
        {
            LoadBuiltInThemes();
            LoadCustomThemes();
        }

        private void LoadBuiltInThemes()
        {
            _builtInThemes.Add(CreateDarkTheme());
            _builtInThemes.Add(CreateLightTheme());
            _builtInThemes.Add(CreateNeonTheme());
            _builtInThemes.Add(CreateRetroTheme());
            _builtInThemes.Add(CreateMidnightTheme());
            _builtInThemes.Add(CreateSunsetTheme());
        }

        private void LoadCustomThemes()
        {
            Directory.CreateDirectory(Paths.CustomThemes);

            foreach (string directory in Directory.GetDirectories(Paths.CustomThemes))
            {
                string manifestPath = Path.Combine(directory, "theme.json");

                if (!File.Exists(manifestPath))
                    continue;

                try
                {
                    var pack = ThemePack.LoadFromDirectory(directory);
                    pack.Metadata.IsBuiltIn = false;
                    _customThemes.Add(pack);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        public ThemePack? FindTheme(string name)
        {
            return AllThemes.FirstOrDefault(t => t.Metadata.Name == name, null);
        }

        public void ApplyTheme(ThemePack pack)
        {
            const string LOG_IDENT = "ThemeManager::ApplyTheme";

            try
            {
                ThemeResources.ApplyThemeToApplication(pack);
                _activeTheme = pack;

                App.Settings.Prop.SelectedCustomTheme = pack.Metadata.Name;
                App.Settings.Save();

                App.Logger.WriteLine(LOG_IDENT, $"Applied theme: {pack.Metadata.Name}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public void RemoveBuiltInTheme(string name)
        {
            _builtInThemes.RemoveAll(t => t.Metadata.Name == name);
        }

        public void RemoveCustomTheme(string name)
        {
            var pack = _customThemes.FirstOrDefault(t => t.Metadata.Name == name);

            if (pack is not null)
            {
                _customThemes.Remove(pack);

                if (Directory.Exists(pack.Metadata.DirectoryPath))
                    Directory.Delete(pack.Metadata.DirectoryPath, true);
            }
        }

        public void ImportTheme(string zipPath)
        {
            const string LOG_IDENT = "ThemeManager::ImportTheme";

            string tempDir = Path.Combine(Paths.Temp, "ThemeImport_" + Guid.NewGuid().ToString("N"));

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempDir);

                string manifestPath = Path.Combine(tempDir, "theme.json");

                if (!File.Exists(manifestPath))
                    throw new FileNotFoundException("Invalid theme pack: theme.json not found");

                var pack = ThemePack.LoadFromDirectory(tempDir);

                string targetDir = Path.Combine(Paths.CustomThemes, pack.Metadata.Name);

                if (Directory.Exists(targetDir))
                    Directory.Delete(targetDir, true);

                Directory.Move(tempDir, targetDir);

                pack.Metadata.DirectoryPath = targetDir;
                _customThemes.Add(pack);

                App.Logger.WriteLine(LOG_IDENT, $"Imported theme: {pack.Metadata.Name}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);

                throw;
            }
        }

        public void ExportTheme(ThemePack pack, string zipPath)
        {
            const string LOG_IDENT = "ThemeManager::ExportTheme";

            string sourceDir = pack.Metadata.DirectoryPath;

            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Theme directory not found: {sourceDir}");

            System.IO.Compression.ZipFile.CreateFromDirectory(sourceDir, zipPath);

            App.Logger.WriteLine(LOG_IDENT, $"Exported theme: {pack.Metadata.Name} to {zipPath}");
        }

        public ThemePack CreateTheme(string name, string author = "", string description = "")
        {
            string dir = Path.Combine(Paths.CustomThemes, name);

            Directory.CreateDirectory(dir);

            var pack = new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = name,
                    Author = author,
                    Description = description,
                    Version = "1.0.0",
                    DirectoryPath = dir
                }
            };

            pack.SaveToDirectory(dir);
            _customThemes.Add(pack);

            return pack;
        }

        public void RenameTheme(string oldName, string newName)
        {
            var pack = _customThemes.FirstOrDefault(t => t.Metadata.Name == oldName);

            if (pack is null)
                throw new InvalidOperationException($"Theme '{oldName}' not found");

            string oldDir = pack.Metadata.DirectoryPath;
            string newDir = Path.Combine(Paths.CustomThemes, newName);

            if (Directory.Exists(newDir))
                throw new InvalidOperationException($"Theme '{newName}' already exists");

            Directory.Move(oldDir, newDir);

            pack.Metadata.Name = newName;
            pack.Metadata.DirectoryPath = newDir;
            pack.SaveToDirectory(newDir);
        }

        public ThemePreviewData PreviewTheme(ThemePack pack)
        {
            return new ThemePreviewData
            {
                ThemeName = pack.Metadata.Name,
                WindowBackground = pack.Elements.Window?.Background ?? "#1E1E1E",
                WindowForeground = pack.Elements.Window?.Foreground ?? "#FFFFFF",
                ButtonBackground = pack.Elements.Buttons?.Background ?? "#0078D4",
                ButtonForeground = pack.Elements.Buttons?.Foreground ?? "#FFFFFF",
                TextPrimary = pack.Elements.TextBlocks?.PrimaryForeground ?? "#FFFFFF",
                TextSecondary = pack.Elements.TextBlocks?.SecondaryForeground ?? "#B0B0B0",
                CardBackground = pack.Elements.Cards?.Background ?? "#2D2D2D",
                Description = pack.Metadata.Description
            };
        }

        private static ThemePack CreateDarkTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Dark",
                    Author = "Rystrap",
                    Description = "Default dark theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#1E1E1E", Foreground = "#FFFFFF" },
                    TitleBar = new TitleBarTheme { Background = "#2D2D2D", Foreground = "#FFFFFF" },
                    Buttons = new ButtonTheme { Background = "#0078D4", Foreground = "#FFFFFF", HoverBackground = "#1A6FBF" },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#FFFFFF", SecondaryForeground = "#B0B0B0", TertiaryForeground = "#808080" },
                    Cards = new CardTheme { Background = "#2D2D2D", BorderBrush = "#404040" },
                    Inputs = new InputTheme { Background = "#3D3D3D", BorderBrush = "#555555", Foreground = "#FFFFFF" }
                }
            };
        }

        private static ThemePack CreateLightTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Light",
                    Author = "Rystrap",
                    Description = "Clean light theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#F3F3F3", Foreground = "#1A1A1A" },
                    TitleBar = new TitleBarTheme { Background = "#FFFFFF", Foreground = "#1A1A1A" },
                    Buttons = new ButtonTheme { Background = "#0078D4", Foreground = "#FFFFFF", HoverBackground = "#1A6FBF" },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#1A1A1A", SecondaryForeground = "#616161", TertiaryForeground = "#9E9E9E" },
                    Cards = new CardTheme { Background = "#FFFFFF", BorderBrush = "#E0E0E0" },
                    Inputs = new InputTheme { Background = "#FFFFFF", BorderBrush = "#CCCCCC", Foreground = "#1A1A1A" }
                }
            };
        }

        private static ThemePack CreateNeonTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Neon",
                    Author = "Rystrap",
                    Description = "Vibrant neon cyberpunk theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#0A0A1A", Foreground = "#E0E0FF" },
                    TitleBar = new TitleBarTheme { Background = "#0F0F2A", Foreground = "#00FFFF" },
                    Buttons = new ButtonTheme { Background = "#FF00FF", Foreground = "#000000", HoverBackground = "#CC00CC" },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#00FFFF", SecondaryForeground = "#FF00FF", TertiaryForeground = "#8080FF" },
                    Cards = new CardTheme { Background = "#121230", BorderBrush = "#FF00FF" },
                    Inputs = new InputTheme { Background = "#1A1A40", BorderBrush = "#00FFFF", Foreground = "#E0E0FF" }
                }
            };
        }

        private static ThemePack CreateRetroTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Retro",
                    Author = "Rystrap",
                    Description = "Nostalgic retro computing theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#000080", Foreground = "#FFFF00" },
                    TitleBar = new TitleBarTheme { Background = "#0000AA", Foreground = "#FFFFFF" },
                    Buttons = new ButtonTheme { Background = "#C0C0C0", Foreground = "#000000", HoverBackground = "#A0A0A0", CornerRadius = 0 },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#FFFF00", SecondaryForeground = "#00FF00", TertiaryForeground = "#00AAAA" },
                    Cards = new CardTheme { Background = "#0000AA", BorderBrush = "#FFFF00", CornerRadius = 0 },
                    Inputs = new InputTheme { Background = "#000000", BorderBrush = "#C0C0C0", Foreground = "#FFFF00", CornerRadius = 0 }
                }
            };
        }

        private static ThemePack CreateMidnightTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Midnight",
                    Author = "Rystrap",
                    Description = "Deep blue midnight theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#0D1117", Foreground = "#C9D1D9" },
                    TitleBar = new TitleBarTheme { Background = "#161B22", Foreground = "#C9D1D9" },
                    Buttons = new ButtonTheme { Background = "#238636", Foreground = "#FFFFFF", HoverBackground = "#2EA043" },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#C9D1D9", SecondaryForeground = "#8B949E", TertiaryForeground = "#6E7681" },
                    Cards = new CardTheme { Background = "#161B22", BorderBrush = "#30363D" },
                    Inputs = new InputTheme { Background = "#0D1117", BorderBrush = "#30363D", Foreground = "#C9D1D9" }
                }
            };
        }

        private static ThemePack CreateSunsetTheme()
        {
            return new ThemePack
            {
                Metadata = new ThemeInfo
                {
                    Name = "Sunset",
                    Author = "Rystrap",
                    Description = "Warm orange and purple sunset theme",
                    Version = "1.0.0",
                    IsBuiltIn = true
                },
                Elements = new ThemeElements
                {
                    Window = new WindowTheme { Background = "#1A0A1E", Foreground = "#FFE0CC" },
                    TitleBar = new TitleBarTheme { Background = "#2D1033", Foreground = "#FFB366" },
                    Buttons = new ButtonTheme { Background = "#FF6B35", Foreground = "#FFFFFF", HoverBackground = "#E55A25" },
                    TextBlocks = new TextBlockTheme { PrimaryForeground = "#FFE0CC", SecondaryForeground = "#CC9977", TertiaryForeground = "#996644" },
                    Cards = new CardTheme { Background = "#2D1033", BorderBrush = "#FF6B35" },
                    Inputs = new InputTheme { Background = "#250D2B", BorderBrush = "#CC6633", Foreground = "#FFE0CC" }
                }
            };
        }
    }

    public class ThemePreviewData
    {
        public string ThemeName { get; set; } = "";
        public string WindowBackground { get; set; } = "";
        public string WindowForeground { get; set; } = "";
        public string ButtonBackground { get; set; } = "";
        public string ButtonForeground { get; set; } = "";
        public string TextPrimary { get; set; } = "";
        public string TextSecondary { get; set; } = "";
        public string CardBackground { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
