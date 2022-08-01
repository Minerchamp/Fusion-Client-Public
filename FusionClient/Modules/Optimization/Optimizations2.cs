using System;
using System.Linq;
using MelonLoader;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security;

namespace FusionClient.Modules
{
    public class Optimizations2 : MelonMod
    {
        public static bool enable = true;

        [Flags]
        private enum AllocationType : uint
        {
            COMMIT = 0x1000,
        }

        [Flags]
        private enum MemoryProtection : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuartModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        private enum FreeType : uint
        {
            MEM_DECOMMIT = 0x4000,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private unsafe static extern byte* VirtualAlloc(byte* lpAddress, UIntPtr dwSize,
        AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private unsafe static extern bool VirtualProtect(byte* lpAddress, UIntPtr dwSize,
        MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        [SecurityCritical]
        private unsafe static extern IntPtr memset(byte* dest, int c, long count);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public unsafe static byte* Allocate(int size)
        {

            var remaining = size % 4096;
            var sizeInPages = (size / 4096) + (remaining == 0 ? 0 : 1);

            var allocatedSize = ((sizeInPages + 2) * 4096);
            var virtualAlloc = VirtualAlloc(null, (UIntPtr)allocatedSize, AllocationType.COMMIT,
                MemoryProtection.ReadWrite);
            if (virtualAlloc == null)
                throw new Win32Exception();

            *(int*)virtualAlloc = allocatedSize;

            MemoryProtection protect;
            if (VirtualProtect(virtualAlloc, (UIntPtr)(4096), MemoryProtection.NoAccess,
                    out protect) == false)
                throw new Win32Exception();

            if (VirtualProtect(virtualAlloc + (sizeInPages + 1) * 4096, (UIntPtr)(4096), MemoryProtection.NoAccess,
                    out protect) == false)
                throw new Win32Exception();

            var firstWritablePage = virtualAlloc + 4096;

            memset(firstWritablePage, 0xED, 4096 * sizeInPages);
            if (remaining == 0)
                return firstWritablePage;

            return firstWritablePage + (4096 - remaining);

        }

        public static IntPtr GetModuleBaseAddress(string processName, string moduleName)
        {
            // Get an instance of the specified process
            Process process;

            try
            {
                process = Process.GetProcessesByName(processName)[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException($"No process with name {processName} is currently running");
            }

            var module = process.Modules.Cast<ProcessModule>().SingleOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));
            return module?.BaseAddress ?? IntPtr.Zero;
        }

        public unsafe static void OnApplicationStart()
        {
            if (enable)
            {
                enable = false;
                IntPtr vr_handle = Process.GetProcessesByName("VRChat")[0].Handle;
                byte[] shellcode = new byte[69] {

                   0x50, 0x53, 0x51, 0x52, 0x56, 0x57, 0x55, 0x41, 0x50, 0x41, 0x51, 0x41, 0x52, 0x41, 0x53, 0x41, 0x54, 0x41, 0x55, 0x41, 0x56, 0x41, 0x57, 0x9C,

                   0xb9, 0x03, 0x00, 0x00, 0x00,

                   0x9D, 0x41, 0x5F, 0x41, 0x5E, 0x41, 0x5D, 0x41, 0x5C, 0x41, 0x5B, 0x41, 0x5A, 0x41, 0x59, 0x41, 0x58, 0x5D, 0x5F, 0x5E, 0x5A, 0x5a, 0x5B, 0x58, // 54

	               0x48, 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,

                   0xc3
                };
                ulong remote_allocation_shell = (ulong)VirtualAlloc(null, (UIntPtr)shellcode.Length, AllocationType.COMMIT,
                MemoryProtection.ExecuteReadWrite);
                Marshal.Copy(shellcode, 0, (IntPtr)(remote_allocation_shell), shellcode.Length);
                ulong kernel32module = (ulong)GetModuleBaseAddress("VRChat", "kernel32.dll");
                ulong kernelbasemodule = (ulong)GetModuleBaseAddress("VRChat", "KernelBase.dll");
                ulong local_kernel32_base = (ulong)LoadLibrary("kernel32.dll");
                ulong local_kernelbase_base = (ulong)LoadLibrary("KernelBase.dll");
                ulong remote_kernel32_modulehandle = kernel32module +
                (ulong)(GetProcAddress((IntPtr)local_kernel32_base, "Sleep")) - (local_kernel32_base);
                ulong remote_kernelbase_modulehandle = kernelbasemodule +
                (ulong)(GetProcAddress((IntPtr)local_kernelbase_base, "Sleep")) - (local_kernelbase_base);
                byte[] jmp_address = BitConverter.GetBytes(remote_kernelbase_modulehandle);
                jmp_address.CopyTo(shellcode, 60);
                Marshal.Copy(shellcode, 0, (IntPtr)(remote_allocation_shell), shellcode.Length);
                // place jmp on kernel32 sleep
                byte[] jmp_shellcode = new byte[14] {0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                byte[] jmp_addressog = BitConverter.GetBytes(remote_allocation_shell);
                jmp_addressog.CopyTo(jmp_shellcode, 6);
                MemoryProtection protect;
                if (VirtualProtect((byte*)remote_kernel32_modulehandle, (UIntPtr)(4096), MemoryProtection.ExecuteReadWrite,
                out protect) == false)
                    throw new Win32Exception();
                Marshal.Copy(jmp_shellcode, 0, (IntPtr)(remote_kernel32_modulehandle), jmp_shellcode.Length);
            }
        }
    }
}