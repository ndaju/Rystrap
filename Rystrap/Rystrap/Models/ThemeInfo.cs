using System.Text.Json.Serialization;

namespace Rystrap.Models
{
    public class ThemeInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("author")]
        public string Author { get; set; } = "";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("previewImage")]
        public string PreviewImage { get; set; } = "";

        [JsonPropertyName("isBuiltIn")]
        public bool IsBuiltIn { get; set; } = false;

        public string DirectoryPath { get; set; } = "";

        public string ManifestPath => Path.Combine(DirectoryPath, "theme.json");

        public bool HasPreviewImage => !string.IsNullOrEmpty(PreviewImage) && File.Exists(Path.Combine(DirectoryPath, PreviewImage));
    }
}
