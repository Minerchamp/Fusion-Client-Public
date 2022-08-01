namespace FusionClient.Startup.Hooks.PhotonHook.Tools.Translators;

using Structs;
using Structs.ModerationStructures;
using Structs.PhotonBytes;

internal static class Photon_StructToString
{
    internal static string TranslateModerationEvent(byte moderationEvent)
    {
        switch (moderationEvent)
        {
            case ModerationCode.Warning: return "Warning";
            case ModerationCode.Mod_Mute: return "Mod Mute";
            case ModerationCode.Friend_State: return "Friend State";
            case ModerationCode.VoteKick: return "VoteKick";
            case ModerationCode.Unknown: return "Unknown";
            case ModerationCode.Block_Or_Mute: return "Block/Mute";
            default:
                return $"Unknown Moderation Event Byte {moderationEvent}";
        }
    }

    internal static string HookActionToString(HookAction action)
    {
        switch (action)
        {
            case HookAction.Patch: return "PATCHED ";
            case HookAction.Block: return "BLOCKED ";
            case HookAction.Empty: return "EMPTIED ";
            case HookAction.Reset: return "RESET ";
            default:
                return string.Empty;
        }
    }

    internal static string TranslateEventData(byte code)
    {
        switch (code)
        {
            case VRChat_Photon_Events.OpRemoveCache_etc: return "OpRemoveCache , etc...";
            case VRChat_Photon_Events.USpeaker_Voice_Data: return "USpeak voice Data";
            case VRChat_Photon_Events.Disconnect_Message: return "Disconnect (Kick)";
            case VRChat_Photon_Events.Cached_Events: return "Cached Events";
            case VRChat_Photon_Events.Master_allowing_player_to_join: return "Master allowing player to join";
            case VRChat_Photon_Events.RPC: return "RPC";
            case VRChat_Photon_Events.Motion: return "Motion";
            case VRChat_Photon_Events.interest: return "Interest";
            case VRChat_Photon_Events.Reliable: return "Reliable";
            case VRChat_Photon_Events.Moderations: return "Moderation";
            case VRChat_Photon_Events.OpCleanRpcBuffer: return "OPCleanRPCBuffer (int actorNumber)";
            case VRChat_Photon_Events.SendSerialize: return "SendSerialize";
            case VRChat_Photon_Events.Instantiation: return "Instantiation";
            case VRChat_Photon_Events.CloseConnection: return "CloseConnection (PhotonPlayer kickPlayer)";
            case VRChat_Photon_Events.Destroy: return "Destroy";
            case VRChat_Photon_Events.RemoveCachedRPCs: return "RemoveCachedRPCs";
            case VRChat_Photon_Events.SendSerializeReliable: return "SendSerializeReliable";
            case VRChat_Photon_Events.Destroy_Player: return "Destroy Player";
            case VRChat_Photon_Events.SetMasterClient: return "SetMasterClient (int playerId, bool sync)";
            case VRChat_Photon_Events.Request_Ownership: return "Request Ownership";
            case VRChat_Photon_Events.Transfer_Ownership: return "Transfer Ownership";
            case VRChat_Photon_Events.VacantViewIds: return "VacantViewIds";
            case VRChat_Photon_Events.UploadAvatar: return "UploadAvatar";
            case VRChat_Photon_Events.Custom_Properties: return "Custom Properties";
            case VRChat_Photon_Events.Leaving_World: return "Leaving World";
            case VRChat_Photon_Events.Joining_World: return "Joining World";
            default:
                return $"Unrecognized Byte : {code}";
        }
    }
}