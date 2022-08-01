using System;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using VRC.Core;

namespace FusionClient.Modules
{
	public class AvatarObject
	{
		public string name { get; set; }

		public string id { get; set; }

		public string authorId { get; set; }

		public string authorName { get; set; }

		public string assetUrl { get; set; }

		public string thumbnailImageUrl { get; set; }

		public string supportedPlatforms { get; set; }

		public string releaseStatus { get; set; }

		public string description { get; set; }

		public int version { get; set; }

		public ApiAvatar ToApiAvatar()
		{
			return new ApiAvatar
			{
				name = this.name,
				id = this.id,
				authorId = this.authorId,
				authorName = this.authorName,
				assetUrl = this.assetUrl,
				thumbnailImageUrl = this.thumbnailImageUrl,
				supportedPlatforms = ((ApiModel.SupportedPlatforms)((this.supportedPlatforms == "All ") ? 3 : 1)),
				description = this.description,
				releaseStatus = this.releaseStatus,
				version = this.version
			};
		}

		public static ApiAvatar ApiAvatar(AvatarObject avatar)
		{
			return new ApiAvatar
			{
				name = avatar.name,
				id = avatar.id,
				authorId = avatar.authorId,
				authorName = avatar.authorName,
				assetUrl = avatar.assetUrl,
				thumbnailImageUrl = avatar.thumbnailImageUrl,
				supportedPlatforms = ((ApiModel.SupportedPlatforms)((avatar.supportedPlatforms == "All") ? 3 : 1)),
				description = avatar.description,
				releaseStatus = avatar.releaseStatus,
				version = avatar.version
			};
		}

		public AvatarObject(ApiAvatar avtr)
		{
			this.name = avtr.name;
			this.id = avtr.id;
			this.authorId = avtr.authorId;
			this.authorName = avtr.authorName;
			this.assetUrl = avtr.assetUrl;
			this.thumbnailImageUrl = avtr.thumbnailImageUrl;
			this.supportedPlatforms = avtr.supportedPlatforms.ToString();
			this.description = avtr.description;
			this.releaseStatus = avtr.releaseStatus;
			this.version = avtr.version;
		}

		public AvatarObject(Dictionary<string, UnityEngine.Object> avtr)
		{
			this.name = avtr["name"].ToString();
			this.id = avtr["id"].ToString();
			this.authorId = avtr["authorId"].ToString();
			this.authorName = avtr["authorName"].ToString();
			this.assetUrl = avtr["assetUrl"].ToString();
			this.thumbnailImageUrl = avtr["thumbnailImageUrl"].ToString();
			this.supportedPlatforms = "All";
			this.description = avtr["description"].ToString();
			this.releaseStatus = avtr["releaseStatus"].ToString();
		}

		public AvatarObject()
		{
		}
	}
}
