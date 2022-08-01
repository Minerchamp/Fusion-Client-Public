using FC;
using FC.Utils;
using FusionClient.Core;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace FusionClient.Modules
{
    class HWIDSpoof : FusionModule
    {
        [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
        private static Il2CppSystem.Object ourGeneratedHwidString;

        public override void Start()
        {
            if (BotModule.IsBot)
            {
                try
                {
                    var OriginalHWID = SystemInfo.deviceUniqueIdentifier;
                    var random = new System.Random();
                    var newId = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                    {
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9)
                    }))).Select((byte x) =>
                    {
                        return x.ToString("x2");
                    }).Aggregate((string x, string y) => x + y);

                    ourGeneratedHwidString = new Il2CppSystem.Object(IL2CPP.ManagedStringToIl2Cpp(newId));

                    var icallName = "UnityEngine.SystemInfo::GetDeviceUniqueIdentifier";
                    var icallAddress = IL2CPP.il2cpp_resolve_icall(icallName);
                    if (icallAddress == IntPtr.Zero)
                    {
                        MelonLogger.Msg("[Security] Can't resolve the icall, not patching HWID", ConsoleColor.Red);
                        return;
                    }

                    unsafe
                    {
                        CompatHook((IntPtr)(&icallAddress),
                                typeof(HWIDSpoof).GetMethod(nameof(GetDeviceIdPatch),
                                    BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                    }

                    MelonLogger.Msg("Changed HWID For Your Safety", ConsoleColor.Cyan);
                }
                catch (Exception e)
                {
                    MelonLogger.Msg("[Security] Error Spoofing HWID! | " + e.Message, ConsoleColor.Red);
                }
            }
            else
            {
                if (!Config.Main.HWIDSpoofing) return;
                try
                {
                    if (string.IsNullOrEmpty(Config.Main.HWID))
                    {
                        var random = new System.Random();
                        Config.Main.HWID = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                        {
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9)
                        }))).Select((byte x) =>
                        {
                            return x.ToString("x2");
                        }).Aggregate((string x, string y) => x + y);
                    }
                    var OriginalHWID = SystemInfo.deviceUniqueIdentifier;
                    var newId = Config.Main.HWID;
                    ourGeneratedHwidString = new Il2CppSystem.Object(IL2CPP.ManagedStringToIl2Cpp(newId));
                    var icallName = "UnityEngine.SystemInfo::GetDeviceUniqueIdentifier";
                    var icallAddress = IL2CPP.il2cpp_resolve_icall(icallName);
                    if (icallAddress == IntPtr.Zero)
                    {
                        Logs.Log("[HWID Spoofer] Can't resolve the icall, not patching HWID", ConsoleColor.Red);
                        return;
                    }
                    unsafe
                    {
                        CompatHook((IntPtr)(&icallAddress), typeof(HWIDSpoof).GetMethod(nameof(GetDeviceIdPatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                    }
                    Logs.Log("=====[HWID SPOOFER]=====", ConsoleColor.White);
                    Logs.Log($"OLD HWID [{OriginalHWID}]", ConsoleColor.Cyan);
                    Logs.Log($"NEW HWID:  [{newId}]", ConsoleColor.Magenta);
                    bool vrcSpoofed = false;
                    bool unitySpoofed = false;
                    if (VRC.Core.API.DeviceID == newId) vrcSpoofed = true;
                    if (SystemInfo.deviceUniqueIdentifier == newId) unitySpoofed = true;

                    if (vrcSpoofed && unitySpoofed)
                    {
                        Logs.Log("VRC [Success] | Unity [Success]", ConsoleColor.Yellow);
                    }
                    else
                    {
                        if (vrcSpoofed && !unitySpoofed)
                        {
                            Logs.Log("VRC [Success] | Unity [Failed]", ConsoleColor.Yellow);
                        }
                        if (!vrcSpoofed && unitySpoofed)
                        {
                            Logs.Log("VRC [Failed] | Unity [Success]", ConsoleColor.Yellow);
                        }
                    }
                    Logs.Log("========================", ConsoleColor.White);
                }
                catch (Exception e)
                {
                    Logs.Log("[HWID Spoofer] Error Spoofing HWID! | " + e.Message, ConsoleColor.Red);
                }
            }
        }

        private static IntPtr GetDeviceIdPatch() => ourGeneratedHwidString.Pointer;

        private static void CompatHook(IntPtr first, IntPtr second)
        {
            typeof(Imports).GetMethod("Hook", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!
                .Invoke(null, new object[] { first, second });
        }
    }
}