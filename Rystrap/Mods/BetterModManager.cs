using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text.Json;

using Rystrap.Models.Mods;

namespace Rystrap.Mods
{
    public class BetterModManager
    {
        private readonly string _modsDirectory;
        private readonly string _builtInModsDirectory;
        private readonly string _customModsDirectory;
        private readonly string _stateFilePath;
        private readonly List<Models.Mods.ModEntry> _allMods = new();
        private readonly List<Models.Mods.ModEntry> _enabledMods = new();

        public ObservableCollection<Models.Mods.ModEntry> AllMods => new(_allMods);
        public ObservableCollection<Models.Mods.ModEntry> EnabledMods => new(_enabledMods);

        public BetterModManager()
        {
            _modsDirectory = Path.Combine(Paths.Base, "BetterMods");
            _builtInModsDirectory = Path.Combine(_modsDirectory, "BuiltIn");
            _customModsDirectory = Path.Combine(_modsDirectory, "Custom");
            _stateFilePath = Path.Combine(_modsDirectory, "enabled_mods.json");

            try
            {
                Directory.CreateDirectory(_builtInModsDirectory);
                Directory.CreateDirectory(_customModsDirectory);
            }
            catch { }
        }

        public void LoadCustomMods()
        {
            LoadCustomMods(_customModsDirectory);
        }

        public void LoadBuiltInMods()
        {
            const string LOG_IDENT = "BetterModManager::LoadBuiltInMods";

            // Add built-in cursor packs
            AddBuiltInCursorPacks();

            // Add built-in sound presets
            AddBuiltInSoundPresets();

            // Add built-in texture presets
            AddBuiltInTexturePresets();

            // Add built-in font presets
            AddBuiltInFontPresets();

            // Add built-in animation mods
            AddBuiltInAnimationMods();

            // Add built-in UI mods
            AddBuiltInUIMods();

            App.Logger.WriteLine(LOG_IDENT, $"Loaded {_allMods.Count} built-in mods");
        }

        private void AddBuiltInCursorPacks()
        {
            string[] cursorPacks = { "Old 2006", "Old 2013", "Classic", "Modern" };

            foreach (string packName in cursorPacks)
            {
                string packDir = Path.Combine(_builtInModsDirectory, "Cursors", packName);
                Directory.CreateDirectory(packDir);

                string packId = $"cursor_{packName.ToLower().Replace(" ", "_")}";

                // Check if pack already exists
                if (_allMods.Any(m => m.Id == packId))
                    continue;

                var mod = new Models.Mods.ModEntry
                {
                    Id = packId,
                    Name = $"{packName} Cursor Pack",
                    Description = $"Restore the {packName} cursor style for Roblox",
                    Category = ModCategory.Cursor,
                    SourcePath = packDir,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(packDir, "preview.png"),
                    Settings = new Dictionary<string, string>
                    {
                        { "PackName", packName },
                        { "ArrowCursor", $"{packName}.ArrowCursor.png" },
                        { "ArrowFarCursor", $"{packName}.ArrowFarCursor.png" }
                    }
                };

                // Set target paths for all cursor files
                foreach (string cursorPath in ModCatalog.KnownCursorPaths)
                {
                    string targetFile = Path.GetFileName(cursorPath);
                    mod.Settings[targetFile] = $"{packName}.{targetFile}";
                }

                _allMods.Add(mod);
            }
        }

        private void AddBuiltInSoundPresets()
        {
            string[] soundPresets = { "Classic 2006", "Retro 2013", "Modern Default", "Silent" };

            foreach (string presetName in soundPresets)
            {
                string presetDir = Path.Combine(_builtInModsDirectory, "Sounds", presetName);
                Directory.CreateDirectory(presetDir);

                string presetId = $"sound_{presetName.ToLower().Replace(" ", "_")}";

                if (_allMods.Any(m => m.Id == presetId))
                    continue;

                var mod = new Models.Mods.ModEntry
                {
                    Id = presetId,
                    Name = $"{presetName} Sound Pack",
                    Description = $"Replace all game sounds with the {presetName} preset",
                    Category = ModCategory.Sound,
                    SourcePath = presetDir,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(presetDir, "preview.png"),
                    Settings = new Dictionary<string, string>
                    {
                        { "PresetName", presetName },
                        { "ReplaceAll", "true" }
                    }
                };

                _allMods.Add(mod);
            }

            // Add individual sound mods from catalog
            foreach (var soundEntry in ModCatalog.KnownSounds)
            {
                string soundId = $"sound_{soundEntry.FriendlyName.ToLower().Replace(" ", "_")}";

                if (_allMods.Any(m => m.Id == soundId))
                    continue;

                string soundDir = Path.Combine(_builtInModsDirectory, "Sounds", soundEntry.FriendlyName);
                Directory.CreateDirectory(soundDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = soundId,
                    Name = soundEntry.FriendlyName,
                    Description = soundEntry.Description,
                    Category = soundEntry.Category,
                    SourcePath = soundDir,
                    TargetPath = soundEntry.TargetPath,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(soundDir, "preview.png")
                };

                _allMods.Add(mod);
            }
        }

        private void AddBuiltInTexturePresets()
        {
            string[] texturePresets = { "Clean", "Minimalist", "Classic", "HD Retexture" };

            foreach (string presetName in texturePresets)
            {
                string presetDir = Path.Combine(_builtInModsDirectory, "Textures", presetName);
                Directory.CreateDirectory(presetDir);

                string presetId = $"texture_{presetName.ToLower().Replace(" ", "_")}";

                if (_allMods.Any(m => m.Id == presetId))
                    continue;

                var mod = new Models.Mods.ModEntry
                {
                    Id = presetId,
                    Name = $"{presetName} Texture Pack",
                    Description = $"Replace game textures with the {presetName} style",
                    Category = ModCategory.Texture,
                    SourcePath = presetDir,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(presetDir, "preview.png"),
                    Settings = new Dictionary<string, string>
                    {
                        { "PresetName", presetName }
                    }
                };

                _allMods.Add(mod);
            }

            // Add individual texture categories
            string[] textureCategories = { "Sky", "Terrain", "UI" };
            foreach (string category in textureCategories)
            {
                string catId = $"texture_{category.ToLower()}";

                if (_allMods.Any(m => m.Id == catId))
                    continue;

                string catDir = Path.Combine(_builtInModsDirectory, "Textures", category);
                Directory.CreateDirectory(catDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = catId,
                    Name = $"{category} Textures",
                    Description = $"Replace all {category.ToLower()} textures",
                    Category = ModCategory.Texture,
                    SourcePath = catDir,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(catDir, "preview.png")
                };

                _allMods.Add(mod);
            }

            // Add individual texture mods from catalog
            foreach (string texturePath in ModCatalog.KnownTexturePaths)
            {
                string textureName = Path.GetFileNameWithoutExtension(texturePath);
                string textureId = $"texture_{textureName.ToLower().Replace(" ", "_").Replace("-", "_")}";

                if (_allMods.Any(m => m.Id == textureId))
                    continue;

                string textureDir = Path.Combine(_builtInModsDirectory, "Textures", textureName);
                Directory.CreateDirectory(textureDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = textureId,
                    Name = textureName,
                    Description = $"Replace {textureName} texture",
                    Category = ModCategory.Texture,
                    SourcePath = textureDir,
                    TargetPath = texturePath,
                    IsBuiltIn = true
                };

                _allMods.Add(mod);
            }
        }

        private void AddBuiltInFontPresets()
        {
            string[] fontPresets = { "Source Sans Pro", "Noto Sans", "Roboto", "Open Sans" };

            foreach (string presetName in fontPresets)
            {
                string presetDir = Path.Combine(_builtInModsDirectory, "Fonts", presetName);
                Directory.CreateDirectory(presetDir);

                string presetId = $"font_{presetName.ToLower().Replace(" ", "_")}";

                if (_allMods.Any(m => m.Id == presetId))
                    continue;

                var mod = new Models.Mods.ModEntry
                {
                    Id = presetId,
                    Name = $"{presetName} Font Pack",
                    Description = $"Replace game fonts with {presetName}",
                    Category = ModCategory.Font,
                    SourcePath = presetDir,
                    IsBuiltIn = true,
                    PreviewImagePath = Path.Combine(presetDir, "preview.png"),
                    Settings = new Dictionary<string, string>
                    {
                        { "FontFamily", presetName }
                    }
                };

                _allMods.Add(mod);
            }

            // Add individual font mods from catalog
            foreach (string fontPath in ModCatalog.KnownFontPaths)
            {
                string fontName = Path.GetFileNameWithoutExtension(fontPath);
                string fontId = $"font_{fontName.ToLower().Replace(" ", "_").Replace("-", "_")}";

                if (_allMods.Any(m => m.Id == fontId))
                    continue;

                string fontDir = Path.Combine(_builtInModsDirectory, "Fonts", fontName);
                Directory.CreateDirectory(fontDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = fontId,
                    Name = fontName,
                    Description = $"Replace {fontName} font file",
                    Category = ModCategory.Font,
                    SourcePath = fontDir,
                    TargetPath = fontPath,
                    IsBuiltIn = true
                };

                _allMods.Add(mod);
            }
        }

        private void AddBuiltInAnimationMods()
        {
            foreach (string animPath in ModCatalog.KnownAnimationPaths)
            {
                string animName = Path.GetFileNameWithoutExtension(animPath);
                string animId = $"anim_{animName.ToLower().Replace(" ", "_").Replace("-", "_")}";

                if (_allMods.Any(m => m.Id == animId))
                    continue;

                string animDir = Path.Combine(_builtInModsDirectory, "Animations", animName);
                Directory.CreateDirectory(animDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = animId,
                    Name = animName,
                    Description = $"Replace {animName} animation",
                    Category = ModCategory.Animation,
                    SourcePath = animDir,
                    TargetPath = animPath,
                    IsBuiltIn = true
                };

                _allMods.Add(mod);
            }
        }

        private void AddBuiltInUIMods()
        {
            foreach (string uiPath in ModCatalog.KnownUIPaths)
            {
                string uiName = Path.GetFileNameWithoutExtension(uiPath);
                string uiId = $"ui_{uiName.ToLower().Replace(" ", "_").Replace("-", "_")}";

                if (_allMods.Any(m => m.Id == uiId))
                    continue;

                string uiDir = Path.Combine(_builtInModsDirectory, "UI", uiName);
                Directory.CreateDirectory(uiDir);

                var mod = new Models.Mods.ModEntry
                {
                    Id = uiId,
                    Name = uiName,
                    Description = $"Replace {uiName} UI element",
                    Category = ModCategory.UI,
                    SourcePath = uiDir,
                    TargetPath = uiPath,
                    IsBuiltIn = true
                };

                _allMods.Add(mod);
            }
        }

        public void LoadCustomMods(string directory)
        {
            const string LOG_IDENT = "BetterModManager::LoadCustomMods";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                return;
            }

            foreach (string modDir in Directory.GetDirectories(directory))
            {
                try
                {
                    string manifestPath = Path.Combine(modDir, "mod.json");
                    if (!File.Exists(manifestPath))
                        continue;

                    string json = File.ReadAllText(manifestPath);
                    var mod = JsonSerializer.Deserialize<Models.Mods.ModEntry>(json);

                    if (mod is null)
                        continue;

                    mod.SourcePath = modDir;
                    mod.IsBuiltIn = false;

                    // Auto-discover custom file from custom/ subdirectory
                    string customDir = Path.Combine(modDir, "custom");
                    if (Directory.Exists(customDir))
                    {
                        string[] customFiles = Directory.GetFiles(customDir);
                        if (customFiles.Length > 0)
                            mod.CustomFilePath = customFiles[0];
                    }

                    var existing = _allMods.FirstOrDefault(m => m.Id == mod.Id);
                    if (existing is not null)
                    {
                        // Update existing mod with custom file path if we found one
                        if (!string.IsNullOrEmpty(mod.CustomFilePath))
                            existing.CustomFilePath = mod.CustomFilePath;
                    }
                    else
                    {
                        _allMods.Add(mod);
                    }
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }

        public void ToggleMod(string modId)
        {
            var mod = _allMods.FirstOrDefault(m => m.Id == modId);
            if (mod is null)
                return;

            if (mod.IsEnabled)
                DisableMod(modId);
            else
                EnableMod(modId);
        }

        public void EnableMod(string modId)
        {
            const string LOG_IDENT = "BetterModManager::EnableMod";

            var mod = _allMods.FirstOrDefault(m => m.Id == modId);
            if (mod is null || mod.IsEnabled)
                return;

            mod.IsEnabled = true;

            if (!_enabledMods.Any(m => m.Id == modId))
                _enabledMods.Add(mod);

            SaveEnabledState();

            App.Logger.WriteLine(LOG_IDENT, $"Enabled mod: {mod.Name}");
        }

        public void DisableMod(string modId)
        {
            const string LOG_IDENT = "BetterModManager::DisableMod";

            var mod = _allMods.FirstOrDefault(m => m.Id == modId);
            if (mod is null || !mod.IsEnabled)
                return;

            mod.IsEnabled = false;
            _enabledMods.RemoveAll(m => m.Id == modId);

            SaveEnabledState();

            App.Logger.WriteLine(LOG_IDENT, $"Disabled mod: {mod.Name}");
        }

        public void EnableAllModsInCategory(Models.Mods.ModCategory category)
        {
            var modsInCategory = _allMods.Where(m => m.Category == category && !m.IsEnabled).ToList();
            foreach (var mod in modsInCategory)
            {
                EnableMod(mod.Id);
            }
        }

        public void DisableAllModsInCategory(Models.Mods.ModCategory category)
        {
            var modsInCategory = _allMods.Where(m => m.Category == category && m.IsEnabled).ToList();
            foreach (var mod in modsInCategory)
            {
                DisableMod(mod.Id);
            }
        }

        public (int success, int failed, List<string> errors) ApplyMods()
        {
            const string LOG_IDENT = "BetterModManager::ApplyMods";

            int success = 0;
            int failed = 0;
            var errors = new List<string>();

            foreach (var mod in _enabledMods.ToList())
            {
                try
                {
                    bool hasCustomFile = !string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath);
                    bool alreadyApplied = !string.IsNullOrEmpty(mod.TargetPath) && File.Exists(Path.Combine(Paths.Modifications, mod.TargetPath));

                    if (!hasCustomFile && !alreadyApplied)
                    {
                        errors.Add($"{mod.Name}: No custom file selected");
                        failed++;
                        continue;
                    }

                    ApplySingleMod(mod);
                    success++;
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                    errors.Add($"{mod.Name}: {ex.Message}");
                    failed++;
                }
            }

            return (success, failed, errors);
        }

        private void ApplySingleMod(Models.Mods.ModEntry mod)
        {
            const string LOG_IDENT = "BetterModManager::ApplySingleMod";

            if (mod.Category == ModCategory.Cursor)
                ApplyCursorMod(mod);
            else if (mod.Category == ModCategory.Sound)
                ApplySoundMod(mod);
            else if (mod.Category == ModCategory.Texture)
                ApplyTextureMod(mod);
            else if (mod.Category == ModCategory.Font)
                ApplyFontMod(mod);
            else if (mod.Category == ModCategory.Animation)
                ApplyAnimationMod(mod);
            else if (mod.Category == ModCategory.UI)
                ApplyUIMod(mod);
        }

        private void ApplyCursorMod(Models.Mods.ModEntry mod)
        {
            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                // User provided a custom cursor file - apply to ALL cursor paths
                string ext = Path.GetExtension(mod.CustomFilePath);
                foreach (string cursorPath in ModCatalog.KnownCursorPaths)
                {
                    string destPath = Path.Combine(Paths.Modifications, cursorPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                    Filesystem.AssertReadOnly(destPath);
                    File.Copy(mod.CustomFilePath, destPath, true);
                }
                return;
            }

            foreach (string cursorPath in ModCatalog.KnownCursorPaths)
            {
                string fileName = Path.GetFileName(cursorPath);
                string sourceFile = Path.Combine(mod.SourcePath, mod.Settings.ContainsKey(fileName) ? mod.Settings[fileName] : fileName);
                string destPath = Path.Combine(Paths.Modifications, cursorPath);

                if (File.Exists(sourceFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                    Filesystem.AssertReadOnly(destPath);
                    File.Copy(sourceFile, destPath, true);
                }
            }
        }

        private void ApplySoundMod(Models.Mods.ModEntry mod)
        {
            if (string.IsNullOrEmpty(mod.TargetPath))
                return;

            string sourceFile = "";

            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                sourceFile = mod.CustomFilePath;
            }
            else
            {
                // Try source directory
                var candidates = new[]
                {
                    Path.Combine(mod.SourcePath, "sound.mp3"),
                    Path.Combine(mod.SourcePath, "sound.ogg"),
                };
                foreach (var c in candidates)
                {
                    if (File.Exists(c))
                    {
                        sourceFile = c;
                        break;
                    }
                }

                // Try custom/ subdirectory of SourcePath
                if (string.IsNullOrEmpty(sourceFile))
                {
                    string customDir = Path.Combine(mod.SourcePath, "custom");
                    if (Directory.Exists(customDir))
                    {
                        var files = Directory.GetFiles(customDir);
                        if (files.Length > 0)
                            sourceFile = files[0];
                    }
                }

                // Try custom mods directory by mod ID
                if (string.IsNullOrEmpty(sourceFile))
                {
                    string customDir = Path.Combine(_customModsDirectory, mod.Id, "custom");
                    if (Directory.Exists(customDir))
                    {
                        var files = Directory.GetFiles(customDir);
                        if (files.Length > 0)
                            sourceFile = files[0];
                    }
                }

                if (string.IsNullOrEmpty(sourceFile))
                    return;
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            Filesystem.AssertReadOnly(destPath);
            File.Copy(sourceFile, destPath, true);
        }

        private void ApplyTextureMod(Models.Mods.ModEntry mod)
        {
            if (string.IsNullOrEmpty(mod.TargetPath))
                return;

            string sourceFile;

            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                sourceFile = mod.CustomFilePath;
            }
            else
            {
                sourceFile = Path.Combine(mod.SourcePath, Path.GetFileName(mod.TargetPath));
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);

            if (File.Exists(sourceFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                Filesystem.AssertReadOnly(destPath);
                File.Copy(sourceFile, destPath, true);
            }
        }

        private void ApplyFontMod(Models.Mods.ModEntry mod)
        {
            if (string.IsNullOrEmpty(mod.TargetPath))
                return;

            string sourceFile;

            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                sourceFile = mod.CustomFilePath;
            }
            else
            {
                sourceFile = Path.Combine(mod.SourcePath, Path.GetFileName(mod.TargetPath));
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);

            if (File.Exists(sourceFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                Filesystem.AssertReadOnly(destPath);
                File.Copy(sourceFile, destPath, true);
            }
        }

        private void ApplyAnimationMod(Models.Mods.ModEntry mod)
        {
            if (string.IsNullOrEmpty(mod.TargetPath))
                return;

            string sourceFile;

            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                sourceFile = mod.CustomFilePath;
            }
            else
            {
                sourceFile = Path.Combine(mod.SourcePath, Path.GetFileName(mod.TargetPath));
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);

            if (File.Exists(sourceFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                Filesystem.AssertReadOnly(destPath);
                File.Copy(sourceFile, destPath, true);
            }
        }

        private void ApplyUIMod(Models.Mods.ModEntry mod)
        {
            if (string.IsNullOrEmpty(mod.TargetPath))
                return;

            string sourceFile;

            if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
            {
                sourceFile = mod.CustomFilePath;
            }
            else
            {
                sourceFile = Path.Combine(mod.SourcePath, Path.GetFileName(mod.TargetPath));
            }

            string destPath = Path.Combine(Paths.Modifications, mod.TargetPath);

            if (File.Exists(sourceFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                Filesystem.AssertReadOnly(destPath);
                File.Copy(sourceFile, destPath, true);
            }
        }

        public void SetCustomFile(string modId, string filePath)
        {
            var mod = _allMods.FirstOrDefault(m => m.Id == modId);
            if (mod is not null)
                mod.CustomFilePath = filePath;
        }

        public string GetCustomFile(string modId)
        {
            var mod = _allMods.FirstOrDefault(m => m.Id == modId);
            return mod?.CustomFilePath ?? "";
        }

        public void ExportModPack(string title, string description, string author, string filePath)
        {
            const string LOG_IDENT = "BetterModManager::ExportModPack";

            var manifest = new ModPackManifest
            {
                Title = title,
                Description = description,
                Author = author,
                CreatedAt = DateTime.UtcNow,
                Mods = new List<Models.Mods.ModEntry>(_enabledMods),
                Settings = new Dictionary<string, string>
                {
                    { "ExportedWith", "Rystrap" },
                    { "ExportDate", DateTime.UtcNow.ToString("o") }
                }
            };

            try
            {
                using var archive = ZipFile.Open(filePath, ZipArchiveMode.Create);

                // Write manifest
                string manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
                var manifestEntry = archive.CreateEntry("modpack.json");
                using (var stream = manifestEntry.Open())
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(manifestJson);
                }

                // Copy mod files
                foreach (var mod in manifest.Mods)
                {
                    // Copy custom user files (browsed PNG, MP3, etc.)
                    if (!string.IsNullOrEmpty(mod.CustomFilePath) && File.Exists(mod.CustomFilePath))
                    {
                        string customEntryPath = $"mods/{mod.Id}/custom/{Path.GetFileName(mod.CustomFilePath)}";
                        archive.CreateEntryFromFile(mod.CustomFilePath, customEntryPath);
                    }

                    // Copy built-in source files
                    if (!string.IsNullOrEmpty(mod.SourcePath) && Directory.Exists(mod.SourcePath))
                    {
                        string modDirInZip = $"mods/{mod.Id}/source";

                        foreach (string file in Directory.GetFiles(mod.SourcePath, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = file[(mod.SourcePath.Length + 1)..];
                            string entryPath = $"{modDirInZip}/{relativePath}";

                            archive.CreateEntryFromFile(file, entryPath);
                        }
                    }
                }

                App.Logger.WriteLine(LOG_IDENT, $"Exported mod pack: {title} to {filePath}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                throw;
            }
        }

        public ModPackManifest? ImportModPack(string zipPath)
        {
            const string LOG_IDENT = "BetterModManager::ImportModPack";

            string tempDir = Path.Combine(Paths.Temp, "ModImport_" + Guid.NewGuid().ToString("N"));

            try
            {
                ZipFile.ExtractToDirectory(zipPath, tempDir);

                string manifestPath = Path.Combine(tempDir, "modpack.json");

                if (!File.Exists(manifestPath))
                    throw new FileNotFoundException("Invalid mod pack: modpack.json not found");

                string manifestJson = File.ReadAllText(manifestPath);
                var manifest = JsonSerializer.Deserialize<ModPackManifest>(manifestJson);

                if (manifest is null)
                    throw new InvalidDataException("Failed to deserialize mod pack manifest");

                // Import mod files
                string modsDir = Path.Combine(tempDir, "mods");
                foreach (var mod in manifest.Mods)
                {
                    string sourceModDir = Path.Combine(modsDir, mod.Id);
                    string destModDir = Path.Combine(_customModsDirectory, mod.Id);

                    if (!Directory.Exists(sourceModDir))
                        continue;

                    Directory.CreateDirectory(destModDir);

                    // Restore custom user files
                    string customDir = Path.Combine(sourceModDir, "custom");
                    if (Directory.Exists(customDir))
                    {
                        string destCustomDir = Path.Combine(destModDir, "custom");
                        Directory.CreateDirectory(destCustomDir);
                        foreach (string file in Directory.GetFiles(customDir))
                        {
                            File.Copy(file, Path.Combine(destCustomDir, Path.GetFileName(file)), true);
                        }
                    }

                    // Restore source files
                    string sourceDir = Path.Combine(sourceModDir, "source");
                    if (Directory.Exists(sourceDir))
                    {
                        CopyDirectory(sourceDir, destModDir);
                    }

                    // Write mod.json
                    mod.SourcePath = destModDir;
                    mod.IsBuiltIn = false;
                    string modJsonPath = Path.Combine(destModDir, "mod.json");
                    string modJson = JsonSerializer.Serialize(mod, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(modJsonPath, modJson);
                }

                // Reload mod lists
                _allMods.Clear();
                _enabledMods.Clear();
                LoadBuiltInMods();
                LoadCustomMods();
                LoadEnabledState();

                // For each imported mod: find it in _allMods, set CustomFilePath, enable it
                foreach (var manifestMod in manifest.Mods)
                {
                    var existingMod = _allMods.FirstOrDefault(m => m.Id == manifestMod.Id);
                    if (existingMod is null)
                        continue;

                    // Auto-discover custom file from the custom directory
                    string customDir = Path.Combine(_customModsDirectory, manifestMod.Id, "custom");
                    if (Directory.Exists(customDir))
                    {
                        string[] files = Directory.GetFiles(customDir);
                        if (files.Length > 0)
                            existingMod.CustomFilePath = files[0];
                    }

                    // Enable the mod
                    if (!existingMod.IsEnabled)
                    {
                        existingMod.IsEnabled = true;
                        if (!_enabledMods.Any(m => m.Id == existingMod.Id))
                            _enabledMods.Add(existingMod);
                    }
                }

                SaveEnabledState();

                App.Logger.WriteLine(LOG_IDENT, $"Imported mod pack: {manifest.Title}");
                return manifest;
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

        public List<Models.Mods.ModEntry> GetModsByCategory(Models.Mods.ModCategory category)
        {
            return _allMods.Where(m => m.Category == category).ToList();
        }

        public List<Models.Mods.ModEntry> GetAllMods()
        {
            return new List<Models.Mods.ModEntry>(_allMods);
        }

        public List<Models.Mods.ModEntry> GetEnabledMods()
        {
            return new List<Models.Mods.ModEntry>(_enabledMods);
        }

        public Models.Mods.ModEntry? GetModById(string modId)
        {
            return _allMods.FirstOrDefault(m => m.Id == modId);
        }

        public int GetEnabledCount(Models.Mods.ModCategory? category = null)
        {
            if (category.HasValue)
                return _enabledMods.Count(m => m.Category == category.Value);

            return _enabledMods.Count;
        }

        public int GetTotalCount(Models.Mods.ModCategory? category = null)
        {
            if (category.HasValue)
                return _allMods.Count(m => m.Category == category.Value);

            return _allMods.Count;
        }

        private void SaveEnabledState()
        {
            try
            {
                var enabledIds = _enabledMods.Select(m => m.Id).ToList();
                string json = JsonSerializer.Serialize(enabledIds, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_stateFilePath, json);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("BetterModManager::SaveEnabledState", ex);
            }
        }

        public void LoadEnabledState()
        {
            const string LOG_IDENT = "BetterModManager::LoadEnabledState";

            if (!File.Exists(_stateFilePath))
                return;

            try
            {
                string json = File.ReadAllText(_stateFilePath);
                var enabledIds = JsonSerializer.Deserialize<List<string>>(json);

                if (enabledIds is null)
                    return;

                foreach (string modId in enabledIds)
                {
                    var mod = _allMods.FirstOrDefault(m => m.Id == modId);
                    if (mod is not null)
                    {
                        mod.IsEnabled = true;
                        _enabledMods.Add(mod);
                    }
                }

                App.Logger.WriteLine(LOG_IDENT, $"Loaded {enabledIds.Count} enabled mods");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public void ResetAllMods()
        {
            const string LOG_IDENT = "BetterModManager::ResetAllMods";

            try
            {
                if (Directory.Exists(Paths.Modifications))
                    Directory.Delete(Paths.Modifications, true);

                foreach (var mod in _allMods)
                {
                    mod.IsEnabled = false;
                    mod.CustomFilePath = "";
                }

                _enabledMods.Clear();
                SaveEnabledState();

                App.Logger.WriteLine(LOG_IDENT, "All mods reset to defaults, Modifications folder deleted");
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                throw;
            }
        }

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }
    }
}
