using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FC.Utils;
using MelonLoader;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using VRC;

namespace FusionClient.Modules
{
    public class FCNameplate : MonoBehaviour
    {
        public FCNameplate(IntPtr id) : base(id) { }

        internal static int lastNetworkedUpdatePacketNumber;
        internal static float lastNetworkedUpdateTime;
        internal static int lagBarrier;
        internal TextMeshProUGUI text;
        internal VRCPlayer vrcPlayer;

        //public void LateUpdate()
        //{
        //    if (text != null && vrcPlayer != null)
        //        text.text = GetInfo(vrcPlayer);
        //}

        public void Start()
        {
            InvokeRepeating("Stats", 0f, 1f);
        }

        void Stats()
        {
            if (text != null && vrcPlayer != null)
                text.text = GetInfo(vrcPlayer);
        }

        [HideFromIl2Cpp]

        internal static string GetInfo(VRCPlayer instance)
        {
            int networkedUpdatePacketNumber = instance.prop_PlayerNet_0.field_Private_Int32_0;
            float lastNetworkedUpdateDelta = Time.realtimeSinceStartup - lastNetworkedUpdateTime;
            short ping = MelonUtils.Clamp<short>(instance.prop_PlayerNet_0.prop_Int16_0, 0, short.MaxValue);

            var results = string.Empty;

            //This would be unreliable if we didn't consider ping in the threshold too
            float lagThreshold = 0.3f + (Mathf.Min(ping, 500f) / 1000f);

            //If the latest received packet is over the threshold, then something is clearly wrong
            if (lastNetworkedUpdateDelta > lagThreshold && lagBarrier < 5)
            {
                lagBarrier++;
            }

            //Players latest packet number has changed - decrease their lag indicator
            if (lastNetworkedUpdatePacketNumber != networkedUpdatePacketNumber)
            {
                lastNetworkedUpdatePacketNumber = networkedUpdatePacketNumber;
                lastNetworkedUpdateTime = Time.realtimeSinceStartup + Time.time;
                lagBarrier--;
            }

            if (instance.GetVRCPlayerApi().playerId != PlayerUtils.GetCurrentUser().GetVRCPlayerApi().playerId)
            {
                if (lastNetworkedUpdateDelta > 10f)
                {
                    results += ("<color=white>[<color=red>Crashed<color=white>] | ");
                }
                else if (lagBarrier > 0)
                {
                    results += ("<color=white>[<color=yellow>Lagging<color=white>] | ");
                }
                else
                {
                    results += ("<color=white>[<color=green>Stable<color=white>] | ");
                }
            }

            // Trust Rank
            //results += $"[<color={instance.GetAPIUser().GetRankColor()}>{instance.GetAPIUser().GetRank().Substring(0, 3)}</color>] ";

            //Hacker Detector
            PlayerUtils.IsHacker(instance);
            if (PlayerUtils.UserIds.Contains(instance.GetAPIUser().id))
            {
                results += $"[<color=red>Client User</color>] | ";
            }

            //Platform
            results += $"[{instance._player.GetPlatformNP()}";
            results += $"</color>] | ";

            // Ping
            results += $"[Ping: {GetPingColored(instance._player)}] | ";

            // Frames
            results += $"[FPS: {GetFramesColored(instance._player)}] | ";

            // Master
            if (instance._player.IsInstanceMaster())
                results += "[<color=#FFD700>Master</color>] | ";

            // Friend
            if (instance._player.IsFriend())
                results += "[<color=yellow>F</color>] | ";

            if (instance.GetAPIUser().hasSuperPowers || instance.GetAPIUser().hasModerationPowers)
                results += "[<color=red>[MOD]</color>] | ";

            //if (PlayerUtils.IsFlying(instance._player))
            //    results += "[<color=red>Flying</color>] ";

            return results;
        }

        [HideFromIl2Cpp]
        private static string GetPingColored(Player instance)
        {
            var ping = instance.GetPing();
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            return ping <= 35 ? $"<color=green>{ping}</color>" : $"<color=yellow>{ping}</color>";
        }

        [HideFromIl2Cpp]
        private static string GetFramesColored(VRC.Player instance)
        {
            var frames = instance.GetFrames();
            if (frames >= 65)
            {
                return $"<color=green>{frames}</color>";
            }
            return frames <= 28 ? $"<color=red>{frames}</color>" : $"<color=yellow>{frames}</color>";
        }
    }
}