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
            using ConfigAccess configAccess = new ConfigAccess();
            DownloadCommand downloadCommand = new DownloadCommand(configAccess);
            PresetCommand presetCommand = new PresetCommand(configAccess);
            BooruCommand booruCommand = new BooruCommand(configAccess);

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru")
            {
                downloadCommand.GetCommand(),
                presetCommand.GetCommand(),
                booruCommand.GetCommand(),
            };

            await rootCommand.InvokeAsync(args);
        }
    }
}
