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

    public interface IPhilomenaApi
    {
        /// <summary>
        /// Fetches a comment response for the comment ID
        /// </summary>
        /// <param name="commentId">The comment ID to fetch</param>
        /// <returns>The comment response</returns>
        // CommentJson GetComment(int commentId);

        /// <summary>
        /// Fetches an image response for the image ID
        /// </summary>
        /// <param name="imageId">The image ID to fetch</param>
        /// <returns>The image response</returns>
        Task<ImageResponseModel> GetImage(int imageId, string? apiKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submits a new image. Both key and url are required. Errors will result in an {"errors":image-errors-response}. (TODO)
        /// </summary>
        /// <param name="apiKey">The API key of the user submitting the image</param>
        /// <param name="imageUrl">The direct URL to the image file</param>
        /// <returns></returns>
        // ImageJson SubmitImage(string apiKey, string imageUrl);

        /// <summary>
        /// Fetches an image response for the for the current featured image.
        /// </summary>
        /// <returns>The featured image</returns>
        // ImageJson GetFeaturedImage();

        /// <summary>
        /// Fetches a tag response for the tag slug.
        /// </summary>
        /// <param name="tagSlug">The tag slug to fetch</param>
        /// <returns>The tag response</returns>
        Task<TagResponseModel> GetTagAsync(string tagSlug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches a post response for the post ID
        /// </summary>
        /// <param name="postId">The post ID to fetch</param>
        /// <returns>The post response</returns>
        // PostJson GetPost(int postId);

        /// <summary>
        /// Fetches a profile response for the user ID
        /// </summary>
        /// <param name="userId">The user ID to fetch</param>
        /// <returns>The user response</returns>
        // UserJson GetUser(int userId);

        /// <summary>
        /// Fetches a filter response for the filter ID
        /// </summary>
        /// <param name="filterId">The filter ID to fetch</param>
        /// <param name="apiKey">The user's API key</param>
        /// <returns>The filter response</returns>
        // FilterJson GetFilter(int filterId, string? apiKey = null);

        /// <summary>
        /// Fetches a list of filter responses that are flagged as being system filters (and thus usable by anyone).
        /// </summary>
        /// <param name="page">The page of filters to fetch (unused?)</param>
        /// <returns>The list of system filters</returns>
        // List<FilterJson> GetSystemFilters(int page = 1);

        /// <summary>
        /// Fetches a list of filter responses that belong to the user.
        /// </summary>
        /// <param name="apiKey">The user's API key</param>
        /// <param name="page">The page of filters to fetch</param>
        /// <returns>The list of user filters</returns>
        // List<FilterJson> GetUserFilters(string apiKey, int page)

        /// <summary>
        /// Fetches an oEmbed response for the given app link or CDN URL.
        /// </summary>
        /// <param name="url">The URL to fetch an oEmbed response for</param>
        /// <returns>The oEmbed response</returns>
        // OembedJson GetOembed(string url);

        /// <summary>
        /// Executes the search given by the query and returns comment responses sorted by descending creation time.
        /// </summary>
        /// <param name="query">The query used to search for comments</param>
        /// <param name="page">The page of comments to fetch</param>
        /// <param name="perPage">How many comments to fetch per page. Maximum of 50.
        /// <param name="apiKey">The user's API key</param>
        /// <returns>A page of comments matching the search query</returns>
        // CommentSearchRootJson SearchComments(string query, int page, int perPage, string apiKey);

        /// <summary>
        /// Executes the search given by the query and returns gallery responses sorted by descending creation time.
        /// </summary>
        /// <param name="query">The query used to search for galleries</param>
        /// <param name="page">The page of galleries to fetch</param>
        /// <param name="perPage">How many galleries to fetch per page. Maximum of 50.</param>
        /// <param name="apiKey">The user's API key</param>
        /// <returns>A page of galleries matching the search query</returns>
        // GallerySearchRoot SearchGalleries(string query, int page, int perPage, string apiKey);

        /// <summary>
        /// Executes the search given by the query and returns post responses sorted by descending creation time.
        /// </summary>
        /// <param name="query">The query used to search for posts</param>
        /// <param name="page">The page of posts to fetch</param>
        /// <param name="perPage">How many posts to fetch per page. Maximum of 50.</param>
        /// <param name="apiKey">The user's API key</param>
        /// <returns>A page of posts matching the search query</returns>
        // PostSearchRootJson SearchPosts(string query, int page, int perPage, string apiKey);

        /// <summary>
        /// Executes the search given by the query and returns image responses
        /// </summary>
        /// <param name="query">The query used to search for images</param>
        /// <param name="page">The page of images to fetch</param>
        /// <param name="perPage">How many images to fetch per page. Maximum of 50.</param>
        /// <param name="sortField">The field to sort by</param>
        /// <param name="sortDirection">The direction to sort by</param>
        /// <param name="filterId">The filter to use when searching</param>
        /// <param name="apiKey">The user's API key</param>
        /// <returns>A page of images matching the search query</returns>
        Task<ImageSearchModel> SearchImagesAsync(string query, int? page, int? perPage, SortField? sortField, SortDirection? sortDirection, int? filterId, string? apiKey, int? randomSeed, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the search given by the query and returns tag responses sorted by descending image count.
        /// </summary>
        /// <param name="query">The query used to search for tags</param>
        /// <param name="page">The page of tags to fetch</param>
        /// <param name="perPage">How many tags to fetch per page. Maximum of 50.</param>
        /// <returns>A page of tags matching the search query</returns>
        Task<TagSearchModel> SearchTagsAsync(string query, int? page, int? perPage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns image responses based on the results of reverse-searching the image given by the url.
        /// </summary>
        /// <param name="imageUrl">The direct URL to the image file</param>
        /// <param name="distance">How similar the matched images must be. Ranges from 0 to 1.</param>
        /// <param name="apiKey">The user's API key</param>
        /// <returns>A list of image responses that are similar to the searched image</returns>
        // List<ImageJson> ReverseImageSearch(string imageUrl, double distance, string apiKey);

        /// <summary>
        /// Fetches a list of forum responses.
        /// </summary>
        /// <returns>The list of forum responses</returns>
        // List<ForumJson> GetForums();

        /// <summary>
        /// Fetches a forum response for the abbreviated name.
        /// </summary>
        /// <param name="forumShortName">The abbreviated forum name</param>
        /// <returns>The forum response</returns>
        // ForumJson GetForum(string forumShortName);

        /// <summary>
        /// Fetches a list of topic responses for the abbreviated forum name.
        /// </summary>
        /// <param name="forumShortName">The abbreviated forum name</param>
        /// <param name="page">The page of topics to fetch</param>
        /// <returns>A page of topics for the forum</returns>
        // List<TopicJson> GetTopics(string forumShortName, int page);

        /// <summary>
        /// Fetches a topic response for the abbreviated forum name and topic slug.
        /// </summary>
        /// <param name="forumShortName">The abbreviated forum name</param>
        /// <param name="topicSlug">The topic slug</param>
        /// <returns>The topic response</returns>
        // TopicJson GetTopic(string forumShortName, string topicSlug);

        /// <summary>
        /// Fetches a list of post responses for the abbreviated forum name and topic slug.
        /// </summary>
        /// <param name="forumShortName">The abbreviated forum name</param>
        /// <param name="topicSlug">The topic slug</param>
        /// <param name="page">The page of posts to fetch</param>
        /// <returns>A page of posts for the topic</returns>
        // List<PostJson> GetPosts(string forumShortName, string topicSlug, int page);

        /// <summary>
        /// Fetches a post response for the abbreviated forum name, topic slug, and post ID.
        /// </summary>
        /// <param name="forumShortName">The abbreviated forum name</param>
        /// <param name="topicSlug">The topic slug</param>
        /// <param name="postId">The ID of the post to fetch</param>
        /// <returns>The post response</returns>
        // PostJson GetPost(string forumShortName, string topicSlug, int postId);
    }
}
