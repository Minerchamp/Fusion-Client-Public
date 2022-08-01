using FC.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace FusionClient.AviShit
{
    public class AviFavoritesObjects
    {
        // Module / Feature Objects

        public class ModAviFavorites
        {
            public class FavoriteList
            {
                public int ID { get; set; }
                public string name { get; set; }
                public string Desciption { get; set; }
                public int Rows { get; set; } = 2;
                public List<ModAviFav> Avatars { get; set; }
            }
            public List<FavoriteList> FavoriteLists = new();
        }

        public class ModAviFav
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string ThumbnailImageURL { get; set; }
        }

        public class ModRoomHistory
        {
            public string Name { get; set; }
            public string WorldID { get; set; }
            public string InstanceID { get; set; }
        }

        public class ApolloAvatar
        {
            public string _id { get; set; }
            public string AssetURL { get; set; }
            public string AuthorID { get; set; }
            public string AuthorName { get; set; }
            public string AvatarName { get; set; }
            public string Description { get; set; }
            public bool Featured { get; set; }
            public string ImageURL { get; set; }
            public string SupportedPlatforms { get; set; }
            public string ReleaseStatus { get; set; }
            public string ThumbnailImageURL { get; set; }
            public int Version { get; set; }
            public ApiAvatar ToApiAvatar()
            {
                return new ApiAvatar
                {
                    name = AvatarName,
                    id = _id,
                    authorId = AuthorID,
                    authorName = AuthorName,
                    assetUrl = AssetURL,
                    thumbnailImageUrl = ThumbnailImageURL,
                    supportedPlatforms = SupportedPlatforms == "All " ? ApiModel.SupportedPlatforms.All : ApiModel.SupportedPlatforms.StandaloneWindows,
                    description = Description,
                    releaseStatus = ReleaseStatus,
                    version = Version,
                    //unityVersion = unityVersion,
                    //assetVersion = new AssetVersion(unityVersion, 0),
                };
            }
        }
    }
}