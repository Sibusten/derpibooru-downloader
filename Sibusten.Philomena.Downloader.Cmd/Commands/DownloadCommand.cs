using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using ShellProgressBar;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Client.Utilities;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
using Sibusten.Philomena.Downloader.Cmd.Reporters;
using Sibusten.Philomena.Downloader.Reporters;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands
{
    public class DownloadCommand
    {
        private ConfigAccess _configAccess;

        public DownloadCommand(ConfigAccess configAccess)
        {
            _configAccess = configAccess;
        }

        public Command GetCommand()
        {
            return new Command("download", "Search for and download images.")
            {
                new Option<string>(new[] { "--preset", "-p" }, "The preset to use as a base. If no preset is given, the default is used"),
                new Option<string>(new[] { "--api-key", "-a" }, "The API key to use"),
            }.WithSearchQueryArgs().WithHandler(new Func<DownloadArgs, Task>(DownloadCommandFunc));
        }

        private async Task DownloadCommandFunc(DownloadArgs args)
        {
            SearchConfig? baseConfig = null;
            if (args.Preset is not null)
            {
                SearchPreset? preset = _configAccess.GetPreset(args.Preset);

                if (preset is null)
                {
                    Console.WriteLine($"Preset '{args.Preset}' does not exist");
                    return;
                }

                baseConfig = preset.Config;
            }

            SearchConfig searchConfig = args.GetSearchConfig(baseConfig);

            // Gather all boorus and ensure they exist
            List<BooruConfig> booruConfigs = new List<BooruConfig>();
            foreach (string booruName in searchConfig.Boorus)
            {
                BooruConfig? booruConfig = _configAccess.GetBooru(booruName);

                if (booruConfig is null)
                {
                    Console.WriteLine($"Booru '{booruName}' was not found");
                    return;
                }

                booruConfigs.Add(booruConfig);
            }

            // Download images on each booru
            foreach (BooruConfig booruConfig in booruConfigs)
            {
                IImageDownloadReporter reporter = new SingleLineImageDownloadConsoleReporter();

                IProgress<ImageDownloadProgressInfo> progress = new SyncProgress<ImageDownloadProgressInfo>(reporter.ReportProgress);

                ImageDownloader downloader = new ImageDownloader(booruConfig, searchConfig);
                await downloader.StartDownload(progress: progress);
            }
        }
    }
}
