using System.Security.Cryptography;
using System.Text.Json;
using Rystrap.Models;

using Rystrap.Models.Mods;
namespace Rystrap.Mods
{
    public class ModManager
    {

        private readonly string _modsDirectory;
        private readonly List<ModEntry> _mods = new();
        private readonly List<ModPreset> _presets = new();

        public IReadOnlyList<ModEntry> Mods => _mods.AsReadOnly();
        public IReadOnlyList<ModPreset> Presets => _presets.AsReadOnly();

        public ModManager()
        {
            _modsDirectory = Path.Combine(Paths.Base, "Mods");
            Directory.CreateDirectory(_modsDirectory);
            Directory.CreateDirectory(ModPreset.PresetsDirectory);

            LoadMods();
            LoadPresets();
        }

        private void LoadMods()
        {
            const string LOG_IDENT = "ModManager::LoadMods";

            string modsManifest = Path.Combine(_modsDirectory, "mods.json");

            if (File.Exists(modsManifest))
            {
                try
                {
                    string json = File.ReadAllText(modsManifest);
                    var loadedMods = JsonSerializer.Deserialize<List<ModEntry>>(json);

                    if (loadedMods is not null)
                        _mods.AddRange(loadedMods);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        private void SaveMods()
        {
            const string LOG_IDENT = "ModManager::SaveMods";

            string modsManifest = Path.Combine(_modsDirectory, "mods.json");

            try
            {
                string json = JsonSerializer.Serialize(_mods, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(modsManifest, json);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        private void LoadPresets()
        {
            _presets.AddRange(ModPreset.LoadAll());
        }

        public ModEntry AddMod(string name, string sourceFilePath, ModCategory category, string targetPath, string author = "", string description = "")
        {
            const string LOG_IDENT = "ModManager::AddMod";

            string modId = Guid.NewGuid().ToString("N");
            string modDir = Path.Combine(_modsDirectory, modId);

            Directory.CreateDirectory(modDir);

            string destFile = Path.Combine(modDir, Path.GetFileName(sourceFilePath));
            File.Copy(sourceFilePath, destFile, true);

            string fileHash = ComputeFileHash(destFile);

            var mod = new ModEntry
            {
                Id = modId,
                Name = name,
                Author = author,
                Description = description,
                Category = category,
                FilePath = destFile,
                FileHash = fileHash,
                TargetPath = targetPath,
                IsEnabled = false
            };

            _mods.Add(mod);
            SaveMods();

            App.Logger.WriteLine(LOG_IDENT, $"Added mod: {name} ({modId})");
            return mod;
        }

        public void RemoveMod(string modId)
        {
            const string LOG_IDENT = "ModManager::RemoveMod";

            var mod = _mods.FirstOrDefault(m => m.Id == modId);

            if (mod is null)
                return;

            if (mod.IsEnabled)
                DisableMod(modId);

            string modDir = Path.Combine(_modsDirectory, modId);

            if (Directory.Exists(modDir))
                Directory.Delete(modDir, true);

            _mods.Remove(mod);
            SaveMods();

            App.Logger.WriteLine(LOG_IDENT, $"Removed mod: {mod.Name} ({modId})");
        }

        public void EnableMod(string modId)
        {
            const string LOG_IDENT = "ModManager::EnableMod";

            var mod = _mods.FirstOrDefault(m => m.Id == modId);

            if (mod is null || mod.IsEnabled)
                return;

            var conflicts = DetectConflicts(mod);

            if (conflicts.Any())
            {
                string conflictNames = string.Join(", ", conflicts.Select(c => c.Name));
                App.Logger.WriteLine(LOG_IDENT, $"Conflicts detected for {mod.Name}: {conflictNames}");
                throw new InvalidOperationException($"Mod conflicts with: {conflictNames}");
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

            Filesystem.AssertReadOnly(destPath);

            if (File.Exists(mod.FilePath))
                File.Copy(mod.FilePath, destPath, true);

            mod.IsEnabled = true;
            SaveMods();

            App.Logger.WriteLine(LOG_IDENT, $"Enabled mod: {mod.Name}");
        }

        public void DisableMod(string modId)
        {
            const string LOG_IDENT = "ModManager::DisableMod";

            var mod = _mods.FirstOrDefault(m => m.Id == modId);

            if (mod is null || !mod.IsEnabled)
                return;

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);

            if (File.Exists(destPath))
            {
                Filesystem.AssertReadOnly(destPath);
                File.Delete(destPath);
            }

            mod.IsEnabled = false;
            SaveMods();

            App.Logger.WriteLine(LOG_IDENT, $"Disabled mod: {mod.Name}");
        }

        public void EnableAllMods()
        {
            const string LOG_IDENT = "ModManager::EnableAllMods";

            foreach (var mod in _mods.Where(m => !m.IsEnabled).ToList())
            {
                try
                {
                    EnableMod(mod.Id);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        public void DisableAllMods()
        {
            foreach (var mod in _mods.Where(m => m.IsEnabled).ToList())
            {
                DisableMod(mod.Id);
            }
        }

        public List<ModEntry> DetectConflicts(ModEntry mod)
        {
            var conflicts = new List<ModEntry>();

            if (!mod.IsEnabled)
            {
                foreach (var existing in _mods.Where(m => m.IsEnabled && m.Id != mod.Id))
                {
                    if (string.Equals(existing.TargetPath, mod.TargetPath, StringComparison.OrdinalIgnoreCase))
                        conflicts.Add(existing);

                    if (mod.ConflictsWith.Contains(existing.Id) || existing.ConflictsWith.Contains(mod.Id))
                        conflicts.Add(existing);
                }
            }

            return conflicts.Distinct().ToList();
        }

        public List<ModConflictInfo> GetAllConflicts()
        {
            var conflicts = new List<ModConflictInfo>();

            var enabledMods = _mods.Where(m => m.IsEnabled).ToList();

            for (int i = 0; i < enabledMods.Count; i++)
            {
                for (int j = i + 1; j < enabledMods.Count; j++)
                {
                    var modA = enabledMods[i];
                    var modB = enabledMods[j];

                    if (string.Equals(modA.TargetPath, modB.TargetPath, StringComparison.OrdinalIgnoreCase) ||
                        modA.ConflictsWith.Contains(modB.Id) ||
                        modB.ConflictsWith.Contains(modA.Id))
                    {
                        conflicts.Add(new ModConflictInfo
                        {
                            ModA = modA,
                            ModB = modB,
                            Reason = string.Equals(modA.TargetPath, modB.TargetPath, StringComparison.OrdinalIgnoreCase)
                                ? "Same target file"
                                : "Declared conflict"
                        });
                    }
                }
            }

            return conflicts;
        }

        public void ApplyModPack(ModPack pack)
        {
            const string LOG_IDENT = "ModManager::ApplyModPack";

            foreach (var entry in pack.Mods)
            {
                try
                {
                    string sourceFile = Path.Combine(Path.GetDirectoryName(pack.Metadata.Name) ?? "", entry.SourceFile);

                    if (!File.Exists(sourceFile))
                    {
                        App.Logger.WriteLine(LOG_IDENT, $"Mod file not found: {sourceFile}");
                        continue;
                    }

                    string destPath = Path.Combine(Paths.Modifications, entry.TargetPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

                    Filesystem.AssertReadOnly(destPath);
                    File.Copy(sourceFile, destPath, true);

                    App.Logger.WriteLine(LOG_IDENT, $"Applied mod: {entry.Name} -> {entry.TargetPath}");
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        public void ImportModPack(string zipPath)
        {
            const string LOG_IDENT = "ModManager::ImportModPack";

            string tempDir = Path.Combine(Paths.Temp, "ModImport_" + Guid.NewGuid().ToString("N"));

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempDir);

                string manifestPath = Path.Combine(tempDir, "modpack.json");

                if (!File.Exists(manifestPath))
                    throw new FileNotFoundException("Invalid mod pack: modpack.json not found");

                var pack = ModPack.LoadFromDirectory(tempDir);

                ApplyModPack(pack);

                App.Logger.WriteLine(LOG_IDENT, $"Imported mod pack: {pack.Metadata.Name}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                throw;
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        // Preset management

        public ModPreset CreatePreset(string name, string description = "")
        {
            var preset = new ModPreset
            {
                Name = name,
                Description = description,
                EnabledModIds = _mods.Where(m => m.IsEnabled).Select(m => m.Id).ToList(),
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            preset.Save();
            _presets.Add(preset);

            return preset;
        }

        public void LoadPreset(ModPreset preset)
        {
            const string LOG_IDENT = "ModManager::LoadPreset";

            DisableAllMods();

            foreach (string modId in preset.EnabledModIds)
            {
                try
                {
                    EnableMod(modId);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        public void DeletePreset(ModPreset preset)
        {
            preset.Delete();
            _presets.Remove(preset);
        }

        public ModPreset? FindPreset(string name)
        {
            return _presets.FirstOrDefault(p => p.Name == name, null);
        }

        // Resolution selection for texture mods

        public static string GetTexturePath(string baseTexturePath, TextureResolution resolution)
        {
            string dir = Path.GetDirectoryName(baseTexturePath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(baseTexturePath);
            string ext = Path.GetExtension(baseTexturePath);

            return resolution switch
            {
                TextureResolution.Low => Path.Combine(dir, $"{filename}_low{ext}"),
                TextureResolution.Medium => Path.Combine(dir, $"{filename}_medium{ext}"),
                TextureResolution.High => baseTexturePath,
                TextureResolution.Ultra => Path.Combine(dir, $"{filename}_ultra{ext}"),
                _ => baseTexturePath
            };
        }

        private static string ComputeFileHash(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }

    }

    public class ModConflictInfo
    {
        public ModEntry ModA { get; set; } = null!;
        public ModEntry ModB { get; set; } = null!;
        public string Reason { get; set; } = "";
    }

    public enum TextureResolution
    {
        Low,
        Medium,
        High,
        Ultra
    }
}
