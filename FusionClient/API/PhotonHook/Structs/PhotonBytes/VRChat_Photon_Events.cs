namespace FusionClient.Startup.Hooks.PhotonHook.Structs.PhotonBytes;

internal struct VRChat_Photon_Events
{
    internal const byte OpRemoveCache_etc = 0;
    internal const byte USpeaker_Voice_Data = 1;
    internal const byte Disconnect_Message = 2;
    internal const byte Cached_Events = 4; // Wut?
    internal const byte Master_allowing_player_to_join = 5;
    internal const byte RPC = 6;
    internal const byte Motion = 7;
    internal const byte interest = 8; // Wut
    internal const byte Reliable = 9;
    internal const byte Moderations = 33;
    internal const byte OpCleanRpcBuffer = 200; // (int actorNumber)
    internal const byte SendSerialize = 201;
    internal const byte Instantiation = 202;
    internal const byte CloseConnection = 203; // (PhotonPlayer kickPlayer)
    internal const byte Destroy = 204;
    internal const byte RemoveCachedRPCs = 205;
    internal const byte SendSerializeReliable = 206;
    internal const byte Destroy_Player = 207;
    internal const byte SetMasterClient = 208; // (int playerId; bool sync)
    internal const byte Request_Ownership = 209;
    internal const byte Transfer_Ownership = 210;
    internal const byte VacantViewIds = 211;
    internal const byte UploadAvatar = 223;
    internal const byte Custom_Properties = 253;
    internal const byte Leaving_World = 254;
    internal const byte Joining_World = 255;
}