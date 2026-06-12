<p align="center">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Rystrap-full-dark.png#gh-dark-mode-only" width="420">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Rystrap-full-light.png#gh-light-mode-only" width="420">
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#installing">Installing</a> •
  <a href="#building-from-source">Build</a> •
  <a href="#license">License</a>
</p>

<p align="center">
  <a href="https://github.com/ndaju/Rystrap/releases"><img src="https://img.shields.io/github/v/release/ndaju/Rystrap?style=flat&logo=github&logoColor=white&labelColor=1e1e2e&color=cba6f7" alt="Release"></a>
  <a href="https://github.com/ndaju/Rystrap/releases"><img src="https://img.shields.io/github/downloads/ndaju/Rystrap/latest/total?style=flat&logo=azurepipelines&logoColor=white&labelColor=1e1e2e&color=89b4fa" alt="Downloads"></a>
  <a href="https://github.com/ndaju/Rystrap/blob/main/LICENSE"><img src="https://img.shields.io/github/license/ndaju/Rystrap?style=flat&logo=opensourceinitiative&logoColor=white&labelColor=1e1e2e&color=a6e3a1" alt="License"></a>
  <a href="https://github.com/ndaju/Rystrap/actions"><img src="https://img.shields.io/github/actions/workflow/status/ndaju/Rystrap/ci-release.yml?style=flat&logo=githubactions&logoColor=white&labelColor=1e1e2e&color=f9e2af" alt="Build"></a>
  <a href="https://dotnet.microsoft.com/download/dotnet/6.0"><img src="https://img.shields.io/badge/.NET-6.0-512BD4?style=flat&logo=dotnet&logoColor=white&labelColor=1e1e2e&color=89dceb" alt=".NET 6"></a>
</p>

<br>

**Rystrap** is a third-party replacement for the standard Roblox bootstrapper — a faster, smarter, and more customizable launcher for Windows.

It's open source, privacy-respecting, and built with ❤️ by the community for the community.

<br>

---

<br>

<h2 align="center">✨ Features</h2>

<br>

<table align="center">
  <tr>
    <td width="50%" align="center">
      <h3>🎮 Discord Rich Presence</h3>
      <p>Show your friends what game you're playing — server type, play time, join button, and more.</p>
    </td>
    <td width="50%" align="center">
      <h3>🎨 Content Modding</h3>
      <p>Replace sounds, textures, cursors, fonts, animations, and UI elements. Preset packs or your own files.</p>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <h3>🧩 Plugin System</h3>
      <p>Extend Rystrap with plugins loaded from DLLs. Full event bus, settings API, and logging support.</p>
    </td>
    <td width="50%" align="center">
      <h3>🎭 Theme Engine</h3>
      <p>6 built-in themes (Dark, Light, Neon, Retro, Midnight, Sunset) + custom theme import via ZIP.</p>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <h3>📡 Server Location</h3>
      <p>See where your game server is located geographically, powered by ipinfo.io.</p>
    </td>
    <td width="50%" align="center">
      <h3>⚡ Graphics Control</h3>
      <p>Configure MSAA, texture quality, DPI scaling, and Fast Flags for optimal performance.</p>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <h3>🌍 30+ Languages</h3>
      <p>Fully localized via Crowdin. Contributions welcome to add or improve translations.</p>
    </td>
    <td width="50%" align="center">
      <h3>🔄 Auto-Updater</h3>
      <p>Seamless background updates with delta patching via GitHub Releases.</p>
    </td>
  </tr>
</table>

<br>

---

<br>

<h2 align="center">📥 Installing</h2>

<br>

<p align="center">
  <a href="https://github.com/ndaju/Rystrap/releases/latest">
    <img src="https://img.shields.io/badge/Download_Latest_Release-1e1e2e?style=for-the-badge&logo=github&logoColor=white" alt="Download">
  </a>
</p>

1. Download the [latest release](https://github.com/ndaju/Rystrap/releases/latest) and run it.
2. Configure your preferences if needed and install. That's it!

> **Note:** You'll need the [.NET 6 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win11-x64&apphost_version=6.0.36&gui=true). If you don't have it, Rystrap will prompt you to install it first.

Once installed, Rystrap appears in your Start Menu where you can access the menu and reconfigure your preferences.

<br>

---

<br>

<h2 align="center">🔧 Building from Source</h2>

<br>

<table align="center">
  <tr>
    <th>Requirement</th>
    <th>Version</th>
  </tr>
  <tr><td>.NET SDK</td><td>6.0</td></tr>
  <tr><td>Visual Studio</td><td>2022 (recommended)</td></tr>
  <tr><td>OS</td><td>Windows 10+</td></tr>
</table>

```bash
# Clone with submodules
git clone --recurse-submodules https://github.com/ndaju/Rystrap.git
cd Rystrap

# Build
dotnet build Rystrap/Rystrap.csproj -c Release

# Publish single-file (win-x64)
dotnet publish Rystrap/Rystrap.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

Rystrap uses the [WPF UI](https://github.com/lepoco/wpfui) library — a fork is included at `wpfui/`.

<br>

---

<br>

<h2 align="center">📄 License</h2>

<p align="center">
  MIT &copy; 2026 <a href="https://github.com/ndaju">ndaju</a>
</p>

<p align="center">
  <sub>Built on the shoulders of the <a href="https://github.com/pizzaboxer/bloxstrap">Bloxstrap</a> project.</sub>
</p>

<br>
