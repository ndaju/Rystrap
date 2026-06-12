<p align="center">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Ndaju-full-dark.png#gh-dark-mode-only" width="420">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Ndaju-full-light.png#gh-light-mode-only" width="420">
</p>

<p align="center">
  <a href="https://github.com/ndaju/Rystrap/releases"><img src="https://img.shields.io/github/v/release/ndaju/Rystrap?style=flat&logo=github&labelColor=1e1e2e&color=cba6f7" alt="Release"></a>
  <a href="https://github.com/ndaju/Rystrap/releases"><img src="https://img.shields.io/github/downloads/ndaju/Rystrap/latest/total?style=flat&logo=azurepipelines&labelColor=1e1e2e&color=89b4fa" alt="Downloads"></a>
  <a href="LICENSE"><img src="https://img.shields.io/github/license/ndaju/Rystrap?style=flat&labelColor=1e1e2e&color=a6e3a1" alt="License"></a>
  <a href="https://dotnet.microsoft.com/download/dotnet/6.0"><img src="https://img.shields.io/badge/.NET_6-512BD4?style=flat&logo=dotnet&labelColor=1e1e2e&color=89dceb" alt=".NET 6"></a>
</p>

Rystrap is a third-party replacement for the standard Roblox bootstrapper. It handles launching Roblox with extra features on top -- Discord rich presence, content modding, FastFlag configuration, server location display, and more. Works on Windows 10+.

## Why Rystrap over Bloxstrap?

Bloxstrap is great but development slowed down. Rystrap keeps things moving with **weekly updates**, new features, and fixes the community actually asks for.

| Feature | Bloxstrap | Rystrap |
|---|---|---|
| Plugin system | ❌ | ✅ Load .dll plugins with event bus, settings API |
| Theme engine | ❌ | ✅ 6 themes + custom import/export via zip |
| Mod manager | Basic | ✅ BetterModManager with presets, conflict detection |
| FastFlag presets | Basic | ✅ MSAA, DPI, texture quality all in one place |
| Accounts manager | ❌ | ✅ Multiple accounts with .ROBLOSECURITY, quick switch |
| Discord RPC | Basic | ✅ Game name, server type, creator badges, join button |
| Better mod system | ❌ | ✅ Mod packs, presets, 190+ sounds |
| Weekly updates | ❌ | ✅ Active development, community-driven features |

## All Features

**Discord Rich Presence** -- shows game name, server type (public/private/reserved), creator badge, play time, join button, and your Roblox avatar as a small image. Games can set custom RPC via RystrapRPC protocol.

**Plugin System** -- load plugins from DLLs with isolated assembly contexts. Full event bus for inter-plugin communication, settings API, logging API, paths API. State persists between sessions.

**Theme Engine** -- 6 built-in themes (Dark, Light, Neon, Retro, Midnight, Sunset). Import/export custom themes as ZIP files. Preview before applying. Each theme sets colors for every UI element.

**Content Modding** -- replace sounds, textures, cursors, fonts, animations, UI elements. 190+ built-in sounds across 8 categories. Preset packs: Old 2006/2013 cursors, Classic/Retro/Modern sounds, Clean/Minimalist/Classic/HD textures, multiple font packs.

**Multi-Account Manager** -- save multiple Roblox accounts with .ROBLOSECURITY cookies. Quick switch between accounts. Cookies never leave your device. Favorites, avatars, volume, notes per account.

**FastFlag Editor** -- toggle MSAA (1x/2x/4x), fix DPI scaling, override texture quality. Edit raw FFlags via JSON editor. Presets for exclusive fullscreen, rendering quality, and more.

**Server Location** -- see where your game server is geographically located, powered by ipinfo.io.

**Auto-Updater** -- automatic background update checking on startup. Delta updates via package manifests. Seamless upgrade without interrupting gameplay.

**Custom Integrations** -- launch external programs alongside Roblox. Auto-close support. Activity tracking for game detection.

**Bootstrapper Customization** -- custom bootstrapper styles/themes. Progress colors. Custom title and icon. Byfron dialog support.

**30+ Languages** -- fully localized via Crowdin. Contribute your own translations.

**Analytics** -- opt-in telemetry. Fully transparent, no data leaks.

## Installing

[Download the latest release](https://github.com/ndaju/Rystrap/releases/latest) and run it. Pick your settings during install and you're done. It'll add itself to your Start Menu.

You need the [.NET 6 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win11-x64&apphost_version=6.0.36&gui=true) if you don't already have it.

## Building

```
git clone --recurse-submodules https://github.com/ndaju/Rystrap.git
cd Rystrap
dotnet build Rystrap/Rystrap.csproj -c Release
```

Built with [WPF UI](https://github.com/lepoco/wpfui) (flork included at wpfui/).

## License

MIT &copy; 2026 [ndaju](https://github.com/ndaju)

Based on [Bloxstrap](https://github.com/pizzaboxer/bloxstrap).
