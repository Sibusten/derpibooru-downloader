namespace Sibusten.Philomena.Downloader.Cmd.Commands.Arguments
{
    public class DownloadArgs : SearchQueryArgs
    {
        public string? Preset { get; set; }
        public string? ApiKey { get; set; }
    }
}
