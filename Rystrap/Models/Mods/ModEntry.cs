using System.Text.Json.Serialization;

namespace Rystrap.Models.Mods
{
    public class ModEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("category")]
        public ModCategory Category { get; set; } = ModCategory.General;

        [JsonPropertyName("sourcePath")]
        public string SourcePath { get; set; } = "";

        [JsonPropertyName("targetPath")]
        public string TargetPath { get; set; } = "";

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; } = false;

        [JsonPropertyName("isBuiltIn")]
        public bool IsBuiltIn { get; set; } = false;

        [JsonPropertyName("previewImagePath")]
        public string? PreviewImagePath { get; set; }

        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; set; } = new();

        // User-selected custom file path (not serialized)
        [JsonIgnore]
        public string CustomFilePath { get; set; } = "";

        // Backward compat aliases for old ModManager
        [JsonIgnore]
        public string Author { get; set; } = "";

        [JsonIgnore]
        public string FilePath { get; set; } = "";

        [JsonIgnore]
        public string SourceFile { get; set; } = "";

        [JsonIgnore]
        public string FileHash { get; set; } = "";

        [JsonIgnore]
        public string Version { get; set; } = "1.0.0";

        [JsonIgnore]
        public List<string> ConflictsWith { get; set; } = new();
    }

    public enum ModCategory
    {
        General,
        Cursor,
        Sound,
        Texture,
        Font,
        Animation,
        UI
    }
}