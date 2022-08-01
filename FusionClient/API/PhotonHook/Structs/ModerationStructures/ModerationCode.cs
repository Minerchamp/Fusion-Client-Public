namespace FusionClient.Startup.Hooks.PhotonHook.Structs.ModerationStructures;

internal struct ModerationCode
{
    internal const byte EventCode = 0;
    internal const byte Warning = 2;
    internal const byte Mod_Mute = 8;
    internal const byte Friend_State = 10;
    internal const byte VoteKick = 13;
    internal const byte Unknown = 20; // Unknown, seems affecting users on reset
    internal const byte Block_Or_Mute = 21;
}