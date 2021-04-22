# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

## [3.0.0-alpha.1] - 2021-04-21

### Added
- Commands for preset management
    - `preset list`: List presets.
    - `preset add`: Add a new preset.
    - `preset delete`: Delete a preset.
    - `preset rename`: Rename a preset.
    - `preset copy`: Copy a preset.
    - `preset update`: Update a preset. Only given options are modified.
- Commands for booru management
    - `booru list`: List boorus
    - `booru add`: Add a new booru
    - `booru delete`: Delete a booru
    - `booru rename`: Rename a booru
    - `booru update`: Update a booru. Only given options are modified
- File path tags
    - `{booru_name}`: The name of the booru
- Progress reporting
    - Total download progress
    - Individual download progress

### Changed
- Rewrote app in C# using .NET 5
- Move image downloads to `download` sub-command
- Some command line options are renamed
    - `--search-format` => `--sort-field`
    - `--search-direction` => `--sort-direction`
- `--booru-url` is now `--boorus`
    - Booru IDs are used instead of urls. Boorus are configured first and then used in searches.
    - Multiple boorus can be listed, and images will be downloaded from all sites: `--boorus derpibooru ponybooru`

### Removed
- GUI version of the program
- PerPage option
    - The max of 50 images per page is always used
- StartPage option
    - Search terms should be used instead for this purpose (`id.gt:1234`, etc.)
- Options to save comments and favorites in the JSON
    - This feature hasn't worked for a long time, since Philomena changed how comments and favorites are requested
- `--json-only` command line option
    - Instead, use `--save-json --skip-images`

## [2.1.0] - 2020-07-15

### Added
- Ability to download from any Philomena site by setting a custom booru URL
- `{booru_url}` file naming tag
- `--booru-url` (`-b`) command line option to set custom booru URL for a query

## [2.0.1] - 2020-07-03

### Fixed
- Downloader getting stuck when using the `OR` (`||`) operator

## [2.0.0] - 2020-02-15

### Changed
- Update to new Philomena API
- Don't disable filter selection if an API key is given

### Fixed
- Crash when image limit is set to a multiple of images per page

## [1.4.5] - 2019-06-29

### Changed
- Improved download speed and server impact when downloading by Creation Date
    - Uses `id` filters instead of pages, which perform better
    - Creation Date should always be used if possible
- Use `www.derpibooru.org` for downloading metadata, since `derpibooru.org` is blocked in Russia

## [1.4.4] - 2019-05-13

### Fixed
- Image corruption when a download is interrupted
    - Images are downloaded to a temp file first, and moved after downloading

## [1.4.3] - 2019-04-22

### Changed
- SSL version

### Fixed
- `{month}` and `{day}` tags not having a leading zero when the number is a single digit

## [1.4.2] - 2019-03-27

### Added
- Option to download only JSON files

## [1.4.1] - 2018-11-06

### Added
- 32 bit version of the program

### Changed
- SSL version

### Fixed
- Skip downloading unrendered images
    - This can happen if an image is downloaded very shortly after being uploaded

## [1.4.0] - 2018-08-08

### Added
- A command line version of the program
    - Shares the same settings file with the Gui version
    - Can be used to run any download the Gui version could run
    - Can use presets created in the Gui version
    - *NOTE:* The GUI version must be ran first to generate the settings file. The command line version cannot create the file.

## [1.3.7] - 2018-06-22

### Added
- `{rating}` tag for path formatting
    - It will contain the rating (or ratings, if there are multiple) that the image is tagged with.

## [1.3.6] - 2018-04-22

### Added
- SVG download options
    - Download PNG version only
    - Download SVG version only
    - Download both

## [1.3.5] - 2018-04-07

### Changed
- Move to msvc compiler for windows releases, using qt 5.10

### Fixed
- SVG images not being downloaded

## [1.3.4] - 2018-02-25

### Fixed
- Image IDs incorrectly read from metadata
    - Derpibooru changed IDs from strings back to ints

## [1.3.2] - 2017-09-06

### Fixed
- Missing images causing the downloader to freeze

## [1.3.1] - 2017-03-11

### Changed
- Use a random seed with the random sorting mode to prevent duplicates across more than one page

### Fixed
- Include `platforms\qwindows.dll` in binary release
- Include openssl in binary release (`libeay32.dll`, `ssleay32.dll`)

## [1.3.0] - 2017-02-13

### Changed
- Updated to support Derpibooru's new search methods
    - User-specific search parameters such as favorites and upvotes now go in the search query itself

<!-- Links -->
[Keep a Changelog]: https://keepachangelog.com/
[Semantic Versioning]: https://semver.org/

<!-- Versions -->
[Unreleased]: https://github.com/Sibusten/derpibooru-downloader/compare/v2.1.0...HEAD

[2.1.0]: https://github.com/Sibusten/derpibooru-downloader/compare/v2.0.1..v2.1.0
[2.0.1]: https://github.com/Sibusten/derpibooru-downloader/compare/v2.0.0..v2.0.1
[2.0.0]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.5..v2.0.0
[1.4.5]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.4..v1.4.5
[1.4.4]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.3..v1.4.4
[1.4.3]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.2..v1.4.3
[1.4.2]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.1..v1.4.2
[1.4.1]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.4.0..v1.4.1
[1.4.0]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.7..v1.4.0
[1.3.7]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.6..v1.3.7
[1.3.6]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.5..v1.3.6
[1.3.5]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.4..v1.3.5
[1.3.4]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.3..v1.3.4
[1.3.3]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.2..v1.3.3
[1.3.2]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3.1..v1.3.2
[1.3.1]: https://github.com/Sibusten/derpibooru-downloader/compare/v1.3..v1.3.1
[1.3.0]: https://github.com/Sibusten/derpibooru-downloader/releases/v1.3
