using System;
using System.Collections.Generic;
using System.Linq;
using Sibusten.Philomena.Downloader.Settings;
using System.CommandLine;
using System.Threading.Tasks;
using Sibusten.Philomena.Api;
using System.Reflection;
using System.CommandLine.Invocation;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
using Newtonsoft.Json;
using Sibusten.Philomena.Downloader.Cmd.Commands;
using Sibusten.Philomena.Downloader.Cmd.Commands.Preset;
using Sibusten.Philomena.Downloader.Cmd.Commands.Booru;

namespace Sibusten.Philomena.Downloader.Cmd
{
    public static class Program
    {
        private static ConfigAccess configAccess = new ConfigAccess();

        private static string[] GetArgsFromConsole()
        {
            Console.WriteLine("Enter program arguments, one per line. Finish by entering an empty line.");

            List<string> args = new List<string>();
            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                args.Add(input);
            }

            return args.ToArray();
        }

        static async Task Main(string[] args)
        {
#if DEBUG
            // Prompt for arguments when debugging if none are given
            if (!args.Any())
            {
                args = GetArgsFromConsole();
            }
#endif

            PresetCommand presetCommand = new PresetCommand(configAccess);
            BooruCommand booruCommand = new BooruCommand(configAccess);

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru")
            {
                new Command("download", "Search for and download images.")
                {
                    new Option<string>(new[] { "--preset", "-p" }, "The preset to use as a base. If no preset is given, the default is used"),
                    new Option<string>(new[] { "--api-key", "-a" }, "The API key to use"),
                }.WithSearchQueryArgs().WithHandler(new Func<DownloadArgs, Task>(DownloadCommand)),

                presetCommand.GetCommand(),
                booruCommand.GetCommand(),
            };

            await rootCommand.InvokeAsync(args);
        }

        private static async Task DownloadCommand(DownloadArgs args)
        {
            SearchConfig? baseConfig = null;
            if (args.Preset is not null)
            {
                // TODO: load preset config from settings
            }

            SearchConfig config = args.GetSearchConfig(baseConfig);
        }
    }
}
