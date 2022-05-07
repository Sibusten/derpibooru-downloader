using System;

namespace Sibusten.Philomena.Client.Options
{
    public record ImageSearchOptions
    {
        /// <summary>
        /// The API Key to use when querying
        /// </summary>
        /// <value></value>
        public string? ApiKey { get; init; }

        /// <summary>
        /// The filter for the query
        /// </summary>
        public int? FilterId { get; init; }
    }
}
