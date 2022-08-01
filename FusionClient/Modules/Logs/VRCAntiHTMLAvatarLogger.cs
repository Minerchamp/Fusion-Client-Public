using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VRC.Core;

namespace FusionClient.Modules
{
	public class HTMLAvatarLogger
	{
		internal static Dictionary<string, ApiAvatar> LoggedAvatars = new Dictionary<string, ApiAvatar>();
		internal static System.DateTime GameStartTime;

		internal static void LogAvatar(ApiAvatar avatarToLog)
		{
			LoggedAvatars[avatarToLog.id] = avatarToLog;
			string text = "<html>\r\n\t<head>\r\n\t\t<title>\r\n\t\t\tFusionClient Logged Avatars\r\n\t\t</title>\r\n\t\r\n\t\t<style>\r\n\t\t\t#InputBox\r\n\t\t\t{\r\n\t\t\t\tbackground-color: black;\r\n\t\t\t\tcolor: cyan;\r\n\t\t\t\toutline: none !important;\r\n\t\t\t\t\r\n\t\t\t\twidth: 100%;\r\n\t\t\t\tborder: 1px solid #0040ff;\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\tbody\r\n\t\t\t{\r\n\t\t\t\tmax-width: 1080px; \r\n\t\t\t\tmargin: auto auto !important;\r\n\t\t\t\tpadding: 25 25;\r\n\t\t\t\tfloat: none !important;\r\n\t\t\t\r\n\t\t\t\tbackground-color: black;\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\th1\r\n\t\t\t{\r\n\t\t\t\tcolor: white;\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\th2\r\n\t\t\t{\r\n\t\t\t\tcolor: white;\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\tp\r\n\t\t\t{\r\n\t\t\t\tcolor: white;\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\ta\r\n\t\t\t{\r\n\t\t\t\tcolor: white;\r\n\t\t\t}\r\n\t\t</style>\r\n\t\r\n\t\t<script>\r\n\t\t\tfunction SearchForAvatar()\r\n\t\t\t{\r\n\t\t\t\t//Declarations\r\n\t\t\t\tlet input, filter, classlist, textvalue, i, i2;\r\n\t\t\t\t\r\n\t\t\t\tinput = document.getElementById('InputBox');\r\n\t\t\t\tfilter = input.value.toLowerCase();\r\n\t\t\t\tclasslist = document.getElementsByClassName('AvatarInformation');\r\n\t\t\t\t\r\n\t\t\t\t//Search Function\r\n\t\t\t\tfor (i = 0; i < classlist.length; i++)\r\n\t\t\t\t{\r\n\t\t\t\t\tlet CurrentClass = classlist[i];\r\n\t\t\t\t\r\n\t\t\t\t\ttextvalue = classlist[i].textContent;\r\n\t\t\t\t\t\t\r\n\t\t\t\t\tif (textvalue.toLowerCase().indexOf(filter) < 0)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tCurrentClass.style.display = \"none\";\r\n\t\t\t\t\t}\r\n\t\t\t\t\telse\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tCurrentClass.style.display = \"initial\";\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t</script>\r\n\t</head>\r\n\r\n\t<body>\r\n\t\t<input type=\"text\" id=\"InputBox\" onkeyup=\"SearchForAvatar()\" placeholder=\"Search\">\r\n";
			foreach (ApiAvatar value in LoggedAvatars.Values)
			{
				text += "\r\n";
				text = text + $"<div class=\"AvatarInformation\">\r\n\t\t\t<h1>Avatar Name: {value.name.Sanitize()} | Last Used: {DateTime.Now}</h1>\r\n\r\n\t\t\t<h2>Information:</h2>\r\n\r\n\t\t\t<p>\r\n\t\t\t\tAvatar Author: <b><a href=\"https://vrchat.com/home/user/{value.authorId}\" target=\"_blank\">{value.authorName.Sanitize()}</a></b><br/>\r\n\t\t\t\tAvatar Version: <b>{value.version}</b><br/>\r\n\t\t\t\tAvatar Release Status: <b>{value.releaseStatus}</b><br/>\r\n\t\t\t\tAvatar Description: <b>{value.description.Sanitize()}</b><br/>\r\n\t\t\t\tAvatar Platform: <b>{value.platform}</b><br/>\r\n\t\t\t\tAvatar ID: <b><a href=\"https://vrchat.com/home/avatar/{value.id}\" target=\"_blank\">{value.id}</a></b><br/>\r\n" + "\t\t\t\tAvatar VRCA: <b><a href=\"" + value.assetUrl + "\" target=\"_blank\">Download</a></b><br/>\r\n" + " </p>\r\n\r\n\t\t\t<h2>Avatar Image:</h2>\r\n\r\n\t\t\t<img src=\"" + value.imageUrl + "\" width=\"500\" height=\"400\"/><br/><br/>\r\n\r\n\t\t\t<hr/>\r\n\t\t</div>\r\n";
			}
			text += "\t</body>\r\n</html>";
			if (!Directory.Exists(Environment.CurrentDirectory + "\\Fusion Client\\"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\Fusion Client\\");
			}
			if (!Directory.Exists(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\");
			}
            File.AppendAllText(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\AvatarLog.html", text);
        }

		internal static void LogAvatartxt(ApiAvatar avatarToLog)
		{
			LoggedAvatars[avatarToLog.id] = avatarToLog;
			string text = "";
			foreach (ApiAvatar value in LoggedAvatars.Values)
			{
				text = text + $"{value.name}:{value.id}:{value.authorName}\n";
			}
			if (!Directory.Exists(Environment.CurrentDirectory + "\\Fusion Client\\"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\Fusion Client\\");
			}
			if (!Directory.Exists(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\");
			}
			File.AppendAllText(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\AvatarLog.txt", text);
		}

	}
}
