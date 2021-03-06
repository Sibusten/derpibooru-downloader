# Philomena Downloader
[![CI](https://github.com/Sibusten/derpibooru-downloader/actions/workflows/ci.yml/badge.svg)](https://github.com/Sibusten/derpibooru-downloader/actions/workflows/ci.yml)
[![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/Sibusten/derpibooru-downloader?include_prereleases)](https://github.com/Sibusten/derpibooru-downloader/releases)

A downloader for imageboards running [Philomena](https://github.com/derpibooru/philomena), such as [Derpibooru](https://derpibooru.org).

**NOTE:** Philomena Downloader v3.0.0 is currently in alpha. Data may not migrate nicely between versions just yet. The documentation for v2.1.0 can be found in the [v2.1.0 branch](https://github.com/Sibusten/derpibooru-downloader/tree/release-v2.1.0)

## Install .NET 5

This program requires the [.NET 5.0 runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime/).

[`dotnet-install`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script) is an alternative, and may be easier for some Linux users: `sudo bash dotnet-install.sh -c 5.0 --runtime dotnet --install-dir /usr/share/dotnet`

## Booru Setup
Boorus must be set up before downloading images. They require a name and the base URL of the booru. For example, to add [Derpibooru](https://derpibooru.org):

`philomena-downloader booru add derpibooru https://derpibooru.org`

## Usage
![Usage](Screenshots/usage.gif)

Run `philomena-downloader download --help` for a list of available options

## Presets

Presets can be set up for commonly used search options. They require a name and the options to use. The options are the same as they are for downloading. For example, to create a preset for the query in the usage gif:

`philomena-downloader preset add fluttershy --boorus derpibooru ponybooru --query fluttershy --limit 20`

Presets can be updated as well:

`philomena-downloader preset update fluttershy --sort-field Score`

Using a preset:

`philomena-downloader download --preset fluttershy`

Any other options given when downloading with a preset will override options in the preset. For example, this will use all options from the `fluttershy` preset, but download 100 images per booru instead:

`philomena-downloader download --preset fluttershy --limit 100`
