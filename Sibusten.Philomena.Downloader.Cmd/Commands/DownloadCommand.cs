using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
using Sibusten.Philomena.Downloader.Cmd.Reporters;
using Sibusten.Philomena.Downloader.Reporters;
using Sibusten.Philomena.Downloader.Settings;
using Sibusten.Philomena.Downloader.Utility;

namespace Sibusten.Philomena.Downloader.Cmd.Commands
{
    public class DownloadCommand
    {
        public Command GetCommand()
        {
            return new Command("download", "Search for and download images.")
            {
                new Option<string>(new[] { "--api-key", "-a" }, "The API key to use"),
                new Option<string>(new[] { "--query", "-q" }, "The search query"),
                new Option<int>(new[] { "--limit", "-l" }, "The maximum number of images to download. Defaults to all images"),
                new Option<int>(new[] { "--filter", "-f" }, "The ID of the filter to use"),
                new Option<string>(new[] { "--image-path", "-I" }, "Where to save images and how to name them"),
                new Option<string>(new[] { "--json-path", "-J" }, "Where to save json files and how to name them"),
                new Option<bool?>(new[] { "--skip-images", "-i" }, "Skip saving images"),
                new Option<bool>(new[] { "--save-json", "-j" }, "Save json metadata files"),
                new Option<bool>(new[] { "--update-json", "-u" }, "Overwrite json metadata files with new data"),
                new Option<List<string>>(new[] { "--booru", "-b" }, "What booru to download from"),
                new Option<SvgMode>(new[] { "--svg-mode", "-g" }, "How to download SVG images"),
            }.WithHandler(new Func<DownloadArgs, Task>(DownloadCommandFunc));
        }

        private async Task DownloadCommandFunc(DownloadArgs args)
        {
            SearchConfig searchConfig = args.GetSearchConfig();

            // Verify booru
            Uri booruBaseUri = UrlUtilities.GetWellFormedWebUri(searchConfig.Booru);

            // Download images
            using IImageDownloadReporter reporter = new AdvancedConsoleReporter(ImageDownloader.MaxDownloadThreads, $"Downloading search '{searchConfig.Query}' from '{booruBaseUri}'");

            ImageDownloader downloader = new ImageDownloader(booruBaseUri, searchConfig);
            await downloader.StartDownload(downloadReporter: reporter);
        }
    }
}
