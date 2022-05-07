using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class UserModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The slug of the user.
        /// </summary>
        [JsonProperty("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// The role of the user.
        /// </summary>
        [JsonProperty("role")]
        public string? Role { get; set; }

        /// <summary>
        /// The description (bio) of the user.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The URL of the user's thumbnail. null if the avatar is not set.
        /// </summary>
        [JsonProperty("avatar_url")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// The creation time, in UTC, of the user.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The comment count of the user.
        /// </summary>
        [JsonProperty("comments_count")]
        public int? CommentsCount { get; set; }

        /// <summary>
        /// The upload count of the user.
        /// </summary>
        [JsonProperty("uploads_count")]
        public int? UploadsCount { get; set; }

        /// <summary>
        /// The forum posts count of the user.
        /// </summary>
        [JsonProperty("posts_count")]
        public int? PostsCount { get; set; }

        /// <summary>
        /// The forum topics count of the user.
        /// </summary>
        [JsonProperty("topics_count")]
        public int? TopicsCount { get; set; }

        /// <summary>
        /// The links the user has registered. See <see cref="LinkModel"/>.
        /// </summary>
        [JsonProperty("links")]
        public List<LinkModel>? Links { get; set; }

        /// <summary>
        /// The awards/badges of the user. See <see cref="AwardModel"/>.
        /// </summary>
        [JsonProperty("awards")]
        public List<AwardModel>? Awards { get; set; }
    }
}
