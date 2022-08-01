using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FusionClient.Modules
{
    internal class Optimizations
    {
        [DllImport("KERNEL32.DLL", EntryPoint =
        "SetProcessWorkingSetSize", SetLastError = true,
        CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize32Bit
        (IntPtr pProcess, int dwMinimumWorkingSetSize,
        int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint =
           "SetProcessWorkingSetSize", SetLastError = true,
           CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize64Bit
           (IntPtr pProcess, long dwMinimumWorkingSetSize,
           long dwMaximumWorkingSetSize);

        public static void RamClear()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize32Bit(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        public static IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitForSeconds(150);
                RamClear();
            }
        }
    }
}