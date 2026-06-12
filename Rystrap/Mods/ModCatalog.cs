using Rystrap.Models.Mods;

namespace Rystrap.Mods
{
    public static class ModCatalog
    {
        public struct SoundEntry
        {
            public string FriendlyName;
            public string TargetPath;
            public Models.Mods.ModCategory Category;
            public string Description;

            public SoundEntry(string friendlyName, string targetPath, Models.Mods.ModCategory category, string description = "")
            {
                FriendlyName = friendlyName;
                TargetPath = targetPath;
                Category = category;
                Description = description;
            }
        }

        public static readonly List<SoundEntry> KnownSounds = new()
        {
            // Character Movement
            new SoundEntry("Jump", @"content\sounds\action_jump.mp3", ModCategory.Sound, "Played when the character jumps"),
            new SoundEntry("Fall", @"content\sounds\action_falling.mp3", ModCategory.Sound, "Played when the character is falling"),
            new SoundEntry("Death", @"content\sounds\action_death.mp3", ModCategory.Sound, "Played when the character dies"),
            new SoundEntry("Get Up", @"content\sounds\action_get_up.mp3", ModCategory.Sound, "Played when the character gets up"),
            new SoundEntry("Walk", @"content\sounds\action_footsteps_plastic.mp3", ModCategory.Sound, "Played when walking on plastic surfaces"),
            new SoundEntry("Jump Land", @"content\sounds\action_jump_land.mp3", ModCategory.Sound, "Played when landing after a jump"),
            new SoundEntry("Swim", @"content\sounds\action_swim.mp3", ModCategory.Sound, "Played when swimming"),
            new SoundEntry("Waterstep", @"content\sounds\impact_water.mp3", ModCategory.Sound, "Played when stepping in water"),
            new SoundEntry("Splash", @"content\sounds\impact_splash.mp3", ModCategory.Sound, "Played when splashing in water"),
            new SoundEntry("Run", @"content\sounds\action_run.mp3", ModCategory.Sound, "Played when running"),
            new SoundEntry("Climb", @"content\sounds\action_climb.mp3", ModCategory.Sound, "Played when climbing"),
            new SoundEntry("Sit", @"content\sounds\action_sit.mp3", ModCategory.Sound, "Played when sitting down"),
            new SoundEntry("Stand Up", @"content\sounds\action_stand.mp3", ModCategory.Sound, "Played when standing up"),
            new SoundEntry("Tap", @"content\sounds\action_tap.mp3", ModCategory.Sound, "Played when tapping surfaces"),

            // Equipment
            new SoundEntry("Equip", @"content\sounds\action_equip.mp3", ModCategory.Sound, "Played when equipping an item"),
            new SoundEntry("Unequip", @"content\sounds\action_unequip.mp3", ModCategory.Sound, "Played when unequipping an item"),
            new SoundEntry("Tool Equip", @"content\sounds\action_tool_equip.mp3", ModCategory.Sound, "Played when equipping a tool"),
            new SoundEntry("Tool Unequip", @"content\sounds\action_tool_unequip.mp3", ModCategory.Sound, "Played when unequipping a tool"),
            new SoundEntry("Pickup", @"content\sounds\action_pickup.mp3", ModCategory.Sound, "Played when picking up an item"),
            new SoundEntry("Drop", @"content\sounds\action_drop.mp3", ModCategory.Sound, "Played when dropping an item"),
            new SoundEntry("Sheathe", @"content\sounds\action_sheathe.mp3", ModCategory.Sound, "Played when sheathing a weapon"),
            new SoundEntry("Unsheathe", @"content\sounds\action_unsheathe.mp3", ModCategory.Sound, "Played when unsheathing a weapon"),

            // UI Interactions
            new SoundEntry("Click", @"content\sounds\ui_click.mp3", ModCategory.Sound, "General UI click sound"),
            new SoundEntry("Hover", @"content\sounds\ui_hover.mp3", ModCategory.Sound, "UI hover over element"),
            new SoundEntry("Button Press", @"content\sounds\ui_button_press.mp3", ModCategory.Sound, "Button press feedback"),
            new SoundEntry("Button Release", @"content\sounds\ui_button_release.mp3", ModCategory.Sound, "Button release feedback"),
            new SoundEntry("Menu Open", @"content\sounds\ui_menu_open.mp3", ModCategory.Sound, "Played when opening a menu"),
            new SoundEntry("Menu Close", @"content\sounds\ui_menu_close.mp3", ModCategory.Sound, "Played when closing a menu"),
            new SoundEntry("Menu Back", @"content\sounds\ui_menu_back.mp3", ModCategory.Sound, "Played when navigating back in menus"),
            new SoundEntry("Toggle On", @"content\sounds\ui_toggle_on.mp3", ModCategory.Sound, "Played when toggling a switch on"),
            new SoundEntry("Toggle Off", @"content\sounds\ui_toggle_off.mp3", ModCategory.Sound, "Played when toggling a switch off"),
            new SoundEntry("Tab Select", @"content\sounds\ui_tab_select.mp3", ModCategory.Sound, "Played when selecting a tab"),
            new SoundEntry("Notification", @"content\sounds\ui_notification.mp3", ModCategory.Sound, "Notification sound"),
            new SoundEntry("Error", @"content\sounds\ui_error.mp3", ModCategory.Sound, "Error alert sound"),
            new SoundEntry("Success", @"content\sounds\ui_success.mp3", ModCategory.Sound, "Success confirmation sound"),
            new SoundEntry("Warning", @"content\sounds\ui_warning.mp3", ModCategory.Sound, "Warning alert sound"),
            new SoundEntry("Window Open", @"content\sounds\ui_window_open.mp3", ModCategory.Sound, "Played when a window opens"),
            new SoundEntry("Window Close", @"content\sounds\ui_window_close.mp3", ModCategory.Sound, "Played when a window closes"),
            new SoundEntry("Scroll", @"content\sounds\ui_scroll.mp3", ModCategory.Sound, "Played when scrolling"),

            // Chat
            new SoundEntry("Chat Message", @"content\sounds\chat_message.mp3", ModCategory.Sound, "Played when sending a chat message"),
            new SoundEntry("Chat Receive", @"content\sounds\chat_receive.mp3", ModCategory.Sound, "Played when receiving a chat message"),
            new SoundEntry("Chat Open", @"content\sounds\chat_open.mp3", ModCategory.Sound, "Played when opening chat"),
            new SoundEntry("Chat Close", @"content\sounds\chat_close.mp3", ModCategory.Sound, "Played when closing chat"),
            new SoundEntry("Typing", @"content\sounds\chat_typing.mp3", ModCategory.Sound, "Played when typing in chat"),
            new SoundEntry("Message Sent", @"content\sounds\chat_sent.mp3", ModCategory.Sound, "Played when message is sent"),

            // Impacts and Physics
            new SoundEntry("Impact Light", @"content\sounds\impact_light.mp3", ModCategory.Sound, "Light impact sound"),
            new SoundEntry("Impact Medium", @"content\sounds\impact_medium.mp3", ModCategory.Sound, "Medium impact sound"),
            new SoundEntry("Impact Heavy", @"content\sounds\impact_heavy.mp3", ModCategory.Sound, "Heavy impact sound"),
            new SoundEntry("Brick", @"content\sounds\impact_brick.mp3", ModCategory.Sound, "Brick impact sound"),
            new SoundEntry("Explosion", @"content\sounds\impact_explode.mp3", ModCategory.Sound, "Explosion impact sound"),
            new SoundEntry("Glass Break", @"content\sounds\impact_glass.mp3", ModCategory.Sound, "Glass breaking sound"),
            new SoundEntry("Metal Hit", @"content\sounds\impact_metal.mp3", ModCategory.Sound, "Metal impact sound"),
            new SoundEntry("Wood Hit", @"content\sounds\impact_wood.mp3", ModCategory.Sound, "Wood impact sound"),
            new SoundEntry("Concrete Hit", @"content\sounds\impact_concrete.mp3", ModCategory.Sound, "Concrete impact sound"),
            new SoundEntry("Dirt Hit", @"content\sounds\impact_dirt.mp3", ModCategory.Sound, "Dirt impact sound"),
            new SoundEntry("Grass Hit", @"content\sounds\impact_grass.mp3", ModCategory.Sound, "Grass impact sound"),
            new SoundEntry("Sand Hit", @"content\sounds\impact_sand.mp3", ModCategory.Sound, "Sand impact sound"),
            new SoundEntry("Snow Hit", @"content\sounds\impact_snow.mp3", ModCategory.Sound, "Snow impact sound"),
            new SoundEntry("Rubble Hit", @"content\sounds\impact_rubble.mp3", ModCategory.Sound, "Rubble impact sound"),

            // Musical and Effects
            new SoundEntry("Bell", @"content\sounds\effect_bell.mp3", ModCategory.Sound, "Bell sound effect"),
            new SoundEntry("Whistle", @"content\sounds\effect_whistle.mp3", ModCategory.Sound, "Whistle sound effect"),
            new SoundEntry("Horn", @"content\sounds\effect_horn.mp3", ModCategory.Sound, "Horn sound effect"),
            new SoundEntry("Chime", @"content\sounds\effect_chime.mp3", ModCategory.Sound, "Chime sound effect"),
            new SoundEntry("Drum Roll", @"content\sounds\effect_drum_roll.mp3", ModCategory.Sound, "Drum roll sound effect"),
            new SoundEntry("Fanfare", @"content\sounds\effect_fanfare.mp3", ModCategory.Sound, "Fanfare sound effect"),
            new SoundEntry("Power Up", @"content\sounds\effect_powerup.mp3", ModCategory.Sound, "Power up sound effect"),
            new SoundEntry("Power Down", @"content\sounds\effect_powerdown.mp3", ModCategory.Sound, "Power down sound effect"),
            new SoundEntry("Warp", @"content\sounds\effect_warp.mp3", ModCategory.Sound, "Warp/teleport sound effect"),
            new SoundEntry("Whoosh", @"content\sounds\effect_whoosh.mp3", ModCategory.Sound, "Whoosh/transition sound effect"),
            new SoundEntry("Sparkle", @"content\sounds\effect_sparkle.mp3", ModCategory.Sound, "Sparkle sound effect"),
            new SoundEntry("Pop", @"content\sounds\effect_pop.mp3", ModCategory.Sound, "Pop sound effect"),
            new SoundEntry("Click High", @"content\sounds\effect_click_high.mp3", ModCategory.Sound, "High pitched click"),
            new SoundEntry("Click Low", @"content\sounds\effect_click_low.mp3", ModCategory.Sound, "Low pitched click"),
            new SoundEntry("Coin", @"content\sounds\effect_coin.mp3", ModCategory.Sound, "Coin collect sound"),
            new SoundEntry("Level Up", @"content\sounds\effect_levelup.mp3", ModCategory.Sound, "Level up sound"),
            new SoundEntry("Achievement", @"content\sounds\effect_achievement.mp3", ModCategory.Sound, "Achievement unlocked sound"),
            new SoundEntry("Health Low", @"content\sounds\effect_health_low.mp3", ModCategory.Sound, "Low health warning sound"),
            new SoundEntry("Damage", @"content\sounds\effect_damage.mp3", ModCategory.Sound, "Taking damage sound"),
            new SoundEntry("Heal", @"content\sounds\effect_heal.mp3", ModCategory.Sound, "Healing sound"),
            new SoundEntry("Shield", @"content\sounds\effect_shield.mp3", ModCategory.Sound, "Shield activation sound"),

            // Music Loops
            new SoundEntry("Music Loop - Ambient", @"content\sounds\music_loop_ambient.mp3", ModCategory.Sound, "Ambient background music loop"),
            new SoundEntry("Music Loop - Battle", @"content\sounds\music_loop_battle.mp3", ModCategory.Sound, "Battle background music loop"),
            new SoundEntry("Music Loop - Calm", @"content\sounds\music_loop_calm.mp3", ModCategory.Sound, "Calm background music loop"),
            new SoundEntry("Music Loop - Happy", @"content\sounds\music_loop_happy.mp3", ModCategory.Sound, "Happy background music loop"),
            new SoundEntry("Music Loop - Sad", @"content\sounds\music_loop_sad.mp3", ModCategory.Sound, "Sad background music loop"),
            new SoundEntry("Music Loop - Epic", @"content\sounds\music_loop_epic.mp3", ModCategory.Sound, "Epic background music loop"),
            new SoundEntry("Music Loop - Mystery", @"content\sounds\music_loop_mystery.mp3", ModCategory.Sound, "Mystery background music loop"),
            new SoundEntry("Music Loop - Horror", @"content\sounds\music_loop_horror.mp3", ModCategory.Sound, "Horror background music loop"),
            new SoundEntry("Music Loop - Sci-Fi", @"content\sounds\music_loop_scifi.mp3", ModCategory.Sound, "Sci-Fi background music loop"),
            new SoundEntry("Music Loop - Fantasy", @"content\sounds\music_loop_fantasy.mp3", ModCategory.Sound, "Fantasy background music loop"),
            new SoundEntry("Music Loop - Retro", @"content\sounds\music_loop_retro.mp3", ModCategory.Sound, "Retro background music loop"),
            new SoundEntry("Music Loop - Space", @"content\sounds\music_loop_space.mp3", ModCategory.Sound, "Space background music loop"),

            // Footstep Variants
            new SoundEntry("Footstep - Concrete", @"content\sounds\footstep_concrete.mp3", ModCategory.Sound, "Footstep on concrete surface"),
            new SoundEntry("Footstep - Wood", @"content\sounds\footstep_wood.mp3", ModCategory.Sound, "Footstep on wood surface"),
            new SoundEntry("Footstep - Metal", @"content\sounds\footstep_metal.mp3", ModCategory.Sound, "Footstep on metal surface"),
            new SoundEntry("Footstep - Grass", @"content\sounds\footstep_grass.mp3", ModCategory.Sound, "Footstep on grass surface"),
            new SoundEntry("Footstep - Gravel", @"content\sounds\footstep_gravel.mp3", ModCategory.Sound, "Footstep on gravel surface"),
            new SoundEntry("Footstep - Sand", @"content\sounds\footstep_sand.mp3", ModCategory.Sound, "Footstep on sand surface"),
            new SoundEntry("Footstep - Snow", @"content\sounds\footstep_snow.mp3", ModCategory.Sound, "Footstep on snow surface"),
            new SoundEntry("Footstep - Water", @"content\sounds\footstep_water.mp3", ModCategory.Sound, "Footstep in water"),
            new SoundEntry("Footstep - Mud", @"content\sounds\footstep_mud.mp3", ModCategory.Sound, "Footstep in mud"),
            new SoundEntry("Footstep - Tile", @"content\sounds\footstep_tile.mp3", ModCategory.Sound, "Footstep on tile surface"),
            new SoundEntry("Footstep - Carpet", @"content\sounds\footstep_carpet.mp3", ModCategory.Sound, "Footstep on carpet surface"),
            new SoundEntry("Footstep - Dirt", @"content\sounds\footstep_dirt.mp3", ModCategory.Sound, "Footstep on dirt surface"),

            // Ambient / Environment
            new SoundEntry("Wind", @"content\sounds\ambient_wind.mp3", ModCategory.Sound, "Wind ambient sound"),
            new SoundEntry("Rain", @"content\sounds\ambient_rain.mp3", ModCategory.Sound, "Rain ambient sound"),
            new SoundEntry("Thunder", @"content\sounds\ambient_thunder.mp3", ModCategory.Sound, "Thunder sound"),
            new SoundEntry("Birds", @"content\sounds\ambient_birds.mp3", ModCategory.Sound, "Birds chirping ambient"),
            new SoundEntry("Crickets", @"content\sounds\ambient_crickets.mp3", ModCategory.Sound, "Crickets ambient"),
            new SoundEntry("Fire", @"content\sounds\ambient_fire.mp3", ModCategory.Sound, "Fire crackling ambient"),
            new SoundEntry("Ocean Waves", @"content\sounds\ambient_ocean.mp3", ModCategory.Sound, "Ocean waves ambient"),
            new SoundEntry("River", @"content\sounds\ambient_river.mp3", ModCategory.Sound, "River flowing ambient"),
            new SoundEntry("Forest", @"content\sounds\ambient_forest.mp3", ModCategory.Sound, "Forest ambient"),
            new SoundEntry("City", @"content\sounds\ambient_city.mp3", ModCategory.Sound, "City ambient sounds"),
            new SoundEntry("Cave", @"content\sounds\ambient_cave.mp3", ModCategory.Sound, "Cave ambient sounds"),
            new SoundEntry("Underwater", @"content\sounds\ambient_underwater.mp3", ModCategory.Sound, "Underwater ambient sounds"),

            // Vehicle Sounds
            new SoundEntry("Engine Start", @"content\sounds\vehicle_engine_start.mp3", ModCategory.Sound, "Vehicle engine starting"),
            new SoundEntry("Engine Idle", @"content\sounds\vehicle_engine_idle.mp3", ModCategory.Sound, "Vehicle engine idle"),
            new SoundEntry("Engine Accelerate", @"content\sounds\vehicle_engine_accel.mp3", ModCategory.Sound, "Vehicle engine acceleration"),
            new SoundEntry("Engine Brake", @"content\sounds\vehicle_engine_brake.mp3", ModCategory.Sound, "Vehicle braking sound"),
            new SoundEntry("Horn Vehicle", @"content\sounds\vehicle_horn.mp3", ModCategory.Sound, "Vehicle horn"),
            new SoundEntry("Crash", @"content\sounds\vehicle_crash.mp3", ModCategory.Sound, "Vehicle crash sound"),
            new SoundEntry("Skid", @"content\sounds\vehicle_skid.mp3", ModCategory.Sound, "Vehicle skidding sound"),
            new SoundEntry("Helicopter", @"content\sounds\vehicle_helicopter.mp3", ModCategory.Sound, "Helicopter rotor sound"),
            new SoundEntry("Jet Engine", @"content\sounds\vehicle_jet.mp3", ModCategory.Sound, "Jet engine sound"),

            // Weapon Sounds
            new SoundEntry("Sword Swing", @"content\sounds\weapon_sword_swing.mp3", ModCategory.Sound, "Sword swinging sound"),
            new SoundEntry("Sword Hit", @"content\sounds\weapon_sword_hit.mp3", ModCategory.Sound, "Sword hitting target"),
            new SoundEntry("Gun Shot", @"content\sounds\weapon_gun_shot.mp3", ModCategory.Sound, "Gunshot sound"),
            new SoundEntry("Gun Reload", @"content\sounds\weapon_gun_reload.mp3", ModCategory.Sound, "Gun reloading sound"),
            new SoundEntry("Arrow Fire", @"content\sounds\weapon_arrow_fire.mp3", ModCategory.Sound, "Arrow being fired"),
            new SoundEntry("Arrow Hit", @"content\sounds\weapon_arrow_hit.mp3", ModCategory.Sound, "Arrow hitting target"),
            new SoundEntry("Magic Cast", @"content\sounds\weapon_magic_cast.mp3", ModCategory.Sound, "Magic spell cast"),
            new SoundEntry("Magic Impact", @"content\sounds\weapon_magic_impact.mp3", ModCategory.Sound, "Magic spell impact"),
            new SoundEntry("Punch", @"content\sounds\weapon_punch.mp3", ModCategory.Sound, "Punch sound"),
            new SoundEntry("Kick", @"content\sounds\weapon_kick.mp3", ModCategory.Sound, "Kick sound"),
            new SoundEntry("Block", @"content\sounds\weapon_block.mp3", ModCategory.Sound, "Blocking an attack"),
            new SoundEntry("Parry", @"content\sounds\weapon_parry.mp3", ModCategory.Sound, "Parrying an attack"),

            // Building / Crafting
            new SoundEntry("Build Place", @"content\sounds\build_place.mp3", ModCategory.Sound, "Placing a building piece"),
            new SoundEntry("Build Remove", @"content\sounds\build_remove.mp3", ModCategory.Sound, "Removing a building piece"),
            new SoundEntry("Craft", @"content\sounds\craft.mp3", ModCategory.Sound, "Crafting an item"),
            new SoundEntry("Hammer", @"content\sounds\craft_hammer.mp3", ModCategory.Sound, "Hammer crafting sound"),
            new SoundEntry("Saw", @"content\sounds\craft_saw.mp3", ModCategory.Sound, "Saw cutting sound"),
            new SoundEntry("Drill", @"content\sounds\craft_drill.mp3", ModCategory.Sound, "Drilling sound"),
            new SoundEntry("Welding", @"content\sounds\craft_welding.mp3", ModCategory.Sound, "Welding sound"),
        };

        public static readonly List<string> KnownCursorPaths = new()
        {
            @"content\textures\Cursors\KeyboardMouse\ArrowCursor.png",
            @"content\textures\Cursors\KeyboardMouse\ArrowFarCursor.png",
            @"content\textures\Cursors\KeyboardMouse\IBeamCursor.png",
            @"content\textures\Cursors\KeyboardMouse\SizeAllCursor.png",
            @"content\textures\Cursors\KeyboardMouse\SizeNESWCursor.png",
            @"content\textures\Cursors\KeyboardMouse\SizeNSCursor.png",
            @"content\textures\Cursors\KeyboardMouse\SizeNWSECursor.png",
            @"content\textures\Cursors\KeyboardMouse\SizeWECursor.png",
            @"content\textures\Cursors\KeyboardMouse\SplitterCursor.png",
            @"content\textures\Cursors\KeyboardMouse\PointingHand.png",
            @"content\textures\Cursors\KeyboardMouse\ForbiddenCursor.png",
            @"content\textures\Cursors\KeyboardMouse\WaitCursor.png",
            @"content\textures\Cursors\KeyboardMouse\BusyCursor.png",
            @"content\textures\Cursors\KeyboardMouse\CrossCursor.png",
        };

        public static readonly List<string> KnownTexturePaths = new()
        {
            @"content\textures\sky\sky.tex",
            @"content\textures\terrain\terrain_normal.tex",
            @"content\textures\terrain\terrain_diffuse.tex",
            @"content\textures\ui\background.tex",
            @"content\textures\ui\button_normal.tex",
            @"content\textures\ui\button_hover.tex",
            @"content\textures\ui\button_pressed.tex",
            @"content\textures\ui\scrollbar_track.tex",
            @"content\textures\ui\scrollbar_thumb.tex",
            @"content\textures\ui\panel_background.tex",
            @"content\textures\ui\textbox_background.tex",
            @"content\textures\ui\checkbox_unchecked.tex",
            @"content\textures\ui\checkbox_checked.tex",
            @"content\textures\ui\radio_unchecked.tex",
            @"content\textures\ui\radio_checked.tex",
        };

        public static readonly List<string> KnownFontPaths = new()
        {
            @"content\fonts\SourceSansPro-Regular.ttf",
            @"content\fonts\SourceSansPro-SemiBold.ttf",
            @"content\fonts\SourceSansPro-Bold.ttf",
            @"content\fonts\NotoSans-Regular.ttf",
            @"content\fonts\NotoSans-SemiBold.ttf",
            @"content\fonts\NotoSans-Bold.ttf",
        };

        public static readonly List<string> KnownAnimationPaths = new()
        {
            @"content\animations\idle.rbxanim",
            @"content\animations\walk.rbxanim",
            @"content\animations\run.rbxanim",
            @"content\animations\jump.rbxanim",
            @"content\animations\fall.rbxanim",
            @"content\animations\climb.rbxanim",
            @"content\animations\swim.rbxanim",
        };

        public static readonly List<string> KnownUIPaths = new()
        {
            @"content\textures\ui\ChatKeyboardIcon.png",
            @"content\textures\ui\EscapeMenuIcon.png",
            @"content\textures\ui\InventoryIcon.png",
            @"content\textures\ui\SettingsIcon.png",
        };

        public static SoundEntry? FindSoundByName(string name)
        {
            return KnownSounds.FirstOrDefault(s =>
                string.Equals(s.FriendlyName, name, StringComparison.OrdinalIgnoreCase));
        }

        public static SoundEntry? FindSoundByTargetPath(string targetPath)
        {
            return KnownSounds.FirstOrDefault(s =>
                string.Equals(s.TargetPath, targetPath, StringComparison.OrdinalIgnoreCase));
        }

        public static List<SoundEntry> GetSoundsByCategory(string category)
        {
            return KnownSounds.Where(s =>
                s.FriendlyName.Contains(category, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
