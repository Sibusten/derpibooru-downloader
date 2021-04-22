using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Newtonsoft.Json;
using Sibusten.Philomena.Downloader.Cmd.Commands.Booru.Arguments;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands.Booru
{
    public class BooruCommand
    {
        private ConfigAccess _configAccess;

        public BooruCommand(ConfigAccess configAccess)
        {
            _configAccess = configAccess;
        }

        public Command GetCommand()
        {
            return new Command("booru", "Manage boorus")
            {
                new Command("list", "List boorus")
                {
                    new Option<bool>(new[] { "--verbose", "-v" }, "Print all booru information as JSON")
                }.WithHandler(new Action<BooruListCommandArgs>(ListCommand)),

                new Command("add", "Add a new booru")
                {
                    new Argument<string>("name", "The name of the new booru"),
                    new Argument<string>("base-url", "The base url of the new booru"),
                    new Option<string>(new[] { "--api-key", "-a" }, "The api key to use with the new booru")
                }.WithHandler(new Action<BooruAddCommandArgs>(AddCommand)),

                new Command("delete", "Delete a booru")
                {
                    new Argument<string>("name", "The booru to delete")
                }.WithHandler(new Action<BooruRemoveCommandArgs>(RemoveCommand)),

                new Command("rename", "Rename a booru")
                {
                    new Argument<string>("from", "The booru to rename"),
                    new Argument<string>("to", "The new name of the booru")
                }.WithHandler(new Action<BooruRenameCommandArgs>(RenameCommand)),

                new Command("update", "Update a booru. Only given options are modified")
                {
                    new Argument<string>("name", "The booru to update"),
                    new Option<string>(new[] { "--base-url", "-u" }, "The base url of the booru"),
                    new Option<string>(new[] { "--api-key", "-a" }, "The api key to use with the booru")
                }.WithHandler(new Action<BooruUpdateCommandArgs>(UpdateCommand))
            };
        }

        private void ListCommand(BooruListCommandArgs args)
        {
            List<BooruConfig> boorus = _configAccess.GetBoorus();

            if (args.Verbose)
            {
                // Convert boorus to a JSON list
                string boorusJson = JsonConvert.SerializeObject(boorus, Formatting.Indented);

                Console.WriteLine(boorusJson);
            }
            else
            {
                // Create a list of the booru names
                IEnumerable<string> booruNames = boorus.Select(p => p.Name);
                string booruList = string.Join(Environment.NewLine, booruNames);

                if (string.IsNullOrEmpty(booruList))
                {
                    Console.WriteLine("There are no boorus");
                }
                else
                {
                    Console.WriteLine(booruList);
                }
            }
        }

        private void AddCommand(BooruAddCommandArgs args)
        {
            BooruConfig? existingBooru = _configAccess.GetBooru(args.Name);
            if (existingBooru is not null)
            {
                Console.WriteLine($"Booru '{args.Name}' already exists");
                return;
            }

            BooruConfig booru = new BooruConfig(args.Name, args.BaseUrl)
            {
                ApiKey = args.ApiKey
            };
            _configAccess.UpsertBooru(booru);

            Console.WriteLine($"Added booru '{args.Name}'");
        }

        private void RemoveCommand(BooruRemoveCommandArgs args)
        {
            BooruConfig? booru = _configAccess.GetBooru(args.Name);

            if (booru is null)
            {
                Console.WriteLine($"Booru '{args.Name}' does not exist");
                return;
            }

            _configAccess.DeleteBooru(booru.Id);

            // Remove booru from any presets
            foreach (SearchPreset preset in _configAccess.GetPresets())
            {
                if (preset.Config.Boorus.Contains(args.Name))
                {
                    preset.Config.Boorus.Remove(args.Name);
                    _configAccess.UpsertPreset(preset);
                }
            }

            Console.WriteLine($"Deleted booru '{args.Name}'");
        }

        private void RenameCommand(BooruRenameCommandArgs args)
        {
            BooruConfig? booruFrom = _configAccess.GetBooru(args.From);

            if (booruFrom is null)
            {
                Console.WriteLine($"Booru '{args.From}' does not exist");
                return;
            }

            BooruConfig? booruTo = _configAccess.GetBooru(args.To);

            if (booruTo is not null)
            {
                Console.WriteLine($"Booru '{args.To}' already exists");
                return;
            }

            // Update the booru's name
            booruFrom.Name = args.To;
            _configAccess.UpsertBooru(booruFrom);

            // Rename booru in any presets that used it
            foreach (SearchPreset preset in _configAccess.GetPresets())
            {
                if (preset.Config.Boorus.Contains(args.From))
                {
                    preset.Config.Boorus.Remove(args.From);
                    preset.Config.Boorus.Add(args.To);
                    _configAccess.UpsertPreset(preset);
                }
            }

            Console.WriteLine($"Renamed booru '{args.From}' to '{args.To}'");
        }

        private void UpdateCommand(BooruUpdateCommandArgs args)
        {
            BooruConfig? booru = _configAccess.GetBooru(args.Name);
            if (booru is null)
            {
                Console.WriteLine($"Booru '{args.Name}' does not exist");
                return;
            }

            if (args.ApiKey is not null)
            {
                booru.ApiKey = args.ApiKey;
            }

            if (args.BaseUrl is not null)
            {
                booru.BaseUrl = args.BaseUrl;
            }

            _configAccess.UpsertBooru(booru);

            Console.WriteLine($"Updated booru '{args.Name}'");
        }
    }
}
