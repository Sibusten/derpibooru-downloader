using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Newtonsoft.Json;
using Sibusten.Philomena.Downloader.Cmd.Commands.Arguments;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands
{
    public class PresetCommand
    {
        private ConfigAccess _configAccess;

        public PresetCommand(ConfigAccess configAccess)
        {
            _configAccess = configAccess;
        }

        public Command GetCommand()
        {
            return new Command("preset", "Manage presets")
            {
                new Command("list", "List presets")
                {
                    new Option<bool>(new[] { "--verbose", "-v" }, "Print all preset information as JSON")
                }.WithHandler(new Action<PresetListCommandArgs>(ListCommand)),

                new Command("add", "Add a new preset")
                {
                    new Argument<string>("name", "The name of the new preset"),
                }.WithSearchQueryArgs().WithHandler(new Action<PresetAddCommandArgs>(AddCommand)),

                new Command("delete", "Delete a preset")
                {
                    new Argument<string>("name", "The preset to delete")
                }.WithHandler(new Action<PresetRemoveCommandArgs>(RemoveCommand)),

                new Command("rename", "Rename a preset.")
                {
                    new Argument<string>("from", "The preset to rename"),
                    new Argument<string>("to", "The new name of the preset")
                }.WithHandler(new Action<PresetRenameCommandArgs>(RenameCommand)),

                new Command("copy", "Copy a preset.")
                {
                    new Argument<string>("from", "The preset to copy from"),
                    new Argument<string>("to", "The preset to copy to")
                }.WithHandler(new Action<PresetCopyCommandArgs>(CopyCommand)),

                new Command("update", "Update a preset. Only given options are modified")
                {
                    new Argument<string>("name", "The preset to update")
                }.WithSearchQueryArgs().WithHandler(new Action<PresetUpdateCommandArgs>(UpdateCommand))
            };
        }

        public void ListCommand(PresetListCommandArgs args)
        {
            List<SearchPreset> presets = _configAccess.GetPresets();

            if (args.Verbose)
            {
                // Convert presets to a JSON list
                string presetsJson = JsonConvert.SerializeObject(presets, Formatting.Indented);

                Console.WriteLine(presetsJson);
            }
            else
            {
                // Create a list of the preset names
                IEnumerable<string> presetNames = presets.Select(p => p.Name);
                string presetList = string.Join(Environment.NewLine, presetNames);

                if (string.IsNullOrEmpty(presetList))
                {
                    Console.WriteLine("There are no presets");
                }
                else
                {
                    Console.WriteLine(presetList);
                }
            }
        }

        public void AddCommand(PresetAddCommandArgs args)
        {
            SearchPreset? existingPreset = _configAccess.GetPreset(args.Name);
            if (existingPreset is not null)
            {
                Console.WriteLine($"Preset '{args.Name}' already exists");
                return;
            }

            SearchConfig config = args.GetSearchConfig();
            SearchPreset preset = new SearchPreset(args.Name, config);
            _configAccess.UpsertPreset(preset);

            Console.WriteLine($"Added preset '{args.Name}'");
        }

        public void RemoveCommand(PresetRemoveCommandArgs args)
        {
            SearchPreset? preset = _configAccess.GetPreset(args.Name);

            if (preset is null)
            {
                Console.WriteLine($"Preset '{args.Name}' does not exist");
                return;
            }

            _configAccess.DeletePreset(preset.Id);

            Console.WriteLine($"Deleted preset '{args.Name}'");
        }

        public void RenameCommand(PresetRenameCommandArgs args)
        {
            SearchPreset? presetFrom = _configAccess.GetPreset(args.From);

            if (presetFrom is null)
            {
                Console.WriteLine($"Preset '{args.From}' does not exist");
                return;
            }

            SearchPreset? presetTo = _configAccess.GetPreset(args.To);

            if (presetTo is not null)
            {
                Console.WriteLine($"Preset '{args.To}' already exists");
                return;
            }

            // Update the preset's name
            presetFrom.Name = args.To;
            _configAccess.UpsertPreset(presetFrom);

            Console.WriteLine($"Renamed preset '{args.From}' to '{args.To}'");
        }

        public void CopyCommand(PresetCopyCommandArgs args)
        {
            SearchPreset? presetFrom = _configAccess.GetPreset(args.From);

            if (presetFrom is null)
            {
                Console.WriteLine($"Preset '{args.From}' does not exist");
                return;
            }

            SearchPreset? presetTo = _configAccess.GetPreset(args.To);

            if (presetTo is not null)
            {
                Console.WriteLine($"Preset '{args.To}' already exists");
                return;
            }

            // Create a new preset as a copy
            presetTo = new SearchPreset(args.To, presetFrom.Config);
            _configAccess.UpsertPreset(presetTo);

            Console.WriteLine($"Copied preset '{args.From}' to '{args.To}'");
        }

        public void UpdateCommand(PresetUpdateCommandArgs args)
        {
            SearchPreset? preset = _configAccess.GetPreset(args.Name);
            if (preset is null)
            {
                Console.WriteLine($"Preset '{args.Name}' does not exists");
                return;
            }

            // Create a new config based on the preset's config
            SearchConfig config = args.GetSearchConfig(baseConfig: preset.Config);
            preset.Config = config;
            _configAccess.UpsertPreset(preset);

            Console.WriteLine($"Updated preset '{args.Name}'");
        }
    }
}
