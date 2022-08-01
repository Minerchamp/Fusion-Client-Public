namespace FusionClient.Startup.Hooks.PhotonHook.Structs;

/// <summary>
///     Actions that the hook Needs to do.
/// </summary>
internal enum HookAction
{
    /// <summary>Do Nothing</summary>
    Nothing,

    /// <summary>Applies any edit you put in the Reflected HashTable/Dictionary in the current parameterDict</summary>
    Patch,

    /// <summary>Empties the ParameterDictionary with a empty Dictionary/Hashtable Content (Might break some stuff)</summary>
    Empty,

    /// <summary>Block the Event completely (Will break some stuff)</summary>
    Block,

    /// <summary>Resets the EventData (Breaks Every event that you reset!)</summary>
    Reset
}