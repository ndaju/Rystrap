using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystrap.Mods
{
    public class ModPreset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonPropertyName("modified")]
        public DateTime Modified { get; set; } = DateTime.Now;

        [JsonPropertyName("enabledMods")]
        public List<string> EnabledModIds { get; set; } = new();

        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; set; } = new();

        public static string PresetsDirectory => Path.Combine(Paths.Base, "ModPresets");

        public string FileName => $"{Name.Replace(" ", "_")}.json";

        public string FilePath => Path.Combine(PresetsDirectory, FileName);

        public static ModPreset LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Preset file not found: {filePath}");

            string json = File.ReadAllText(filePath);
            var preset = JsonSerializer.Deserialize<ModPreset>(json);

            if (preset is null)
                throw new InvalidDataException("Failed to deserialize preset");

            return preset;
        }

        public void Save()
        {
            Directory.CreateDirectory(PresetsDirectory);

            Modified = DateTime.Now;
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public void Delete()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        public static List<ModPreset> LoadAll()
        {
            var presets = new List<ModPreset>();

            if (!Directory.Exists(PresetsDirectory))
                return presets;

            foreach (string file in Directory.GetFiles(PresetsDirectory, "*.json"))
            {
                try
                {
                    var preset = LoadFromFile(file);
                    presets.Add(preset);
                }
                catch
                {
                    // Skip corrupted preset files
                }
            }

            return presets;
        }
    }
}
