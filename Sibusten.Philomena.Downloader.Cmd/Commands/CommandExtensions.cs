using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Client.Options;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands
{
    public static class CommandExtensions
    {
        public static Command WithHandler(this Command command, Delegate handlerDelegate)
        {
            ICommandHandler handler = CommandHandler.Create(handlerDelegate);
            command.Handler = handler;
            return command;
        }

        public static Command WithSearchQueryArgs(this Command command)
        {
            List<Symbol> SearchQueryArgs = new List<Symbol>
            {
                new Option<string>(new[] { "--query", "-q" }, "The search query"),
                new Option<int>(new[] { "--limit", "-l" }, "The maximum number of images to download. Defaults to all images"),
                new Option<int>(new[] { "--filter", "-f" }, "The ID of the filter to use"),
                new Option<string>(new[] { "--image-path", "-I" }, "Where to save images and how to name them"),
                new Option<string>(new[] { "--json-path", "-J" }, "Where to save json files and how to name them"),
                new Option<bool?>(new[] { "--skip-images", "-i" }, "Skip saving images"),
                new Option<bool>(new[] { "--save-json", "-j" }, "Save json metadata files"),
                new Option<bool>(new[] { "--update-json", "-u" }, "Overwrite json metadata files with new data"),
                new Option<List<string>>(new[] { "--boorus", "-b" }, "What booru to download from. Multiple boorus can be given"),
                new Option<SvgMode>(new[] { "--svg-mode", "-g" }, "How to download SVG images"),
            };

            foreach (Symbol arg in SearchQueryArgs)
            {
                command.Add(arg);
            }

            return command;
        }
    }
}
