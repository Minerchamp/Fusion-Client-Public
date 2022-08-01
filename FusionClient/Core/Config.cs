using FusionClient.AviShit;
using FusionClient.Utils;
using FusionClient.Utils.Manager;
using FusionClient.Utils.Objects.Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FusionClient.Core
{
    public static class Config
    {
        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
        public static MainConfig Main => MainConfig.Instance;
        public static RoomHistoryConfig RoomHistory => RoomHistoryConfig.Instance;
        public static AviConfig AvatarFavorites => AviConfig.Instance;
        public static AviIDBlackListConfig IdBlackList => AviIDBlackListConfig.Instance;

        public static void Initialize()
        {
            MainConfig.Load();
            RoomHistoryConfig.Load();
            AviConfig.Load();
            AviIDBlackListConfig.Load();
        }

        public static void SaveAll()
        {
            try
            {
                Main.Save();
                RoomHistory.Save();
                AvatarFavorites.Save();
                IdBlackList.Save();
                Logs.Log($"Configs Saved!", ConsoleColor.Yellow);
            }
            catch (Exception ex)
            {
                Logs.Error("Config Saving", ex);
            }
        }

        public static IEnumerator SaveConfigLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(15);
                try
                {
                    Main.Save();
                    RoomHistory.Save();
                    AvatarFavorites.Save();
                    IdBlackList.Save();
                }
                catch { }
            }
        }

    }

    [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public class MainConfig
    {
        public string UILabel = "---------------------=[𝐔𝐈]=---------------------";
        public bool NoConsoleClear = false;
        public bool ExpirePanel = true;
        public bool CustomLoading = true;
        public bool CustomUiColor = true;
        public bool CustomUi = true;
        public bool UseActionMenu = true;
        public string UIColorHex = "#0056FF";
        public string UITextColorHex = "#0056ff";
        public string MenuPicture = "https://i.imgur.com/JeGqWwK.png";

        public string SelfLabel = "---------------------=[𝐒𝐞𝐥𝐟]=---------------------";
        public bool FakePing = false;
        public bool FakePingReal = false;
        public bool FakeFramesToggle = false;
        public bool FakeFramesReal = false;
        public bool ComfyVR = false;
        public bool MinimalnClipping = false;
        public bool PortalPrompt = true;
        public bool FpsUncap = false;
        public bool OfflineSpoof = false;
        public bool WorldSpoof = false;

        public string MovementLabel = "---------------------=[𝐌𝐨𝐯𝐞𝐦𝐞𝐧𝐭]=---------------------";
        public bool Flight = false;
        public bool InfJump = false;
        public bool BunnyHop = false;
        public bool MouseTP = false;
        public bool KeyBinds = false;
        public float FlightSpeed = 6f;
        public float SpeedHack = 16f;

        public string WorldtLabel = "---------------------=[𝐖𝐨𝐫𝐥𝐝]=---------------------";
        public bool AntiSeats = true;
        public bool PostProcessing = true;
        public bool NoPickups = true;
        public bool OptMirrors = false;
        public bool VideoPlayer = true;
        public bool WorldPortals = true;
        public bool AutoPortalDelete = false;
        public bool PortalTimeReset = false;

        public string ProtectiontLabel = "---------------------=[𝐏𝐫𝐨𝐭𝐞𝐜𝐭𝐢𝐨𝐧]=---------------------";
        public bool HWIDSpoofing = true;
        public bool AntiItemOrbit = false;
        public bool AntiDesync = false;
        public bool AntiWorldTriggers = false;
        public bool AntiLockInstance = true;
        public bool RPCBlock = false;
        public bool AntiMasterDc = false;
        public bool AntiUdon = false;
        public bool AntiCamCrash = true;
        public bool EventLimit = true;
        public bool AntiWarns = true;
        public bool AntiMicOff = true;
        public bool AntiPhotonBots = true;
        public bool AntiIKCrash = true;
        public bool AntiPortal = false;
        public bool AntiBrokenUspeak = true;
        public bool VideoPlayerProtection = true;
        public bool AntiRPCCrash = true;

        public string ExploitstLabel = "---------------------=[𝐄𝐱𝐩𝐥𝐨𝐢𝐭𝐬]=---------------------";
        public string VideoPlayerURL = "http://vrcmods.xyz/assets/YourTOXXXXIICCCCC/EarRape.mp4";
        public string QuestCrashID = "avtr_7cefbef6-c099-4df6-8564-7c8e09e845cf";
        public string AviClapID = "avtr_1b70bfbc-1ff2-4a4b-9811-9f381b992b7a";
        public float ItemOrbitSize = 0.5f;
        public float ItemOrbitSpeed = 1f;
        public float ItemOrbitUpDown = 1;
        public bool WorldTriggers = false;
        public bool InstanceLock = false;
        public bool InfPortals = false;
        public bool InvisibleJoin = false;

        public string ValuesLabel = "---------------------=[𝐕𝐚𝐥𝐮𝐞𝐬]=---------------------";
        public int FakePingValue = -69;
        public int FakeFramesValue = 69;
        public float AviDistinceHider = 7f;
        public string HWID = "";
        public string WorldID = "wrld_123b7fea-0ae2-4c7f-9f5e-28b63930fb0c";

        public string SettingsLabel = "---------------------=[𝐒𝐞𝐭𝐭𝐢𝐧𝐠𝐬]=---------------------";
        public bool AvatarLogging = false;
        public bool WorldLogging = false;
        public bool PlayerLog = true;
        public bool HudPlayerJoin = false;
        public bool HudPlayerJoinFriends = false;
        public bool HudPlayerLeave = false;
        public bool HudPlayerLeaveFriends = false;
        public bool HudModJoin = true;
        public bool PopUpModJoin = true;
        public bool HudWarns = true;
        public bool HudMicOff = true;
        public bool HudVoteKicks = true;
        public bool HudBlocks = true;
        public bool AntiBlock = false;
        public bool HudMutes = true;
        public bool HudUnmutes = true;
        public bool PlayerList = true;
        public bool DebugPanel = true;
        public bool NamePlates = true;
        public bool CustomNamePlates = true;
        public bool CustomNamePlateTags = true;
        public bool AutoUpdateCrasher = true;

        public string AntiCrashLabel = "---------------------=[𝐀𝐧𝐭𝐢𝐂𝐫𝐚𝐬𝐡]=---------------------";
        public bool ScanSelf = false;
        public bool ScanFriends = false;
        public bool ScanOthers = false;
        public bool ScanOnlyPublics = false;
        public bool ProcessTransforms = true;
        public int MaxTransformRotation = 1000;
        public int MaxTransformScale = 500;
        public bool ProcessRigidBodies = true;
        public bool ProcessColliders = true;
        public bool ProcessJoints = true;
        public bool ProcessAudioSources = true;
        public int MaxAudioSources = 25;
        public bool AntiWorldAudio = true;
        public int MaxAudioDistance = 10;

        public string BotLabel = "---------------------=[𝗕𝗼𝘁𝘀]=---------------------";
        public bool DebugMode = false;
        public bool AutoJoin = true;
        public bool LogBotEvents = true;

        public string ModulesLabel = "---------------------=[𝐌𝐨𝐝𝐮𝐥𝐞𝐬]=---------------------";
        public bool RPCLog = false;
        public bool AutoStartBotManagerServer = true;
        public bool DiscordRPC = true;
        public bool PhotonLog = false;
        public bool LogEvents = false;
        public bool AvatarHider = false;

        public static MainConfig Instance;
        public static void Load()
        {
            if (File.Exists(ModFiles.ConfigFile))
            {
                if (File.ReadAllText(ModFiles.ConfigFile).Equals(null))
                {
                    Logs.Log("Sorry But Your Config Was Null It Has Been Reset So The Client Works Right");
                    JsonManager.WriteToJsonFile(ModFiles.ConfigFile, new MainConfig());
                }
            }
            if (File.Exists(ModFiles.ConfigFile))
            {
                if (File.ReadAllText(ModFiles.ConfigFile).Equals(""))
                {
                    Logs.Log("Sorry But Your Config Was Null It Has Been Reset So The Client Works Right");
                    JsonManager.WriteToJsonFile(ModFiles.ConfigFile, new MainConfig());
                }
            }
            if (!File.Exists(ModFiles.ConfigFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.ConfigFile, new MainConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<MainConfig>(ModFiles.ConfigFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.ConfigFile, Instance);
        }
    }

    public class RoomHistoryConfig
    {
        public List<RoomHistoryObject> list = new();

        public static RoomHistoryConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.RoomHistoryFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.RoomHistoryFile, new RoomHistoryConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<RoomHistoryConfig>(ModFiles.RoomHistoryFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.RoomHistoryFile, Instance);
        }
    }

    public class AviIDBlackListConfig
    {
        public List<string> list = new();

        public static AviIDBlackListConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.IDBlackListFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.IDBlackListFile, new AviIDBlackListConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<AviIDBlackListConfig>(ModFiles.IDBlackListFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.IDBlackListFile, Instance);
        }
    }

    public class AviConfig
    {
        public AviFavoritesObjects.ModAviFavorites list = new AviFavoritesObjects.ModAviFavorites();
        /* Methods */
        public static AviConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.AviFavFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.AviFavFile, new AviConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<AviConfig>(ModFiles.AviFavFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.AviFavFile, Instance);
        }
    }
}
