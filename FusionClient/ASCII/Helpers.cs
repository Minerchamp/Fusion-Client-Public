using System.IO;
using System.Reflection;
using UnityEngine;
using FCConsole;
using FusionClient.ConsoleUtils;
using FusionClient.Ect;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace FusionClient.Ect
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public static class Helpers
    {
        public static Texture2D LoadPNG(string filePath)
        {
            
            Texture2D tex;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, fileData); //..this will auto-resize the texture dimensions.
                return tex;
            }
            else
            {
                ModConsole.Error($"Could not load: {filePath}");
                return null;
            }
        }

        internal async static void LoadSprite(Image Instance, string url)
        {
            await GetRemoteTexture(Instance, url);
        }

        private static async Task<Texture2D> GetRemoteTexture(Image Instance, string url)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            var asyncOp = www.SendWebRequest();
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                var Sprite = new Sprite();
                Sprite = Sprite.CreateSprite(DownloadHandlerTexture.GetContent(www), new Rect(0, 0, DownloadHandlerTexture.GetContent(www).width, DownloadHandlerTexture.GetContent(www).height), Vector2.zero, 100 * 1000, 1000, SpriteMeshType.FullRect, Vector4.zero, false);
                Instance.sprite = Sprite;
                Instance.color = Color.white;
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}