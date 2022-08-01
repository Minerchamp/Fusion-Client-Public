using FC;
using FC.Utils;
using FusionClient.Core;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using static FC.Utils.DiscordUtils;

namespace FusionClient.Modules.Discord
{
    class DiscordHelper : FusionModule
    {
        public static string rcpid = "998832222939402260";
        public static string MainLogoSlogan = "Made By Viper";
        public static string statelol = "FusionClient We Spark Our Opponent";
        public static string SmallLogoSlogan = "https://discord.gg/CqpJwHPYJs";
        private static RichPresence Presence;
        private static EventHandlers Handlers;
        public static void StartRPC()
        {
            if (Config.Main.DiscordRPC)
            {
                Logs.Log("[Discord] Starting Rich Presence...", ConsoleColor.Yellow);
                Handlers = default;
                Initialize(rcpid, ref Handlers, false, string.Empty); // Tells DiscordUtils to do its job
                Presence.details = statelol;
                Presence.state = "Logging into FusionClient.";
                Presence.startTimestamp = default(long);
                Presence.largeImageKey = "https://i.imgur.com/bcCplUF.gif";
                Presence.largeImageText = MainLogoSlogan;
                Presence.smallImageKey = "https://i.imgur.com/qx5vJYq.gif";
                Presence.smallImageText = SmallLogoSlogan;
                Presence.joinSecret = "https://discord.gg/CqpJwHPYJs";
                Presence.partySize = 1;
                Presence.partyMax = 1;
                UpdatePresence(ref Presence);
                MelonCoroutines.Start(WaitForWorld());
            }
            else
            {
                ClearPresence();
            }
        }

        public static void StopRPC()
        {
            if (!Config.Main.DiscordRPC)
            {
                ClearPresence();
            }
        }

        public override void Start()
        {
            if (Config.Main.DiscordRPC)
            {
                StartRPC();
            }
        }

        private static IEnumerator WaitForWorld()
        {
            while (!WorldUtils.IsInWorld()) yield return null;
            for (; ; )
            {
                try
                {
                    if (Config.Main.DiscordRPC)
                    {
                        Presence.partySize = WorldUtils.GetPlayerCount();
                        Presence.partyMax = WorldUtils.GetCurrentWorld().capacity;
                        switch (WorldUtils.GetCurrentInstance().type)
                        {
                            default:
                                Presence.state = "Switching Worlds...";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                Presence.partySize = 0;
                                Presence.partyMax = 0;
                                break;

                            case VRC.Core.InstanceAccessType.Public:
                                Presence.state = $"[Public] {WorldUtils.GetCurrentWorld().name}";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                break;

                            case VRC.Core.InstanceAccessType.FriendsOfGuests:
                                Presence.state = $"[Friends+] {WorldUtils.GetCurrentWorld().name}";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                break;

                            case VRC.Core.InstanceAccessType.FriendsOnly:
                                Presence.state = $"[Friends] {WorldUtils.GetCurrentWorld().name}";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                break;

                            case VRC.Core.InstanceAccessType.InviteOnly:
                                Presence.state = $"[Private] {WorldUtils.GetCurrentWorld().name}";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                break;

                            case VRC.Core.InstanceAccessType.InvitePlus:
                                Presence.state = $"[Invite+] {WorldUtils.GetCurrentWorld().name}";
                                //Presence.largeImageKey = Presence.largeImageKey;
                                break;

                        }
                        UpdatePresence(ref Presence);
                    }
                }
                catch { }
                yield return new WaitForSeconds(5f);
            }
        }
    }
}