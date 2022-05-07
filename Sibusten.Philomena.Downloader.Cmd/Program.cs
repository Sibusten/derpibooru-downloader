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
            DownloadCommand downloadCommand = new DownloadCommand();

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru")
            {
                downloadCommand.GetCommand(),
            };

            await rootCommand.InvokeAsync(args);

#if DEBUG
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
#endif
        }
    }
}
