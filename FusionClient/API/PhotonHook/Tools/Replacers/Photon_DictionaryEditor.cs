namespace FusionClient.Startup.Hooks.PhotonHook.Tools.Replacers;

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Il2CppSystem;
using Structs;
using Structs.PhotonBytes;
using UnhollowerRuntimeLib;
using FusionClient.Tools.UdonEditor;
using Exception = System.Exception;

internal class Photon_DictionaryEditor
{

    internal static void ReplaceCustomData(ref EventData PhotonData, bool isDictionary, bool isHashTable, System.Collections.Generic.Dictionary<byte, Il2CppSystem.Object> EditableDictionary)
    {
        try
        {
            if (isDictionary)
            {
                var ModifiedEvent = new Il2CppSystem.Collections.Generic.Dictionary<byte, Object>();
                // Fills it if not empty!
                if (EditableDictionary != null && EditableDictionary.Count != 0)
                    foreach (var key in EditableDictionary.Keys)
                        ModifiedEvent.System_Collections_IDictionary_Add(Il2CppConverter.Generate_Il2CPPObject(key), EditableDictionary[key]);

                // Convert it back to il2cpp now

                PhotonData.customData = ModifiedEvent;

            }
        }
        catch (Exception e)
        {
            MelonLoader.MelonLogger.Msg(e);
        }
    }
}