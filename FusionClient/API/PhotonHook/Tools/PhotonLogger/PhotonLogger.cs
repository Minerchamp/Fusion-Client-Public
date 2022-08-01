namespace FusionClient.Startup.Hooks.PhotonHook.Tools.PhotonLogger;

using System.Text;
using ExitGames.Client.Photon;
using Ext;
using Il2CppSystem;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using Newtonsoft.Json;
using Structs;
using Structs.ModerationStructures;
using Structs.PhotonBytes;
using Translators;
using UnhollowerRuntimeLib;
using FusionClient.Extensions;
using FC;
using FC.Utils;
using FC.Components;
using Exception = System.Exception;
using FusionClient.Core;

internal class PhotonLogger
{

    private static bool EventCodeToLog(byte code)
    {
        switch (code)
        {
            case VRChat_Photon_Events.OpRemoveCache_etc: return false;
            case VRChat_Photon_Events.USpeaker_Voice_Data: return false;
            case VRChat_Photon_Events.Disconnect_Message: return true;
            case VRChat_Photon_Events.Cached_Events: return false;
            case VRChat_Photon_Events.Master_allowing_player_to_join: return true;
            case VRChat_Photon_Events.RPC: return false;
            case VRChat_Photon_Events.Motion: return false;
            case VRChat_Photon_Events.interest: return false;
            case VRChat_Photon_Events.Reliable: return false;
            case VRChat_Photon_Events.Moderations: return true;
            case VRChat_Photon_Events.OpCleanRpcBuffer: return false;
            case VRChat_Photon_Events.SendSerialize: return false;
            case VRChat_Photon_Events.Instantiation: return false;
            case VRChat_Photon_Events.CloseConnection: return false;
            case VRChat_Photon_Events.Destroy: return false;
            case VRChat_Photon_Events.RemoveCachedRPCs: return false;
            case VRChat_Photon_Events.SendSerializeReliable: return false;
            case VRChat_Photon_Events.Destroy_Player: return false;
            case VRChat_Photon_Events.SetMasterClient: return false;
            case VRChat_Photon_Events.Request_Ownership: return false;
            case VRChat_Photon_Events.Transfer_Ownership: return false;
            case VRChat_Photon_Events.VacantViewIds: return false;
            case VRChat_Photon_Events.UploadAvatar: return false;
            case VRChat_Photon_Events.Custom_Properties: return false;
            case VRChat_Photon_Events.Leaving_World: return false;
            case VRChat_Photon_Events.Joining_World: return false;
            default:
                return false;
        }
    }

    internal static void PrintEvent(ref EventData PhotonData, HookAction HookAction, bool SkipBlock = false)
    {
        if (Config.Main.LogEvents)
        {
            StringBuilder line = new StringBuilder();
            StringBuilder prefix = new StringBuilder();
            StringBuilder ContainerType = new StringBuilder();
            try
            {
                if (EventCodeToLog(PhotonData.Code) || SkipBlock)
                {
                    var PhotonSender = FC.Utils.PhotonUtils.TryGetPlayer(PhotonData.sender);
                    var translated = Photon_StructToString.TranslateEventData(PhotonData.Code);
                    if (translated.IsNotNullOrEmptyOrWhiteSpace())
                        prefix.Append($"[Event ({PhotonData.Code})][{translated}]: ");
                    else
                        prefix.Append($"[Event ({PhotonData.Code})] ");

                    line.Append($"from: ({PhotonData.Sender}) ");
                    if (FC.Utils.WorldUtils.IsInWorld() && PhotonSender != null)
                        line.Append($"'{PhotonSender.field_Public_Player_0.field_Private_APIUser_0.GetDisplayName()}'");
                    else
                        line.Append("'NULL' ");

                    if (PhotonData.GetCustomData() != null)
                    {
                        MelonLoader.MelonLogger.Msg("Parsing From CustomData...");
                        ConvertEventDataToString(PhotonData.Code, PhotonData.GetCustomData(), ref line, ref prefix,
                            ref ContainerType);
                    }
                    else if (PhotonData.GetParameterData() != null)
                    {
                        MelonLoader.MelonLogger.Msg("Parsing From Parameters...");
                        ConvertEventDataToString(PhotonData.Code, PhotonData.GetParameterData(), ref line, ref prefix,
                            ref ContainerType);
                    }

                    MelonLoader.MelonLogger.Log($"{Photon_StructToString.HookActionToString(HookAction)}{ContainerType}{prefix}{line}");

                }
            }
            catch (Exception e)
            {
                MelonLoader.MelonLogger.Msg(e);
            }
        }
    }

    private static void ConvertEventDataToString(byte code, Il2CppSystem.Object Data, ref StringBuilder line, ref StringBuilder prefix, ref StringBuilder ContainerType)
    {
        if (Data == null) return;

        if (Data.GetIl2CppType().Equals(Il2CppType.Of<Dictionary<byte, Object>>()))
        {
            ContainerType.Append($"[Dictionary] : ");
            var casteddict = Data.Cast<Dictionary<byte, Object>>();
            if (casteddict != null)
            {
                // If Is a moderation event, get the moderation Type name 
                if (code == VRChat_Photon_Events.Moderations)
                {
                    var moderationevent = casteddict[ModerationCode.EventCode].Unbox<byte>();
                    var moderationeventname = Photon_StructToString.TranslateModerationEvent(moderationevent);
                    if (moderationeventname.IsNotNullOrEmptyOrWhiteSpace()) prefix.Append($"{moderationeventname} ");
                }

                line.AppendLine();
                line.AppendLine(JsonConvert.SerializeObject(FC.Utils.MiscUtils.FromIL2CPPToManaged<object>(casteddict), Formatting.Indented));
            }
        }

        else if (Data.GetIl2CppType().Equals(Il2CppType.Of<Hashtable>()))
        {
            ContainerType.Append("[Hashtable] : ");
            var CastedHashtable = Data.Cast<Hashtable>();
            if (CastedHashtable != null)
            {
                line.AppendLine();
                line.AppendLine(JsonConvert.SerializeObject(FC.Utils.MiscUtils.FromIL2CPPToManaged<object>(CastedHashtable), Formatting.Indented));
            }
        }
        else
        {
            MelonLoader.MelonLogger.Msg($"Unknown Type Detected : {Data.GetIl2CppType().FullName}");
            return;
        }
    }
}