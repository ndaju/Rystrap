using System.Text.Json;
using System.Text.Json.Serialization;
using Rystrap.Models.Mods;

namespace Rystrap.Mods
{
    public class ModPack
    {
        [JsonPropertyName("metadata")]
        public ModPackMetadata Metadata { get; set; } = new();

        [JsonPropertyName("mods")]
        public List<ModEntry> Mods { get; set; } = new();

        public static ModPack LoadFromDirectory(string directory)
        {
            const string LOG_IDENT = "ModPack::LoadFromDirectory";

            string manifestPath = Path.Combine(directory, "modpack.json");

            if (!File.Exists(manifestPath))
                throw new FileNotFoundException($"Mod pack manifest not found at {manifestPath}");

            string json = File.ReadAllText(manifestPath);
            var pack = JsonSerializer.Deserialize<ModPack>(json);

            if (pack is null)
                throw new InvalidDataException("Failed to deserialize mod pack");

            return pack;
        }

        public void SaveToDirectory(string directory)
        {
            Directory.CreateDirectory(directory);

            string manifestPath = Path.Combine(directory, "modpack.json");
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(manifestPath, json, new System.Text.UTF8Encoding(true));
        }
    }

    public class ModPackMetadata
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("author")]
        public string Author { get; set; } = "";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }
}