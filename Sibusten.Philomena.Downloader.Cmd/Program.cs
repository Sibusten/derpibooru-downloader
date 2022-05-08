using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;
using Sibusten.Philomena.Downloader.Cmd.Commands;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
using Sibusten.Philomena.Downloader.Cmd.Reporters;
using Sibusten.Philomena.Downloader.Reporters;
using Sibusten.Philomena.Downloader.Settings;
using Sibusten.Philomena.Downloader.Utility;

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

            RootCommand rootCommand = new RootCommand("A downloader for imageboards running Philomena, such as Derpibooru")
            {
                new Argument<string>("query", "The search query"),
                new Option<string>(new[] { "--api-key", "-a" }, "The API key to use"),
                new Option<int>(new[] { "--filter", "-f" }, "The ID of the filter to use"),
                new Option<string>(new[] { "--image-path", "-I" }, "Where to save images and how to name them"),
                new Option<string>(new[] { "--json-path", "-J" }, "Where to save json files and how to name them. Json will not be saved unless this is given."),
                new Option<List<string>>(new[] { "--booru", "-b" }, "What booru to download from"),
                new Option<SvgMode>(new[] { "--svg-mode", "-g" }, "How to download SVG images"),
            };

            rootCommand.WithHandler(new Func<DownloadArgs, Task>(DownloadCommandFunc));

            await rootCommand.InvokeAsync(args);

#if DEBUG
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
#endif
        }

        private static async Task DownloadCommandFunc(DownloadArgs args)
        {
            SearchConfig searchConfig = args.GetSearchConfig();

            // Verify booru
            Uri booruBaseUri = UrlUtilities.GetWellFormedWebUri(searchConfig.Booru);

            // Download images
            using IImageDownloadReporter reporter = new AdvancedConsoleReporter(ImageDownloader.MaxDownloadThreads, $"Downloading search '{searchConfig.Query}' from '{booruBaseUri}'");
            PhilomenaApi api = new(booruBaseUri.ToString());
            PageBasedPhilomenaImageSearch imageSearch = new(api, searchConfig.Query, new()
            {
                FilterId = searchConfig.Filter is SearchConfig.NoFilter ? null : searchConfig.Filter
            });
            ImageDownloader downloader = new ImageDownloader(searchConfig);
            ParallelPhilomenaImageSearchDownloader imageSearchDownloader = new(imageSearch, downloader, ImageDownloader.MaxDownloadThreads);

            await imageSearchDownloader.BeginDownload(CancellationToken.None, reporter.SearchProgress, reporter.SearchDownloadProgress, reporter.IndividualDownloadProgresses);
        }
    }
}
