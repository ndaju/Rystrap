using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystrap.Themes
{
    public class ThemePack
    {
        [JsonPropertyName("metadata")]
        public ThemeInfo Metadata { get; set; } = new();

        [JsonPropertyName("elements")]
        public ThemeElements Elements { get; set; } = new();

        [JsonPropertyName("resources")]
        public Dictionary<string, string> Resources { get; set; } = new();

        public static ThemePack LoadFromDirectory(string directory)
        {
            const string LOG_IDENT = "ThemePack::LoadFromDirectory";

            string manifestPath = Path.Combine(directory, "theme.json");

            if (!File.Exists(manifestPath))
                throw new FileNotFoundException($"Theme manifest not found at {manifestPath}");

            string json = File.ReadAllText(manifestPath);
            var pack = JsonSerializer.Deserialize<ThemePack>(json);

            if (pack is null)
                throw new InvalidDataException("Failed to deserialize theme pack");

            pack.Metadata.DirectoryPath = directory;
            return pack;
        }

        public void SaveToDirectory(string directory)
        {
            Directory.CreateDirectory(directory);

            string manifestPath = Path.Combine(directory, "theme.json");
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(manifestPath, json);
        }
    }

    public class ThemeElements
    {
        [JsonPropertyName("titleBar")]
        public TitleBarTheme? TitleBar { get; set; }

        [JsonPropertyName("window")]
        public WindowTheme? Window { get; set; }

        [JsonPropertyName("buttons")]
        public ButtonTheme? Buttons { get; set; }

        [JsonPropertyName("textBlocks")]
        public TextBlockTheme? TextBlocks { get; set; }

        [JsonPropertyName("cards")]
        public CardTheme? Cards { get; set; }

        [JsonPropertyName("inputs")]
        public InputTheme? Inputs { get; set; }
    }

    public class TitleBarTheme
    {
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        [JsonPropertyName("foreground")]
        public string? Foreground { get; set; }

        [JsonPropertyName("height")]
        public double? Height { get; set; }
    }

    public class WindowTheme
    {
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        [JsonPropertyName("foreground")]
        public string? Foreground { get; set; }

        [JsonPropertyName("borderBrush")]
        public string? BorderBrush { get; set; }

        [JsonPropertyName("borderThickness")]
        public double? BorderThickness { get; set; }
    }

    public class ButtonTheme
    {
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        [JsonPropertyName("foreground")]
        public string? Foreground { get; set; }

        [JsonPropertyName("hoverBackground")]
        public string? HoverBackground { get; set; }

        [JsonPropertyName("cornerRadius")]
        public double? CornerRadius { get; set; }
    }

    public class TextBlockTheme
    {
        [JsonPropertyName("primaryForeground")]
        public string? PrimaryForeground { get; set; }

        [JsonPropertyName("secondaryForeground")]
        public string? SecondaryForeground { get; set; }

        [JsonPropertyName("tertiaryForeground")]
        public string? TertiaryForeground { get; set; }
    }

    public class CardTheme
    {
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        [JsonPropertyName("borderBrush")]
        public string? BorderBrush { get; set; }

        [JsonPropertyName("cornerRadius")]
        public double? CornerRadius { get; set; }
    }

    public class InputTheme
    {
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        [JsonPropertyName("borderBrush")]
        public string? BorderBrush { get; set; }

        [JsonPropertyName("foreground")]
        public string? Foreground { get; set; }

        [JsonPropertyName("cornerRadius")]
        public double? CornerRadius { get; set; }
    }
}
