using System.Collections.ObjectModel;
using System.Text.Json;

namespace Rystrap.Models.Persistable
{
    public class GameSettingsDatabase
    {
        public ObservableCollection<GameSetting> Entries { get; set; } = new();

        public static string FilePath => System.IO.Path.Combine(Paths.Base, "GameSettings.json");

        public static GameSettingsDatabase Load()
        {
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<GameSettingsDatabase>(json) ?? new();
                }
                catch { }
            }
            return new();
        }

        public void Save()
        {
            System.IO.File.WriteAllText(FilePath, JsonSerializer.Serialize(this));
        }
    }

    public class GameSetting
    {
        public string PlaceId { get; set; } = "";
        public string PlaceName { get; set; } = "";
        public int FpsCap { get; set; } = 0;
        public int RenderDistance { get; set; } = 0;
        public string GraphicsQuality { get; set; } = "Auto";
        public bool DisableParticles { get; set; } = false;
        public bool DisableLighting { get; set; } = false;
    }
}
