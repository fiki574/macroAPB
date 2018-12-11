using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace TestApp5
{
    class Program
    {
        public static IntPtr HookID = IntPtr.Zero, Window = IntPtr.Zero;
        public static Process APB = null;
        public static int FireRate = 160, NumShots = 10;
        public static Random Rand = new Random();

        static void Main(string[] args)
        {
            try
            {
                APB = Utilities.GetProcess("APB");
                Window = DllImports.FindWindow(null, "APB Reloaded");
                if(APB == null || Window == null)
                {
                    Utilities.WriteColoredLine("{yellow}Waiting for APB to be started...");
                    while(APB == null || Window == null)
                    {
                        APB = Utilities.GetProcess("APB");
                        Window = DllImports.FindWindow(null, "APB Reloaded");
                        Thread.Sleep(1000);
                    }
                }

                Console.Clear();
                Console.Title = "GamersFirst LIVE!";

                Utilities.WriteColoredLine("{green}Welcome to simple configurable macro!\n\n{yellow}Left Control{white}: activate macro\n{yellow}Page Up{white}: increase fire interval by 1ms\n{yellow}Page Down{white}: decrease fire interval by 1ms\n{yellow}Right Shift{white}: increase number of shots macro will perform\n{yellow}Right Control{white}: decrease number of shots macro will perform\n{yellow}NumPad 2{white}: display this message\n{yellow}NumPad *{white}: activate or deactivate anti-AFK\n{yellow}Delete{white}: force shut down the macro and the game\n{yellow}Insert{white}: clear console output\n{yellow}Tab{white}: enable info printing on key press (disabled by default)\n");
                Utilities.WriteColoredLine("{green}Current settings {white}-> {yellow}fire interval {white}= {cyan}" + FireRate + "ms {white}| {yellow}number of shots {white}= {cyan}" + NumShots);

                HookID = Hooking.SetHook(Hooking.HookCallback);
                Application.Run();
                DllImports.UnhookWindowsHookEx(HookID);
            }
            catch(Exception ex)
            {
                Utilities.WriteColoredLine("\n\n{red}An exception occured! Exiting in 5 seconds...\n\n" + ex.ToString());
                Thread.Sleep(5000);
                Environment.Exit(0);
                return;
            }
        }
    }
}