using System;
using System.Collections.Generic;
using System.Linq;
using Sibusten.Philomena.Downloader.Settings;
using System.CommandLine;
using System.Threading.Tasks;
using Sibusten.Philomena.Api;
using System.Reflection;
using System.CommandLine.Invocation;

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

        static Command WithHandler(this Command command, string name)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo? method = typeof(Program).GetMethod(name, flags);

            ICommandHandler handler = CommandHandler.Create(method!);
            command.Handler = handler;
            return command;
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

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru.")
            {
                new Command("download", "Search for and download images")
                {
                    new Option<string>(new[] { "--preset", "-p" }, "The preset to use. If no preset is given, the default is used."),
                    new Option<string>(new[] { "--query", "-q" }, "The search query."),
                    new Option<int>(new[] { "--limit", "-l" }, "The maximum number of images to download. Defaults to all images."),
                    new Option<int>(new[] { "--filter", "-f" }, "The ID of the filter to use."),
                    new Option<SortField>(new[] { "--sort-field", "-S" }, "How to sort images."),
                    new Option<SortDirection>(new[] { "--sort-direction", "-D" }, "The direction to sort images."),
                    new Option<string>(new[] { "--image-path", "-I" }, "Where to save images and how to name them."),
                    new Option<string>(new[] { "--json-path", "-J" }, "Where to save json files and how to name them."),
                    new Option<bool?>(new[] { "--skip-images", "-i" }, "Skip saving images."),
                    new Option<bool>(new[] { "--save-json", "-j" }, "Save json metadata files."),
                    new Option<bool>(new[] { "--update-json", "-u" }, "Overwrite json metadata files with new data."),
                    new Option<List<string>>(new[] { "--boorus", "-b" }, "What booru to download from. Multiple boorus can be given."),
                    new Option<string>(new[] { "--api-key", "-a" }, "The API key to use."),
                }.WithHandler(nameof(DownloadCommand))
            };

            await rootCommand.InvokeAsync(args);

            // Parser.Default.ParseArguments<SearchArguments>(args).WithParsed<SearchArguments>(searchArgs =>
            // {
            //     SearchConfig config = searchArgs.ToSearchConfig();
            // });

            Console.WriteLine("Hello World!");

            Class1.HelloWorld();
        }

        private static async Task DownloadCommand(DownloadArgs downloadArgs)
        {
            SearchConfig? baseConfig = null;
            if (downloadArgs.Preset is not null)
            {
                // TODO: load preset config from settings
            }

            SearchConfig config = downloadArgs.GetSearchConfig(baseConfig);
        }
    }
}
