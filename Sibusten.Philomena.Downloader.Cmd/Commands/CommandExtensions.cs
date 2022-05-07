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
    }
}
