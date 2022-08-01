using System.IO;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace FusionClient.Utils.Manager
{
    public static class AssetBundleManager
    {
        private static AssetBundle FusionBundle;
        public static Sprite Logo;
        public static Sprite LogoPanel;
        public static Sprite Discord;
        public static Sprite LogoCircle;
        public static Sprite LogoSquare;
        public static Sprite NamePlate;
        public static Sprite NamePlateIco;
        public static Sprite BrokenUspeakAW;
        public static Sprite ExploitsAW;
        public static Sprite FlyAW;
        public static Sprite FreeFallAW;
        public static Sprite ItemLockAW;
        public static Sprite MirrorSpamAW;
        public static Sprite RespawnAW;
        public static Sprite SerializationAW;
        public static Sprite SwastikaAW;

        public static void Initialize()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FusionClient.Resources.fusionclient"); //String is MainNamespace.assetbundlename
            using var tempStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(tempStream);
            FusionBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
            FusionBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            Logo = LoadSprite("Assets/FusionClient/logo.png");
            LogoPanel = LoadSprite("Assets/FusionClient/logoPanel.png");
            Discord = LoadSprite("Assets/FusionClient/discord.png");
            LogoCircle = LoadSprite("Assets/FusionClient/logoCircle.png");
            LogoSquare = LoadSprite("Assets/FusionClient/logoSquare.png");
            NamePlate = LoadSprite("Assets/FusionClient/capsule_background.png");
            NamePlateIco = LoadSprite("Assets/FusionClient/capsule_background2.png");
            BrokenUspeakAW = LoadSprite("Assets/FusionClient/AW/BrokenUspeak.png");
            ExploitsAW = LoadSprite("Assets/FusionClient/AW/Exploits.png");
            FlyAW = LoadSprite("Assets/FusionClient/AW/Fly.png");
            FreeFallAW = LoadSprite("Assets/FusionClient/AW/FreeFall.png");
            ItemLockAW = LoadSprite("Assets/FusionClient/AW/ItemLock.png");
            MirrorSpamAW = LoadSprite("Assets/FusionClient/AW/MirrorSpam.png");
            RespawnAW = LoadSprite("Assets/FusionClient/AW/Respawn.png");
            SerializationAW = LoadSprite("Assets/FusionClient/AW/Serialization.png");
            SwastikaAW = LoadSprite("Assets/FusionClient/AW/Swastika.png");
        }

        public static Font LoadFont(string file)
        {
            Font font2 = FusionBundle.LoadAsset(file, Il2CppType.Of<Font>()).Cast<Font>();
            font2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return font2;
        }

        public static Sprite LoadSprite(string file)
        {
            Sprite sprite2 = FusionBundle.LoadAsset(file, Il2CppType.Of<Sprite>()).Cast<Sprite>();
            sprite2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite2;
        }

        public static AudioClip LoadAudioClip(string file)
        {
            AudioClip AudioClip = FusionBundle.LoadAsset(file, Il2CppType.Of<AudioClip>()).Cast<AudioClip>();
            AudioClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return AudioClip;
        }

        public static Shader LoadShader(string file)
        {
            Shader shader = FusionBundle.LoadAsset(file, Il2CppType.Of<Shader>()).Cast<Shader>();
            shader.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return shader;
        }

        public static Texture2D LoadTexture(string file)
        {
            Texture2D texture2D = FusionBundle.LoadAsset(file, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
            texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return texture2D;
        }
    }
}
