using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TestApp5
{
    public class Memory
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags)
        {
            IntPtr handle = OpenProcess(flags, false, process.Id);
            if (handle == IntPtr.Zero) throw new Exception(string.Format("Failed to open process ({0}).", GetLastError()));
            return handle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        public static int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesWritten;
            int result = WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesWritten);
            if (result == 0)
                throw new Exception(string.Format("Failed to write to process ({0}).", GetLastError()));
            return result;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        public static int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
        {
            IntPtr bytesRead;
            int result = ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, out bytesRead);
            if (result == 0)
                throw new Exception("Failed to read from process.");
            return result;
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern int VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static uint VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int size, uint newProtect)
        {
            uint oldProtect;
            int result = VirtualProtectEx(hProcess, lpAddress, new IntPtr(size), newProtect, out oldProtect);
            if (result == 0)
                throw new Exception(string.Format("Failed to chance access ({0}).", GetLastError()));
            return oldProtect;
        }

        public static int WriteString(IntPtr handle, IntPtr address, string str)
        {
            IntPtr written;
            byte[] data = Encoding.Default.GetBytes(str + "\0");
            return WriteProcessMemory(handle, address, data, data.Length, out written);
        }
    }
}