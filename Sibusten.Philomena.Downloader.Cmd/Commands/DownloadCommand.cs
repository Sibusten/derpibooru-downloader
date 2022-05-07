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
            }.WithSearchQueryArgs().WithHandler(new Func<DownloadArgs, Task>(DownloadCommandFunc));
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
