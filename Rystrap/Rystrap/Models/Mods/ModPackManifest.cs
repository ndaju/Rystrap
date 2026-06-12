using System.Text.Json.Serialization;

namespace Rystrap.Models.Mods
{
    public class ModPackManifest
    {
        [JsonPropertyName("packId")]
        public string PackId { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("author")]
        public string Author { get; set; } = "Unknown";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("mods")]
        public List<ModEntry> Mods { get; set; } = new();

        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; set; } = new();
    }
}
