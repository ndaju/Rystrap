<p align="center">
    <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Rystrap-full-dark.png#gh-dark-mode-only" width="380">
    <img src="https://github.com/ndaju/Rystrap/raw/main/Images/Rystrap-full-light.png#gh-light-mode-only" width="380">
</p>

<div align="center">

[![License][shield-repo-license]][repo-license]
[![GitHub Workflow Status][shield-repo-workflow]][repo-actions]
[![Crowdin][shield-crowdin-status]][crowdin-project]
[![Downloads][shield-repo-releases]][repo-releases]
[![Version][shield-repo-latest]][repo-latest]

</div>

----

Rystrap is a third-party replacement for the standard Roblox bootstrapper, providing additional useful features and improvements.

Rystrap is only supported for PCs running Windows.

## Frequently Asked Questions

**Q: Is this malware?**

**A:** No. The source code here is viewable to all, and it'd be impossible for us to slip anything malicious into the downloads without anyone noticing.

**Q: Can using this get me banned?**

**A:** No, it shouldn't. Rystrap doesn't interact with the Roblox client in the same way that exploits do.

## Features

- Hassle-free Discord Rich Presence to let your friends know what you're playing at a glance
- Simple support for modding of content files for customizability (death sound, mouse cursor, etc)
- See where your server is geographically located (courtesy of [ipinfo.io](https://ipinfo.io))
- Ability to configure graphics fidelity and UI experience

## Installing
Download the [latest release of Rystrap](https://github.com/ndaju/Rystrap/releases/latest), and run it. Configure your preferences if needed, and install. That's about it!

You will also need the [.NET 6 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win11-x64&apphost_version=6.0.36&gui=true). If you don't already have it installed, you'll be prompted to install it anyway. Be sure to install Rystrap after you've installed this.

It's not unlikely that Windows Smartscreen will show a popup when you run Rystrap for the first time. This happens because it's an unknown program, not because it's actually detected as being malicious. To dismiss it, just click on "More info" and then "Run anyway".

Once installed, Rystrap is added to your Start Menu, where you can access the menu and reconfigure your preferences if needed.

## Code

Rystrap uses the [WPF UI](https://github.com/lepoco/wpfui) library for the user interface design. This repository includes a fork of WPF UI at `wpfui/`.


[shield-repo-license]:  https://img.shields.io/github/license/ndaju/Rystrap
[shield-repo-workflow]: https://img.shields.io/github/actions/workflow/status/ndaju/Rystrap/ci-release.yml?branch=main&label=builds
[shield-repo-releases]: https://img.shields.io/github/downloads/ndaju/Rystrap/latest/total?color=981bfe
[shield-repo-latest]:   https://img.shields.io/github/v/release/ndaju/Rystrap?color=7a39fb

[shield-crowdin-status]: https://badges.crowdin.net/Rystrap/localized.svg

[repo-license]:  https://github.com/ndaju/Rystrap/blob/main/LICENSE
[repo-actions]:  https://github.com/ndaju/Rystrap/actions
[repo-releases]: https://github.com/ndaju/Rystrap/releases
[repo-latest]:   https://github.com/ndaju/Rystrap/releases/latest

[crowdin-project]: https://crowdin.com/project/Rystrap