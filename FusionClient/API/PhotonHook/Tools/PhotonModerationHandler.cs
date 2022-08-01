namespace FusionClient.Moderation
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FusionClient.AstroEventArgs;
    using FC;
    using FC.Utils;
    using FusionClient.Core;
    using MelonLoader;
    using Photon.Realtime;

    #endregion

    internal class PhotonModerationHandler
    {
        internal static void OnPlayerBlockedYou_Invoker(Player player)
        {
            if (player != null)
            {
                var photonuserid = player.field_Public_Player_0.field_Private_APIUser_0.id;
                var User = PhotonUtils.GetPlayer(player);

                if (!FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Contains(photonuserid))
                {
                    Logs.Hud($"<color=cyan>FusionClient</color> | {FC.Utils.PhotonUtils.GetDisplayName(player)} Blocked You");
                    Logs.Debug($"{FC.Utils.PhotonUtils.GetDisplayName(player)} Blocked You");
                    FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Add(photonuserid);
                    FusionClient.Modules.Nameplates.Disable(User);
                    FusionClient.Modules.Nameplates.Enable(User);
                }
            }
        }

        internal static void OnPlayerUnblockedYou_Invoker(Player player)
        {
            if (player != null)
            {
                var photonuserid = player.field_Public_Player_0.field_Private_APIUser_0.id;
                var User = PhotonUtils.GetPlayer(player);

                if (FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Contains(photonuserid))
                {
                    Logs.Hud($"<color=cyan>FusionClient</color> | {FC.Utils.PhotonUtils.GetDisplayName(player)} UnBlocked You");
                    Logs.Debug($"{FC.Utils.PhotonUtils.GetDisplayName(player)} UnBlocked You");
                    FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Remove(photonuserid);
                    FusionClient.Modules.Nameplates.Disable(User);
                    FusionClient.Modules.Nameplates.Enable(User);
                }
            }
        }

        internal static void OnPlayerMutedYou_Invoker(Player player)
        {
            if (player != null)
            {
                var photonuserid = player.field_Public_Player_0.field_Private_APIUser_0.id;

                if (!FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Contains(photonuserid))
                {
                    Logs.Hud($"<color=cyan>FusionClient</color> | {FC.Utils.PhotonUtils.GetDisplayName(player)} Muted You");
                    Logs.Debug($"{FC.Utils.PhotonUtils.GetDisplayName(player)} Muted You");
                    FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Add(photonuserid);
                }
            }
        }

        internal static void OnPlayerUnmutedYou_Invoker(Player player)
        {
            if (player != null)
            {
                var photonuserid = player.field_Public_Player_0.field_Private_APIUser_0.id;

                if (FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Contains(photonuserid))
                {
                    Logs.Hud($"<color=cyan>FusionClient</color> | {FC.Utils.PhotonUtils.GetDisplayName(player)} UnMuted You");
                    Logs.Debug($"{FC.Utils.PhotonUtils.GetDisplayName(player)} UnMuted You");
                    FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Remove(photonuserid);
                }
            }
        }

        #region EventHandlers

        internal static event EventHandler<PhotonPlayerEventArgs> Event_OnPlayerBlockedYou;
        internal static event EventHandler<PhotonPlayerEventArgs> Event_OnPlayerUnblockedYou;
        internal static event EventHandler<PhotonPlayerEventArgs> Event_OnPlayerMutedYou;
        internal static event EventHandler<PhotonPlayerEventArgs> Event_OnPlayerUnmutedYou;

        #endregion
    }
}