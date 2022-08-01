using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FC.Utils;
using FusionClient.Core;
using MelonLoader;
using Photon.Realtime;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using VRC;
using VRC.Core;

namespace FusionClient.Modules
{
    public class PlayerList : MonoBehaviour
    {
        public PlayerList(IntPtr id) : base(id) { }
        public static float targetTime = 0.5f;

        public void Awake() => targetTime = 0f;

        public void Update()
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0)
            {
                try
                {
                    var PlayerCount = WorldUtils.GetPlayerCount();
                    string Players;
                    if (WorldUtils.GetPlayerCount() > WorldUtils.GetCurrentWorld().capacity)
                    {
                        Players = $"<b><color=#{ColorUtility.ToHtmlStringRGB(new Color32(0, 255, 255, 255))}> ᶠᵘˢᶦᵒⁿᶜˡᶦᵉⁿᵗ ᴾˡᵃʸᵉʳᴸᶦˢᵗ \n</color>Room Count: <color=red>{PlayerCount}</color>/<color=#{ColorUtility.ToHtmlStringRGB(new Color32(0, 148, 255, 255))}>{WorldUtils.GetCurrentWorld().capacity}</color></b> <color=yellow><i>World is over capacity</i></color>\n";
                    }
                    else
                    {
                        Players = $"<b><color=#{ColorUtility.ToHtmlStringRGB(new Color32(0, 255, 255, 255))}> ᶠᵘˢᶦᵒⁿᶜˡᶦᵉⁿᵗ ᴾˡᵃʸᵉʳᴸᶦˢᵗ \n</color>Room Count: <color=#{ColorUtility.ToHtmlStringRGB(new Color32(0, 148, 255, 255))}>{PlayerCount}</color>/<color=#{ColorUtility.ToHtmlStringRGB(new Color32(0, 148, 255, 255))}>{WorldUtils.GetCurrentWorld().capacity}</color></b>\n";
                    }
                    List<Photon.Realtime.Player> list = PlayerUtils.LoadBalancingPeer.GetAllPhotonPlayers();
                    for (int i = 0; i < list.Count; i++)
                    {
                        Photon.Realtime.Player player = list[i];
                        var returnstring = string.Empty;
                        string playerstring;

                        var p = player.GetPlayer();
                        if (p == null)
                        {
                            playerstring = $"[<color=yellow>{player.GetPhotonID()}</color>] {player.GetDisplayName()} <color=red>[INVIS]</color> ";
                        }
                        else
                        {
                            playerstring = $" {GetTags(p)} {p.GetPlatformColored()} {p.GetiFft()} <color={p.GetAPIUser().GetRankColor()}>{p.GetAPIUser().displayName}</color> | [P:{GetPingColored(p)}] [F:{GetFramesColored(p)}] ";
                        }
                        returnstring += playerstring;
                        Players += returnstring + "\n";
                    }
                    UI.PlayerList.SetText(Players);
                }
                catch { }
                targetTime = 0.5f;
            }
        }

        [HideFromIl2Cpp]
        private string GetTags(VRC.Player instance)
        {
            var tags = string.Empty;

            if (instance.IsInstanceMaster())
                tags += "<color=#ff6600>[M]</color>";

            if (instance.IsFriend())
                tags += "<color=yellow>[F]</color>";

            if (instance.IsFlying())
                tags += "<color=#A6CACD>[FLY]</color>";

            if (instance.GetAPIUser().isSupporter)
                tags += "<color=yellow>[V+]</color> ";

            if (instance.GetAPIUser().hasSuperPowers || instance.GetAPIUser().hasModerationPowers || instance.GetAPIUser().showModTag || instance.GetAPIUser().hasScriptingAccess)
                tags += "<color=red>[MOD]</color> ";

            return tags;
        }

        [HideFromIl2Cpp]
        internal static string GetPingColored(VRC.Player instance)
        {
            var ping = instance.GetPing();
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            return ping <= 35 ? $"<color=green>{ping}</color>" : $"<color=yellow>{ping}</color>";
        }

        [HideFromIl2Cpp]
        internal static string GetFramesColored(VRC.Player instance)
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