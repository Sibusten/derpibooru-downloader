using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings.Legacy
{
    public enum LegacyPresetFilterIndex
    {
        UserDefault = 0,
        Everything = 1,
        EighteenPlusR34 = 2,
        EighteenPlusDark = 3,
        Default = 4,
        MaximumSpoilers = 5,
        LegacyDefault = 6
    }

    public enum LegacySearchFormatIndex
    {
        CreationDate = 0,
        Score = 1,
        Relevance = 2,
        Width = 3,
        Height = 4,
        Comments = 5,
        Random = 6
    }

    public enum LegacySearchDirectionIndex
    {
        Descending = 0,
        Ascending = 1
    }

    public enum LegacySvgActionIndex
    {
        SaveSvgOnly = 0,
        SavePngOnly = 1,
        SaveSvgAndPng = 2
    }

    /// <summary>
    /// A legacy preset model from Derpibooru Downloader v2.x
    /// </summary>
    public class LegacyPreset
    {
        [JsonProperty("query")]
        public string Query { get; set; } = "";

        [JsonProperty("startPage")]
        public int StartPage { get; set; } = 1;

        [JsonProperty("perPage")]
        public int PerPage { get; set; } = 50;

        [JsonProperty("imageLimit")]
        public int ImageLimit { get; set; } = 1;

        /// <summary>
        /// The filter *index* from the previous app. Only used if UseCustomFilter is false.
        /// </summary>
        [JsonProperty("filter")]
        public LegacyPresetFilterIndex Filter { get; set; } = LegacyPresetFilterIndex.UserDefault;

        [JsonProperty("useCustomFilter")]
        public bool UseCustomFilter { get; set; } = false;

        [JsonProperty("customFilterID")]
        public int CustomFilterId { get; set; } = 0;

        /// <summary>
        /// The search format *index* from the previous app.
        /// </summary>
        [JsonProperty("searchFormat")]
        public LegacySearchFormatIndex SearchFormat { get; set; } = LegacySearchFormatIndex.CreationDate;

        /// <summary>
        /// The search direction *index* from the previous app.
        /// </summary>
        [JsonProperty("searchDirection")]
        public LegacySearchDirectionIndex SearchDirection { get; set; } = LegacySearchDirectionIndex.Descending;

        [JsonProperty("imagePathFormat")]
        public string ImagePathFormat { get; set; } = "Downloads/{id}.{ext}";

        [JsonProperty("jsonPathFormat")]
        public string JsonPathFormat { get; set; } = "Json/{id}.json";

        [JsonProperty("saveJson")]
        public bool SaveJson { get; set; } = false;

        [JsonProperty("updateJson")]
        public bool UpdateJson { get; set; } = false;

        [JsonProperty("jsonOnly")]
        public bool JsonOnly { get; set; } = false;

        [JsonProperty("jsonComments")]
        public bool JsonComments { get; set; } = false;

        [JsonProperty("jsonFavorites")]
        public bool JsonFavorites { get; set; } = false;

        [JsonProperty("limitImages")]
        public bool LimitImages { get; set; } = false;

        /// <summary>
        /// The svg action *index* from the previous app
        /// </summary>
        [JsonProperty("svgAction")]
        public LegacySvgActionIndex SvgAction { get; set; } = LegacySvgActionIndex.SaveSvgOnly;
    }
}
