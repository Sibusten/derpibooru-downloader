namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public record PhilomenaImageDownloadProgressInfo
    {
        public string Action { get; init; } = "";
        public long Current { get; init; }
        public long? Total { get; init; }
    }
}
