<p align="center">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Ndaju-full-dark.png#gh-dark-mode-only" width="420">
  <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Ndaju-full-light.png#gh-light-mode-only" width="420">
</p>

## Rystrap v2.11.5 Release Notes

### New Features
- **Launch-time account selection** — choose which Roblox account to use when launching the game
- **Mod preset tasks** — one-click mods for old avatar background, old character sounds, emoji font, cursor styles, and custom fonts
- **Sound mods** — replace Roblox jump, death, and walk sounds with custom audio files
- **Bootstrapper sound customization** — set custom start, error, and completion sounds for the bootstrapper
- **Bootstrapper progress bar color picker** — customize the progress bar color (defaults to green `#4CAF50`); applies to all dialog styles
- **Bootstrapper background image** — set a background image on the bootstrapper window (Fluent, ClassicFluent, RystrapTransparent)
- **Reset to Default buttons** — one-click reset for Mods tab and Bootstrapper tab

### Improvements
- Discord Rich Presence settings moved from Integrations tab to Discord Settings tab
- Progress bar color now automatically applies to FluentDialog and ClassicFluentDialog
- Removed Better Mods tab
- Removed analytics tracking and WebEnvironment enum
- Updated About page contributor labels

### Bug Fixes
- Fixed `TargetInvocationException` crash from invalid `GitHub20` icon (replaced with `LinkSquare20`)
- Fixed build errors by excluding nested `wpfui` and `Rystrap` source trees
- Progress bar color setting now correctly applies across all bootstrapper dialog styles
- Bootstrapper progress bar defaults to green automatically
