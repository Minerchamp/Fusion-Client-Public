using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FusionClient.AviShit;
using VRC.Core;

namespace FC.Utils
{
    internal static class NetworkUtils
    {
        public static string Convert(WebResponse res)
        {
            string strResponse = "";
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                strResponse = reader.ReadToEnd();
            }

            res.Dispose();
            return strResponse;
        }

        public static string[] Array(WebResponse res)
        {
            return Convert(res).Split(Environment.NewLine.ToCharArray());
        }

        internal static ApiAvatar ToApiAvatar(this VRCAvatar a)
        {
            return new ApiAvatar
            {
                id = a.id,
                name = a.name,
                description = a.description,
                authorId = a.authorId,
                authorName = a.authorName,
                assetUrl = a.assetUrl,
                imageUrl = a.imageUrl,
                thumbnailImageUrl = a.thumbnailImageUrl,
                releaseStatus = a.releaseStatus,
                version = a.version,
                featured = a.featured,
                created_at = Il2CppSystem.DateTime.Parse(a.created_at.ToString(CultureInfo.CurrentCulture)),
                updated_at = Il2CppSystem.DateTime.Parse(a.updated_at.ToString(CultureInfo.CurrentCulture))
            };
        }

        internal static ApiAvatar ToApiAvatar(this AviFavoritesObjects.ModAviFav a)
        {
            return new ApiAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        internal static VRCAvatar ToVRCAvatar(this ApiAvatar a)
        {
            return new VRCAvatar
            {
                id = a.id,
                name = a.name,
                description = a.description,
                authorId = a.authorId,
                authorName = a.authorName,
                assetUrl = a.assetUrl,
                imageUrl = a.imageUrl,
                thumbnailImageUrl = a.thumbnailImageUrl,
                releaseStatus = a.releaseStatus,
                version = a.version,
                featured = a.featured,
                created_at = DateTime.Parse(a.created_at.ToString()),
                updated_at = DateTime.Parse(a.updated_at.ToString())
            };
        }

        internal static VRCAvatar ToVRCAvatar(this AviFavoritesObjects.ModAviFav a)
        {
            return new VRCAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        internal static AviFavoritesObjects.ModAviFav ToModFavAvi(this ApiAvatar a)
        {
            return new AviFavoritesObjects.ModAviFav
            {
                Name = a.name,
                ID = a.id,
                ThumbnailImageURL = a.thumbnailImageUrl
            };
        }
    }
}
