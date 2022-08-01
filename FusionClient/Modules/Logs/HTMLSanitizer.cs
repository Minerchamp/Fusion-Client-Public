
namespace FusionClient.Modules
{
	internal static class HTMLSanitizer
	{
		internal static string Sanitize(this string htmlshit)
		{
			if (string.IsNullOrEmpty(htmlshit))
			{
				return "";
			}
			string text = "&^$#@!()+-,:;<>’'-_*/\\";
			string text2 = htmlshit;
			string text3 = text;
			for (int i = 0; i < text3.Length; i++)
			{
				text2 = text2.Replace(text3[i].ToString(), string.Empty);
			}
			return text2;
		}
	}
}
