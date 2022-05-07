using System;
using System.IO;
using Sibusten.Philomena.Api.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Client.Images
{
    public class PhilomenaImage : IPhilomenaImage
    {
        public ImageModel Model { get; private init; }
        private readonly int _id;

        public bool IsSvgImage => Model.Format == "svg";

        public PhilomenaImage(ImageModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (model.Id is null)
            {
                throw new ArgumentNullException("Image is missing an ID", nameof(model.Id));
            }

            Model = model;
            _id = model.Id.Value;
        }

        public string RawMetadata => JsonConvert.SerializeObject(Model);

        public int Id => _id;

        public string? Name
        {
            get
            {
                if (Model.ViewUrl is null)
                {
                    return null;
                }

                string localPath = new Uri(Model.ViewUrl).LocalPath;
                return Path.GetFileNameWithoutExtension(localPath);
            }
        }

        public string? OriginalName
        {
            get
            {
                if (Model.Name is null)
                {
                    return null;
                }

                return Path.GetFileNameWithoutExtension(Model.Name);
            }
        }

        public string? ShortViewUrl => Model.Representations?.Full;

        public string? ShortSvgViewUrl
        {
            get
            {
                if (ShortViewUrl is null)
                {
                    return null;
                }

                // Modify the URL to point to the SVG image
                string urlWithoutExtension = ShortViewUrl.Substring(0, ShortViewUrl.LastIndexOf('.'));
                return urlWithoutExtension + ".svg";
            }
        }

        public string? ViewUrl => Model.ViewUrl;

        public string? SvgViewUrl
        {
            get
            {
                if (ViewUrl is null)
                {
                    return null;
                }

                // Modify the URL to point to the SVG image
                string urlWithoutExtension = ViewUrl.Substring(0, ViewUrl.LastIndexOf('.'));
                return urlWithoutExtension + ".svg";
            }
        }

        public string? Format
        {
            get
            {
                if (IsSvgImage)
                {
                    // The image is an SVG image, which has two possible formats
                    // Assume rasters are always png, and return that as the format since rasters are what is presented to the booru user
                    return "png";
                }

                return Model.Format;
            }
        }

        public int? FileSize => Model.Size;
        public string? Hash => Model.Sha512Hash;
        public string? OriginalHash => Model.OrigSha512Hash;
        public List<string> TagNames => Model.Tags?.ToList() ?? new List<string>();  // .ToList to prevent editing the original model list
        public List<int> TagIds => Model.TagIds?.ToList() ?? new List<int>();  // .ToList to prevent editing the original model list
        public int? Score => Model.Score;
        public string? SourceUrl => Model.SourceUrl;
        public bool? IsSpoilered => Model.IsSpoilered;
        public int? TagCount => Model.TagCount;
        public bool? ThumbnailsGenerated => Model.ThumbnailsGenerated;
        public DateTime? UpdatedAt => Model.UpdatedAt;
        public string? Uploader => Model.Uploader;
        public int? UploaderId => Model.UploaderId;
        public int? Upvotes => Model.Upvotes;
        public bool? Processed => Model.Processed;
        public string? MimeType => Model.MimeType;
        public bool? IsAnimated => Model.IsAnimated;
        public double? AspectRatio => Model.AspectRatio;
        public int? CommentCount => Model.CommentCount;
        public DateTime? CreatedAt => Model.CreatedAt;
        public string? DeletionReason => Model.DeletionReason;
        public string? Description => Model.Description;
        public int? Downvotes => Model.Downvotes;
        public int? Width => Model.Width;
        public int? DuplicateOf => Model.DuplicateOf;
        public int? Faves => Model.Faves;
        public DateTime? FirstSeenAt => Model.FirstSeenAt;
        public int? Height => Model.Height;
        public bool? IsHiddenFromUsers => Model.IsHiddenFromUsers;
        public double? Duration => Model.Duration;
        public double? WilsonScore => Model.WilsonScore;
    }
}
