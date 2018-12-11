using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace TestApp5
{
    public class Hooking
    {
        public static System.Timers.Timer Macro, AntiAFK;
        public static bool IsMacroRunning = false, IsAntiAFKRunning = false, IsPrintInfoEnabled = false;

        public static IntPtr SetHook(DllImports.LowLevelKeyboardProc proc)
        {
            return DllImports.SetWindowsHookEx(13, proc, DllImports.GetModuleHandle(Program.APB.MainModule.ModuleName), 0);
        }

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Utilities.KBDLLHOOKSTRUCT key = (Utilities.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Utilities.KBDLLHOOKSTRUCT));
            if(IsPrintInfoEnabled) Utilities.WriteColoredLine(key.ToString());
            if (nCode >= 0 && wParam == (IntPtr)0x0100)
            {
                switch ((Keys)Marshal.ReadInt32(lParam))
                {
                    case Keys.LControlKey:
                        {
                            if (!IsMacroRunning)
                            {
                                Macro = new System.Timers.Timer(1 + Program.Rand.Next(0, 4));
                                Macro.Elapsed += MacroTimedEvent;
                                Macro.AutoReset = false;
                                Macro.Enabled = true;
                                IsMacroRunning = true;
                            }
                            else
                                IsMacroRunning = false;
                            break;
                        }

                    case Keys.PageUp:
                        {
                            Program.FireRate++;
                            Utilities.WriteColoredLine("{green}Increased{white} fire interval to {yellow}" + Program.FireRate + "ms");
                            break;
                        }

                    case Keys.PageDown:
                        {
                            Program.FireRate--;
                            Utilities.WriteColoredLine("{red}Decreased{white} fire interval to {yellow}" + Program.FireRate + "ms");
                            break;
                        }

                    case Keys.RShiftKey:
                        {
                            Program.NumShots++;
                            Utilities.WriteColoredLine("{green}Increased{white} number of macro shots to {yellow}" + Program.NumShots);
                            break;
                        }

                    case Keys.RControlKey:
                        {
                            Program.NumShots--;
                            Utilities.WriteColoredLine("{red}Decreased{white} number of macro shots to {yellow}" + Program.NumShots);
                            break;
                        }

                    case Keys.Delete:
                        {
                            if (Program.APB.MainModule.BaseAddress != null)
                                Program.APB.Kill();

                            Environment.Exit(0);
                            break;
                        }

                    case Keys.NumPad2:
                        {
                            Utilities.WriteColoredLine("{yellow}Left Control{white}: activate macro\n{yellow}Page Up{white}: increase fire interval by 1ms\n{yellow}Page Down{white}: decrease fire interval by 1ms\n{yellow}Right Shift{white}: increase number of shots macro will perform\n{yellow}Right Control{white}: decrease number of shots macro will perform\n{yellow}NumPad 2{white}: display this message\n{yellow}NumPad *{white}: activate or deactivate anti-AFK\n{yellow}Delete{white}: force shut down the macro and the game\n{yellow}Insert{white}: clear console output\n");
                            break;
                        }

                    case Keys.Insert:
                        {
                            Console.Clear();
                            break;
                        }

                    case Keys.Multiply:
                        {
                            if(!IsAntiAFKRunning)
                            {
                                AntiAFK = new System.Timers.Timer(45000);
                                AntiAFK.Elapsed += AntiAFKTimedEvent;
                                AntiAFK.AutoReset = true;
                                AntiAFK.Enabled = true;
                                IsAntiAFKRunning = true;
                                Utilities.WriteColoredLine("{yellow}Anti-AFK {white}is {green}running{white}!");
                            }
                            else
                            {
                                IsAntiAFKRunning = false;
                                AntiAFK.Stop();
                                Utilities.WriteColoredLine("{yellow}Anti-AFK {white}has been {red}stopped{white}!");
                            }
                            break;
                        }

                    case Keys.Tab:
                        {
                            if(IsPrintInfoEnabled)
                            {
                                Utilities.WriteColoredLine("{yellow}Key press info printing {white}is {red}disabled{white}!");
                                IsPrintInfoEnabled = false;
                            }
                            else
                            {
                                Utilities.WriteColoredLine("{yellow}Key press info printing {white}is {green}enabled{white}!");
                                IsPrintInfoEnabled = true;
                            }
                            break;
                        }
                }
            }
            return DllImports.CallNextHookEx(Program.HookID, nCode, wParam, lParam);
        }

        private static void MacroTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Program.NumShots; i++)
            {
                if (IsMacroRunning)
                {
                    Utilities.SendKey(Keys.LControlKey);
                    Thread.Sleep(Program.FireRate + Program.Rand.Next(0, 5));
                }
                else
                    break;
            }
            IsMacroRunning = false;
        }

        private static void AntiAFKTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < 5; i++) Utilities.SendKey(Keys.W, 200);
            Thread.Sleep(1000);
            Utilities.SendKey(Keys.G, 200);
            Thread.Sleep(2000);
            for (int i = 0; i < 5; i++) Utilities.SendKey(Keys.D, 200);
            Thread.Sleep(3000);
            Utilities.SendKey(Keys.Space, 200);
            Thread.Sleep(4000);
            Utilities.SendKey(Keys.LControlKey, 200);
        }
    }
}