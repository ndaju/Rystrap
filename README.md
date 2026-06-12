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

## Features

- **Better Discord Rich Presence** -- shows your friends what game you're in
- **Content modding** -- swap out sounds, textures, cursors, fonts
- **Plugin system** -- load .dll plugins with a full event bus
- **Themes** -- 6 built-in, import/export your own via zip
- **Server location** -- see where the game server is (powered by ipinfo.io)
- **FastFlag editor** -- configure graphics, MSAA, texture quality, DPI
- **30+ Futures** -- more futures then bloxstrap thing ah
- **Auto-updater** -- checks for new releases on startup

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
