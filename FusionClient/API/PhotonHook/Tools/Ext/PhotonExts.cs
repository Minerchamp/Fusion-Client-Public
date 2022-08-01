namespace FusionClient.Startup.Hooks.PhotonHook.Tools.Ext;

using ExitGames.Client.Photon;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using FC;
using FC.Components;
using FC.Utils;
using FusionClient.Tools.UdonEditor;
using FusionClient.Extensions;

internal static class PhotonExts
{
    internal static Il2CppSystem.Object GetCustomData(this EventData data)
    {
        if (data != null)
        {
            if (data.CustomData != null)
            {
                return data.CustomData;
            }

            if (data.customData != null)
            {
                return data.customData;
            }
        }
        return null;
    }


    internal static Il2CppSystem.Object GetParameterData(this EventData data)
    {
        if (data != null)
        {
            if (data.Parameters.ContainsKey(251))
            {
               return data.Parameters[251];
            }
        }
        return null;
    }


    internal static bool IsLocalUser(this Hashtable table)
    {
        if (table != null)
        {
            if (table.ContainsKey("user"))
            {
                var userkey = table["user"].Cast<Dictionary<string, Il2CppSystem.Object>>();
                if (userkey.ContainsKey("id"))
                {
                    var ApiUserID = userkey["id"].ToString();
                    if (ApiUserID == PlayerUtils.GetCurrentUser().GetAPIUser().id)
                    {
                        return true;
                    }
                    return false;
                }
            }

        }

        return false;
    }

    internal static void DumpKeys(this EventData data)
    {
        try
        {
            if (data != null)
            {
                if (data.Parameters != null)
                {
                    MelonLoader.MelonLogger.Msg($"Current Event Code : {data.Code}");
                    for (byte i = 0; i < byte.MaxValue; i++)
                    {
                        if (data.Parameters.ContainsKey(i))
                        {
                            MelonLoader.MelonLogger.Msg($"Parameter Contains key {i}, with Type {data.Parameters[i].GetIl2CppType().FullName}");
                        }
                    }
                }
                else if (data.Parameters.paramDict != null)
                {
                    MelonLoader.MelonLogger.Msg($"Current Event Code : {data.Code}");
                    for (byte i = 0; i < byte.MaxValue; i++)
                    {
                        if (data.Parameters.paramDict.ContainsKey(i))
                        {
                            MelonLoader.MelonLogger.Msg($"paramDict Contains key {i}, with Type {data.Parameters.paramDict[i].GetIl2CppType().FullName}");
                        }
                    }
                }

            }
        }
        catch{} // SHUT THE FUCK UP
        
    }
}