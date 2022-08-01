using System;
using System.Collections;
using UnityEngine;
using VRC;
using VRC.Core;
using FC;
using FC.Components;
using FC.Utils;
using System.Linq;
using FusionClient.Core;

namespace FusionClient.Modules
{
    internal class AviDistanceHide
    {
        public static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static GameObject GetAvatarObject(VRC.Player p) => p.prop_VRCPlayer_0.prop_VRCAvatarManager_0.prop_GameObject_0;
        public static bool IsMe(VRC.Player p) => p.name == GetLocalVRCPlayer().name;
        public static bool IsFriendsWith(string id) => APIUser.CurrentUser.friendIDs.Contains(id);
        internal static IEnumerator AvatarScanner()
        {
            while (true)
            {
                if (Config.Main.AvatarHider && PlayerUtils.GetCurrentUser() != null)
                {
                    foreach (VRC.Player player in PlayerUtils.GetPlayers())
                    {
                        try
                        {
                            APIUser apiUser = player.prop_APIUser_0;
                            GameObject avtrObject = GetAvatarObject(player);
                            if (avtrObject == null)
                                continue;

                            float dist = Vector3.Distance(PlayerUtils.GetCurrentUser().transform.position, avtrObject.transform.position);
                            bool isActive = avtrObject.active;

                            if (Config.Main.AvatarHider && isActive && dist > Config.Main.AviDistinceHider)
                                avtrObject.SetActive(false);
                            else if (Config.Main.AvatarHider && !isActive && dist <= Config.Main.AviDistinceHider)
                                avtrObject.SetActive(true);
                            else if (!Config.Main.AvatarHider && !isActive)
                                avtrObject.SetActive(true);

                        }
                        catch (Exception e)
                        {
                            Logs.Log($"Failed to scan avatar: {e}", ConsoleColor.Cyan);
                        }
                        yield return new WaitForSeconds(.19f);
                    }
                }
                yield return new WaitForSeconds(.5f);
            }
        }

        public static void UnHideAvatars()
        {
            try
            {
                foreach (VRC.Player player in PlayerUtils.GetPlayers())
                {
                    if (player == null || IsMe(player)) continue;


                    GameObject avtrObject = GetAvatarObject(player);

                    if (avtrObject == null || avtrObject.active)
                    {
                        continue;
                    }

                    avtrObject.SetActive(true);
                }
            }
            catch
            {

            }
        }
    }
}