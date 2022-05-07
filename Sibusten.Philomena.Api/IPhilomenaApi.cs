using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Api.Models;

namespace Sibusten.Philomena.Api
{
    public enum SortDirection
    {
        Descending,
        Ascending
    }

    public enum SortField
    {
        ImageId,
        LastModificationDate,
        InitialPostDate,
        AspectRatio,
        FaveCount,
        Upvotes,
        Downvotes,
        Score,
        WilsonScore,
        Relevance,
        Width,
        Height,
        Comments,
        TagCount,
        Pixels,
        FileSize,
        Duration,
        Random
    }
}
