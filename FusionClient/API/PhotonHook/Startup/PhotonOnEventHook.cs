namespace FusionClient.Startup.Hooks.PhotonHook.Startup;

using System.IO;
using System.Reflection;
using FusionClient;
using ExitGames.Client.Photon;
using FC.Utils;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Newtonsoft.Json;
using Photon.Realtime;
using PhotonHandlers;
using Structs;
using Structs.PhotonBytes;
using Tools.Ext;
using Tools.PhotonLogger;
using Tools.Replacers;
using Tools.Translators;
using UnhollowerRuntimeLib;
using Exception = System.Exception;

[Obfuscation(Feature = "HarmonyRenamer")]
internal class PhotonOnEventHook
{
    internal static bool LogCurrentHookActivity { get; set; } = true;

    [Obfuscation(Feature = "HarmonyGetPatch")]
    private static HarmonyMethod GetPatch(string name)
    {
        return new HarmonyMethod(typeof(PhotonOnEventHook).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
    }

    internal static void HookPhotonOnEvent()
    {
        new FusionPatch(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.OnEvent)), GetPatch(nameof(OnEventPatch)));
    }

    // Current Targeted Byte 

    private static bool OnEventPatch(ref EventData __0)
    {
        try
        {
            if (__0 != null)
            {
                HookAction CurrentAction = HookAction.Nothing;

                bool isParameterData = false;
                bool isCustomData = false;

                bool isHashtable = false;
                bool isDictionary = false;
                bool isUnknownType = false;

                // These two exists For easily edit and rebuild them instead of editing Original data (Unhollower crashes)

                System.Collections.Generic.Dictionary<byte, Il2CppSystem.Object> Dictionary_EditableCopy = new System.Collections.Generic.Dictionary<byte, Object>();
                // System.Collections.Hashtable Hashtable_EditableCopy = new System.Collections.Hashtable();

                // Identify what Type & Where is the data 
                if (__0.CustomData != null)
                {
                    isCustomData = true;
                    //MelonLoader.MelonLogger.Msg($"Received CustomData with Type : {__0.CustomData}");
                    if (__0.CustomData.GetIl2CppType().Equals(Il2CppType.Of<Dictionary<byte, Object>>()))
                    {
                        // Flag and convert the item that needs to be processed into a Editable copy .
                        isDictionary = true;

                        // This casts the dictionary still to a Il2cpp one, we need a system one!

                        var casted = __0.CustomData.Cast<Dictionary<byte, Il2CppSystem.Object>>();
                        if (casted != null)
                        {
                            // Copy all the stuff contained into The System Dictionary!
                            foreach (var item in casted)
                            {
                                //MelonLoader.MelonLogger.Msg($"Copying Item Key {item.Key}");
								
                                // Copy using only the Existing keys!
                                Dictionary_EditableCopy.Add(item.Key, casted[item.Key]);
                            }

                        }
                    }

                    else if (__0.CustomData.GetIl2CppType().Equals(Il2CppType.Of<Hashtable>()))
                    {
                        isHashtable = true;
                        // TODO: ADD A CAST SYSTEM TO CONVERT IL2CPP HASHTABLE BACK TO A SYSTEM HASHTABLE (not important unless needed)!

                    }
                    else
                        isUnknownType = true;
                }
                else if (__0.GetParameterData() != null)
                {
                    isParameterData = true;
                    //MelonLoader.MelonLogger.Msg($"Received CustomData with Type : {__0.CustomData}");
                    if (__0.GetParameterData().GetIl2CppType().Equals(Il2CppType.Of<Dictionary<byte, Object>>()))
                    {
                        // Flag and convert the item that needs to be processed into a Editable copy .
                        isDictionary = true;

                        // This casts the dictionary still to a Il2cpp one, we need a system one!

                        var casted = __0.GetParameterData().Cast<Dictionary<byte, Il2CppSystem.Object>>();
                        if (casted != null)
                        {
                            // Copy all the stuff contained into The System Dictionary!
                            foreach (var item in casted)
                            {
                                //MelonLoader.MelonLogger.Msg($"Copying Item Key {item.Key}");

                                // Copy using only the Existing keys!
                                Dictionary_EditableCopy.Add(item.Key, casted[item.Key]);
                            }

                        }
                    }

                }

                switch (__0.Code)
                {

                    case VRChat_Photon_Events.Moderations:
                    {
                        // Give a safe copy to patch and replace.
                        if (isCustomData)
                        {
                            // Feed only the Editable copy and let it process
                            if (isDictionary)
                            {
                                CurrentAction = Photon_PlayerModerationHandler.HandleEvent(ref Dictionary_EditableCopy);
                            }
                        }

                        break;
                    }

                }
                switch (CurrentAction)
                {
                    case HookAction.Patch:

                        if (isCustomData)
                        {
                            // Do Patching and Report
                            if (isDictionary)
                            {
                                Photon_DictionaryEditor.ReplaceCustomData(ref __0, isDictionary, isHashtable, Dictionary_EditableCopy);
                            }
                        }

                        if(LogCurrentHookActivity) PhotonLogger.PrintEvent(ref __0, CurrentAction);
                        return true;
                    case HookAction.Empty:
                        if (isCustomData)
                        {
                            // Do Patching and Report
                            if (isDictionary)
                            {
                                Photon_DictionaryEditor.ReplaceCustomData(ref __0, isDictionary, isHashtable, new System.Collections.Generic.Dictionary<byte, Object>());
                            }
                        }

                        if(LogCurrentHookActivity) PhotonLogger.PrintEvent(ref __0, CurrentAction);
                        return true;

                    case HookAction.Block:
                      if(LogCurrentHookActivity) PhotonLogger.PrintEvent(ref __0, CurrentAction);
                        return false;
                    case HookAction.Reset:
                       if(LogCurrentHookActivity) PhotonLogger.PrintEvent(ref __0, CurrentAction);
                        __0.Reset();
                        return true;
                    default:
                        if(LogCurrentHookActivity) PhotonLogger.PrintEvent(ref __0, CurrentAction);
                        return true;
                }
            }
        }
        catch (Exception e)
        {
            MelonLoader.MelonLogger.Msg("Exception in OnEvent");
            MelonLoader.MelonLogger.Msg(e);
            return true;
        }
        return true;
    }
}