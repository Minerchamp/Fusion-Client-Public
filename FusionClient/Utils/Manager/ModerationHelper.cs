using FusionClient.Utils;
using System;
using System.Collections.Generic;
using System.IO;

// Credit to Dayoftheplay for this.

namespace FC.Utils
{
    internal class ModerationHelper
    {
        public class ModerationEntry
        {
            public string UserID { get; set; }
            public string Displayname { get; set; }
            public bool BlockedU { get; set; }
            public bool MutedU { get; set; }
            public bool UBlocked { get; set; }
            public bool UMuted { get; set; }
            public int WarnU { get; set; }
            public int MicOffU { get; set; }
            public int KickedU { get; set; }
        }

        public enum ModerationType
        {
            BlockU,
            MuteU,
            UBlocked,
            UMute,
            WarnU,
            MicOffU,
            KickU,
            None
        }

        public static List<ModerationEntry> Moderations = new List<ModerationEntry>();

        public static ModerationEntry GetModeration(string SourceID, string Display = null, bool GenNew = true)
        {
            
            var Moderation = Moderations.Find(moderation => moderation.UserID == SourceID);
            if (Moderation == null && GenNew)
            {
                Moderations.Add(new ModerationEntry() { UserID = SourceID, BlockedU = false, Displayname = Display, MutedU = false, UBlocked = false, UMuted = false, WarnU = 0, MicOffU = 0, KickedU = 0 });
                SaveModerations();
                return GetModeration(SourceID, Display, GenNew);
            }
            return Moderation;
        }

        public static ModerationType GetModerationType(string userid)
        {
            var Moderation = ModerationHelper.GetModeration(userid, null, false);
            if (Moderation != null)
            {
                if (Moderation.BlockedU)
                    return ModerationType.BlockU;
                if (Moderation.MutedU)
                    return ModerationType.MuteU;
            }
            return ModerationType.None;
        }

        public static void UpdateModeration(string userid, ModerationType type, bool Change, string Displayname = null)
        {
            var Moderation = GetModeration(userid, Displayname);
            if (Displayname != null)
                Moderation.Displayname = Displayname;
            switch (type)
            {
                case ModerationType.BlockU:
                    Moderation.BlockedU = Change;
                    break;

                case ModerationType.MuteU:
                    Moderation.MutedU = Change;
                    break;

                case ModerationType.UBlocked:
                    Moderation.UBlocked = Change;
                    break;

                case ModerationType.UMute:
                    Moderation.UMuted = Change;
                    break;

                case ModerationType.WarnU:
                    Moderation.WarnU += 1;
                    break;

                case ModerationType.MicOffU:
                    Moderation.MicOffU += 1;
                    break;

                case ModerationType.KickU:
                    Moderation.KickedU += 1;
                    break;
            }
            LogModeration(userid, type, Change, Displayname);
            SaveModerations();
        }

        public static void SaveModerations()
        {
            var SavePath = ModFiles.MiscPath + "\\Moderations.json";
            File.WriteAllText(SavePath, Newtonsoft.Json.JsonConvert.SerializeObject(Moderations, Newtonsoft.Json.Formatting.Indented));
        }

        public static void LoadModerations()
        {
            var SavePath = ModFiles.MiscPath + "\\Moderations.json";
            if (!File.Exists(SavePath))
                File.WriteAllText(SavePath, Newtonsoft.Json.JsonConvert.SerializeObject(Moderations, Newtonsoft.Json.Formatting.Indented));
            Moderations = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ModerationEntry>>(File.ReadAllText(SavePath));
        }

        public static void LogModeration(string userid, ModerationType type, object Change, string Displayname = null)
        {
            var LogPath = ModFiles.MiscPath + "\\ModerationsLog.Json";
            if (!File.Exists(LogPath))
                File.WriteAllText(LogPath, Newtonsoft.Json.JsonConvert.SerializeObject(new List<ModerationLogEntry>(), Newtonsoft.Json.Formatting.Indented));
            var Cache = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ModerationLogEntry>>(File.ReadAllText(LogPath));
            Cache.Insert(0, new ModerationLogEntry() { UserID = userid, Displayname = Displayname, type = type.ToString(), Change = Change, Time = DateTime.Now });
            File.WriteAllText(LogPath, Newtonsoft.Json.JsonConvert.SerializeObject(Cache, Newtonsoft.Json.Formatting.Indented));
        }

        public class ModerationLogEntry
        {
            public string UserID { get; set; }
            public string Displayname { get; set; }
            public string type { get; set; }
            public object Change { get; set; }
            public DateTime Time { get; set; }
        }
    }
}