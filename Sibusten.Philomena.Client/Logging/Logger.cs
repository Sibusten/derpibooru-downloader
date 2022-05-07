using Microsoft.Extensions.Logging;

namespace Sibusten.Philomena.Client.Logging
{
    public static class Logger
    {
        /// <summary>
        /// Gets or sets the logger factory singleton used for logging. Defaults to a logger factory that does nothing.
        /// </summary>
        public static ILoggerFactory Factory { get; set; } = LoggerFactory.Create(builder => {});
    }
}
