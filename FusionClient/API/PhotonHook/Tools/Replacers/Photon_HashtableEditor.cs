namespace FusionClient.Startup.Hooks.PhotonHook.Tools.Replacers;

using System.Collections;
using ExitGames.Client.Photon;
using Il2CppSystem;
using Structs.PhotonBytes;
using UnhollowerRuntimeLib;
using FusionClient.Tools.UdonEditor;
using Exception = System.Exception;

internal class Photon_HashtableEditor
{
    internal static void ReplaceData(ref EventData __0, ref Hashtable ConvertedToNormalTable)
    {
        try
        {
            if (__0.Parameters[Photon_OnEventBytes.CustomData].GetIl2CppType().Equals(Il2CppType.Of<Il2CppSystem.Collections.Hashtable>()))
            {
                var ModifiedEvent = new Il2CppSystem.Collections.Hashtable();
                // Fills it if not empty!
                foreach (byte key in ConvertedToNormalTable.Keys) ModifiedEvent.Add(Il2CppConverter.Generate_Il2CPPObject(key), FC.Utils.MiscUtils.FromManagedToIL2CPP<Object>(ConvertedToNormalTable[key]));

                var rebuiltparams = new NonAllocDictionary<byte, Object>();
                for (byte i = 0; i <= byte.MaxValue; i++)
                    if (__0.Parameters.paramDict.ContainsKey(i))
                    {
                        // Replace here
                        if (i == Photon_OnEventBytes.CustomData)
                        {
                            rebuiltparams.Add(i, ModifiedEvent);
                        }
                        else
                        {
                            __0.Parameters.TryGetValue(i, out var value);
                            if (value != null) rebuiltparams.Add(i, value);
                        }
                    }

                // Clear parameters Dict
                __0.Parameters.paramDict.Clear();
                __0.Parameters.paramDict = null;
                __0.Parameters.paramDict = new NonAllocDictionary<byte, Object>();
                // Fill back everything
                for (byte i = 0; i <= byte.MaxValue; i++)
                    if (rebuiltparams.ContainsKey(i))
                        if (!__0.Parameters.paramDict.ContainsKey(i))
                        {
                            rebuiltparams.TryGetValue(i, out var value);
                            if (value != null) __0.Parameters.paramDict.Add(i, value);
                        }
            }
        }
        catch (Exception e)
        {
            MelonLoader.MelonLogger.Msg(e);
        }
    }
}