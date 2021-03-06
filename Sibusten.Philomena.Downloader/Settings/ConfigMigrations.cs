using Sibusten.Philomena.Api;
using Sibusten.Philomena.Downloader.Settings.Legacy;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class ConfigMigrations
    {
        public static SearchConfig LegacyPresetToSearchConfig(LegacyPreset legacyPreset)
        {
            int filter;
            if (legacyPreset.UseCustomFilter)
            {
                filter = legacyPreset.CustomFilterId;
            }
            else
            {
                filter = legacyPreset.Filter switch
                {
                    LegacyPresetFilterIndex.Everything => 56027,
                    LegacyPresetFilterIndex.EighteenPlusR34 => 37432,
                    LegacyPresetFilterIndex.EighteenPlusDark => 37429,
                    LegacyPresetFilterIndex.Default => 100073,
                    LegacyPresetFilterIndex.MaximumSpoilers => 37430,
                    LegacyPresetFilterIndex.LegacyDefault => 37431,

                    // Use no filter for any other values, or for UserDefault
                    _ => SearchConfig.NoFilter
                };
            }

            SortDirection sortDirection = legacyPreset.SearchDirection switch
            {
                LegacySearchDirectionIndex.Descending => SortDirection.Descending,
                LegacySearchDirectionIndex.Ascending => SortDirection.Ascending,

                _ => SortDirection.Descending
            };

            SortField sortField = legacyPreset.SearchFormat switch
            {
                LegacySearchFormatIndex.CreationDate => SortField.ImageId,
                LegacySearchFormatIndex.Score => SortField.Score,
                LegacySearchFormatIndex.Relevance => SortField.Relevance,
                LegacySearchFormatIndex.Width => SortField.Width,
                LegacySearchFormatIndex.Height => SortField.Height,
                LegacySearchFormatIndex.Comments => SortField.Comments,
                LegacySearchFormatIndex.Random => SortField.Random,

                _ => SortField.ImageId
            };

            return new SearchConfig
            {
                Filter = filter,
                ImageLimit = legacyPreset.LimitImages ? legacyPreset.ImageLimit : SearchConfig.NoLimit,
                ImagePathFormat = legacyPreset.ImagePathFormat,
                JsonPathFormat = legacyPreset.JsonPathFormat,
                Query = legacyPreset.Query,
                ShouldSaveComments = legacyPreset.JsonComments,
                ShouldSaveFavorites = legacyPreset.JsonFavorites,
                ShouldSaveImages = !legacyPreset.JsonOnly,
                ShouldSaveJson = legacyPreset.SaveJson,
                ShouldUpdateJson = legacyPreset.UpdateJson,
                SortDirection = sortDirection,
                SortField = sortField
                // TODO: Add SVG action
            };
        }
    }
}
