namespace FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers;

using System.Collections.Generic;
using FC;
using Il2CppSystem;
using Structs;
using Structs.ModerationStructures;
using UnhollowerBaseLib;
using FusionClient.Tools.UdonEditor;
using FusionClient.Extensions;
using Exception = System.Exception;
using FusionClient.Moderation;
using FC.Utils;
using FusionClient.Core;

internal class Photon_PlayerModerationHandler
{
    internal static List<string> BlockedYouPlayers { get; } = new();
    internal static List<string> MutedYouPlayers { get; } = new();

    internal static HookAction HandleEvent(ref Dictionary<byte, Object> Dictionary)
    {
        try
        {
            HookAction result = HookAction.Nothing;
            if (Dictionary.ContainsKey(ModerationCode.EventCode))
            {
                var moderationevent = Dictionary[ModerationCode.EventCode].Unbox<byte>();
                switch (moderationevent)
                {
                    case ModerationCode.Warning: // Warnings.

                        if (Dictionary.ContainsKey(PlayerModerationCode.Mod_Warning))
                        {
                            string Message = Dictionary[2].ToString();
                            if (Message.Contains("Unable to start a vote to kick"))
                            {
                                Logs.Hud("<color=cyan>FusionClient</color> | Failed at Votekick");
                            }
                            else if (Message.Contains("You have been warned for your behavior. If you continue, you may be kicked out of the instance"))
                            {
                                if (Config.Main.HudWarns)
                                {
                                    Logs.Hud($"<color=cyan>FusionClient</color> | {WorldUtils.GetInstanceMaster().GetDisplayName()} Warned You");
                                    Logs.Debug($"{WorldUtils.GetInstanceMaster().GetDisplayName()} Warned You");
                                }
                                if (Config.Main.AntiWarns)
                                {
                                    result = HookAction.Block;
                                }
                            }
                        }

                        break;

                    case ModerationCode.Mod_Mute: // Mod Mute

                        if (Config.Main.HudMicOff)
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | {WorldUtils.GetInstanceMaster().GetDisplayName()} Is Trying To Mic-Off You");
                            Logs.Debug($"{WorldUtils.GetInstanceMaster().GetDisplayName()} Is Trying To Mic-Off You");
                        }
                        if (Config.Main.AntiMicOff)
                        {
                            result = HookAction.Block;
                        }

                        break;

                    case ModerationCode.Friend_State: // Friend State
                        break;

                    case ModerationCode.VoteKick: // VoteKick
                        Logs.Hud($"<color=cyan>FusionClient</color> | A Votekick Has Been Started If You Dont See It It's For You");
                        break;

                    case ModerationCode.Block_Or_Mute:

                        #region Blocking and Muting Events.

                        // Single Moderation Event (one player)
                        if (Dictionary.Count == 4)
                        {
                            if (Dictionary.ContainsKey(PlayerModerationCode.ActorID))
                            {
                                var RemoteModerationPhotonID = Dictionary[PlayerModerationCode.ActorID].Unbox<int>();
                                var PhotonPlayer = PlayerUtils.LoadBalancingPeer.GetPhotonPlayer(RemoteModerationPhotonID);
                                if (Dictionary.ContainsKey(PlayerModerationCode.Block))
                                {
                                    var blocked = Dictionary[PlayerModerationCode.Block].Unbox<bool>();
                                    switch (blocked)
                                    {
                                        case true:
                                            {
                                                if (Config.Main.HudBlocks)
                                                {
                                                    PhotonModerationHandler.OnPlayerBlockedYou_Invoker(PhotonPlayer);
                                                }
                                                if (Config.Main.AntiBlock)
                                                {
                                                    Dictionary[PlayerModerationCode.Block] = Il2CppConverter.Generate_Il2CPPObject(false);
                                                    result = HookAction.Patch;
                                                }
                                                break;
                                            }
                                        case false:
                                            {
                                                if (Config.Main.HudBlocks)
                                                {
                                                    PhotonModerationHandler.OnPlayerUnblockedYou_Invoker(PhotonPlayer);
                                                }
                                                break;
                                            }
                                    }
                                }

                                if (Dictionary.ContainsKey(PlayerModerationCode.Mute))
                                {
                                    var muted = Dictionary[PlayerModerationCode.Mute].Unbox<bool>();
                                    switch (muted)
                                    {
                                        case true:
                                            {
                                                if (Config.Main.HudMutes)
                                                {
                                                    PhotonModerationHandler.OnPlayerMutedYou_Invoker(PhotonPlayer);
                                                }
                                                break;
                                            }
                                        case false:
                                            {
                                                if (Config.Main.HudMutes)
                                                {
                                                    PhotonModerationHandler.OnPlayerUnmutedYou_Invoker(PhotonPlayer);
                                                }
                                                break;
                                            }
                                    }
                                }
                            }
                        }

                        // Multiple Moderation Events (Usually happens when you enter the room)
                        else if (Dictionary.Count == 3)
                        {
                            // Blocked List
                            if (Dictionary.ContainsKey(PlayerModerationCode.Block))
                            {
                                var blockedlistObject = Dictionary[PlayerModerationCode.Block];
                                if (blockedlistObject != null)
                                {
                                    var BlockedPlayersArray = blockedlistObject.Cast<Il2CppStructArray<int>>();
                                    if (BlockedPlayersArray != null)
                                        if (BlockedPlayersArray.Count != 0)
                                        {
                                            for (var i = 0; i < BlockedPlayersArray.Count; i++)
                                            {
                                                var blockedplayers = PlayerUtils.LoadBalancingPeer.GetPhotonPlayer(BlockedPlayersArray[i]);
                                                PhotonModerationHandler.OnPlayerBlockedYou_Invoker(blockedplayers);
                                                BlockedPlayersArray[i] = -1;
                                            }

                                            Dictionary[PlayerModerationCode.Block] = new Object(BlockedPlayersArray.Pointer);
                                            result = HookAction.Patch;
                                        }
                                }
                            }

                            // Muted List
                            if (Dictionary.ContainsKey(PlayerModerationCode.Mute))
                            {
                                var MutedlistObject = Dictionary[PlayerModerationCode.Mute];
                                if (MutedlistObject != null)
                                {
                                    var MutePlayersArray = MutedlistObject.Cast<Il2CppStructArray<int>>();
                                    if (MutePlayersArray != null)
                                        if (MutePlayersArray.Count != ModerationCode.EventCode)
                                            for (var i = 0; i < MutePlayersArray.Count; i++)
                                            {
                                                var MutePlayer = PlayerUtils.LoadBalancingPeer.GetPhotonPlayer(MutePlayersArray[i]);
                                                PhotonModerationHandler.OnPlayerMutedYou_Invoker(MutePlayer);
                                            }
                                }
                            }
                        }

                        #endregion Blocking and Muting Events.

                        break;
                }

                return result;
            }
        }
        catch
        {
        }

        return HookAction.Nothing;
    }
}