using Sibusten.Philomena.Downloader.Cmd.Commands.Common.Arguments;

namespace Sibusten.Philomena.Downloader.Cmd.Commands.Arguments
{
    public class DownloadArgs : SearchQueryArgs
    {
        public string? ApiKey { get; set; }
    }
}
