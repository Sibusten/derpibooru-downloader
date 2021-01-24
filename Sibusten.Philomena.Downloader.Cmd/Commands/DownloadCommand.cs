using System;
using System.CommandLine;
using System.Threading.Tasks;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
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
            }.WithSearchQueryArgs().WithHandler(new Func<DownloadArgs, Task>(DownloadCommand));
        }

        private async Task DownloadCommand(DownloadArgs args)
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

            SearchConfig config = args.GetSearchConfig(baseConfig);

            // TODO: Start download
        }
    }
}
