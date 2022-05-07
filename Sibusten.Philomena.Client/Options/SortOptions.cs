using Sibusten.Philomena.Api;

namespace Sibusten.Philomena.Client.Options
{
    public record SortOptions
    {
        /// <summary>
        /// The field to sort images by
        /// </summary>
        public SortField? SortField { get; init; }

        /// <summary>
        /// The direction to sort images
        /// </summary>
        public SortDirection? SortDirection { get; init; }

        /// <summary>
        /// The seed to use if set to sort randomly. If not provided, a seed will be generated.
        /// </summary>
        public int? RandomSeed { get; init; }
    }
}
