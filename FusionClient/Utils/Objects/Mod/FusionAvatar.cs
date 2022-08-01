using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;

namespace FusionClient.Utils.Objects.Mod
{
    public class FusionAvatar
    {
        public string AvatarID { get; set; }
        public string AssetURL { get; set; }
        public string AuthorID { get; set; }
        public string AuthorName { get; set; }
        public string AvatarName { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ReleaseStatus { get; set; }
        public string ThumbnailImageURL { get; set; }
        public int Version { get; set; }

        public class AviSearchResults
        {
            public List<FusionAvatar> results { get; set; }
        }

        public static FusionAvatar ToFusionAvatar(ApiAvatar a)
        {
            return new FusionAvatar()
            {
                AssetURL = a.assetUrl,
                AuthorID = a.authorId,
                AuthorName = a.authorName,
                AvatarID = a.id,
                AvatarName = a.name,
                Description = a.description,
                ImageURL = a.imageUrl,
                ReleaseStatus = a.releaseStatus,
                ThumbnailImageURL = a.thumbnailImageUrl,
                Version = a.version,
            };
        }

        public static ApiAvatar ToApiAvatar(FusionAvatar a)
        {
            return new ApiAvatar()
            {
                assetUrl = a.AssetURL,
                authorId = a.AuthorID,
                authorName = a.AuthorName,
                id = a.AvatarID,
                name = a.AvatarName,
                description = a.Description,
                imageUrl = a.ImageURL,
                releaseStatus = a.ReleaseStatus,
                thumbnailImageUrl = a.ThumbnailImageURL,
                version = a.Version
            };
        }
    }
}
