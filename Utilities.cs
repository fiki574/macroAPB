using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestApp5
{
    public class Utilities
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;

            public override string ToString()
            {
                return "{green}[{yellow}Virtual Key Code{white} = {cyan}"+  vkCode + "{white} | {yellow}Scan Code{white} = {cyan}" + scanCode+ "{white} | {yellow}Flags{white} = {cyan}" + flags.ToString() + "{green}]";
            }
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_NONE = 0x00,
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80
        }

        public static Process GetProcess(string process_name)
        {
            Process[] pname = Process.GetProcessesByName(process_name);
            if (pname.Length == 0)
                return null;
            else
                return pname[0];
        }

        public static void SendKey(Keys key, int delay = 0)
        {
            DllImports.SendMessage(Program.Window, 0x0100, (IntPtr)key, CreateLParam(key, KBDLLHOOKSTRUCTFlags.LLKHF_NONE));
            Thread.Sleep(delay);
            DllImports.SendMessage(Program.Window, 0x0101, (IntPtr)key, CreateLParam(key, KBDLLHOOKSTRUCTFlags.LLKHF_UP));
        }

        public static IntPtr CreateLParam(Keys key, KBDLLHOOKSTRUCTFlags flag)
        {
            KBDLLHOOKSTRUCT lParam = new KBDLLHOOKSTRUCT()
            {
                vkCode = (uint)key,
                scanCode = DllImports.MapVirtualKey((uint)key, 0),
                flags = flag,
                time = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                dwExtraInfo = UIntPtr.Zero
            };
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(lParam));
            Marshal.StructureToPtr(lParam, ptr, true);
            return ptr;
        }

        public static void WriteColoredLine(string str)
        {
            if (!str.Contains("{") && !str.Contains("}"))
                Console.WriteLine(str);

            str += "{";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    int index = str.IndexOf('}', i), next;
                    if (index != -1)
                        next = str.IndexOf('{', index);
                    else
                        break;

                    string text = null;
                    for (int j = index + 1; j <= next - 1; j++)
                        text += str[j];

                    string color = null;
                    for (int k = i + 1; k <= index - 1; k++)
                        color += str[k];

                    switch (color)
                    {
                        case "white":
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case "blue":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "red":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case "yellow":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "green":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "cyan":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "black":
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case "gray":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case "magenta":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.Write(text);
                }
            }
            Console.WriteLine();
        }
    }
}