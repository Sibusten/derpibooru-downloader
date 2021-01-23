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

        static Command WithHandler(this Command command, string name)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo? method = typeof(Program).GetMethod(name, flags);

            ICommandHandler handler = CommandHandler.Create(method!);
            command.Handler = handler;
            return command;
        }

        static Command WithHandler(this Command command, Delegate handlerDelegate)
        {
            ICommandHandler handler = CommandHandler.Create(handlerDelegate);
            command.Handler = handler;
            return command;
        }

        static Command WithSearchQueryArgs(this Command command)
        {
            List<Symbol> SearchQueryArgs = new List<Symbol>
            {
                new Option<string>(new[] { "--query", "-q" }, "The search query"),
                new Option<int>(new[] { "--limit", "-l" }, "The maximum number of images to download. Defaults to all images"),
                new Option<int>(new[] { "--filter", "-f" }, "The ID of the filter to use"),
                new Option<SortField>(new[] { "--sort-field", "-S" }, "How to sort images"),
                new Option<SortDirection>(new[] { "--sort-direction", "-D" }, "The direction to sort images"),
                new Option<string>(new[] { "--image-path", "-I" }, "Where to save images and how to name them"),
                new Option<string>(new[] { "--json-path", "-J" }, "Where to save json files and how to name them"),
                new Option<bool?>(new[] { "--skip-images", "-i" }, "Skip saving images"),
                new Option<bool>(new[] { "--save-json", "-j" }, "Save json metadata files"),
                new Option<bool>(new[] { "--update-json", "-u" }, "Overwrite json metadata files with new data"),
                new Option<List<string>>(new[] { "--boorus", "-b" }, "What booru to download from. Multiple boorus can be given"),
            };

            foreach (Symbol arg in SearchQueryArgs)
            {
                command.Add(arg);
            }

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

            PresetCommand presetCommand = new PresetCommand(configAccess);

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru")
            {
                new Command("download", "Search for and download images.")
                {
                    new Option<string>(new[] { "--preset", "-p" }, "The preset to use as a base. If no preset is given, the default is used"),
                    new Option<string>(new[] { "--api-key", "-a" }, "The API key to use"),
                }.WithSearchQueryArgs().WithHandler(nameof(DownloadCommand)),

                new Command("preset", "Manage presets")
                {
                    new Command("list", "List presets")
                    {
                        new Option<bool>(new[] { "--verbose", "-v" }, "Print all preset information as JSON")
                    }.WithHandler(new Action<PresetListCommandArgs>(presetCommand.ListCommand)),

                    new Command("add", "Add a new preset")
                    {
                        new Argument<string>("name", "The name of the new preset"),
                    }.WithSearchQueryArgs().WithHandler(new Action<PresetAddCommandArgs>(presetCommand.AddCommand)),

                    new Command("delete", "Delete a preset")
                    {
                        new Argument<string>("name", "The preset to delete")
                    }.WithHandler(new Action<PresetRemoveCommandArgs>(presetCommand.RemoveCommand)),

                    new Command("rename", "Rename a preset.")
                    {
                        new Argument<string>("from", "The preset to rename"),
                        new Argument<string>("to", "The new name of the preset")
                    }.WithHandler(new Action<PresetRenameCommandArgs>(presetCommand.RenameCommand)),

                    new Command("copy", "Copy a preset.")
                    {
                        new Argument<string>("from", "The preset to copy from"),
                        new Argument<string>("to", "The preset to copy to")
                    }.WithHandler(new Action<PresetCopyCommandArgs>(presetCommand.CopyCommand)),

                    new Command("update", "Update a preset. Only given options are modified")
                    {
                        new Argument<string>("name", "The preset to update")
                    }.WithSearchQueryArgs().WithHandler(new Action<PresetUpdateCommandArgs>(presetCommand.UpdateCommand))
                }
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
