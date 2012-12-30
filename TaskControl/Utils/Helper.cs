using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Security.Principal;
namespace Monitor.TaskControl.Utils
{
    public static class Helper
    {
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;  //Need to stay public because other classes use them
        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public const uint MOUSEEVENTF_XDOWN = 0x0080;
        public const uint MOUSEEVENTF_XUP = 0x0100;
        public const uint MOUSEEVENTF_WHEEL = 0x0800;
        public const uint MOUSEEVENTF_HWHEEL = 0x01000;
        public const int KEYEVENTF_EXTENDEKEY = 1;
        public const int KEYEVENTF_KEYUP = 2;
        public const int vbKeyControl = 17;
        public const int vbKeyEscape = 27;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static bool AddDesktopShortcut()
        {//As soon as you want windows to do something out of the usual its always becomes a hack, pin.invoke or a tweak to the registry. The wheels are falling off windows and its far too big
            
            return false;
        }
        public static bool IsMetro(Bitmap screeny)
        {//Crap i know but windows won't tell me if it is in metro mode using any known API or the bloated framework
            Color C1 = screeny.GetPixel(6, 6);
            Color C2 = screeny.GetPixel(screeny.Width - 6, 6);
            if (C1.ToArgb() != C2.ToArgb()) return false;
            Color C3 = screeny.GetPixel(6, screeny.Height - 30);
            Color C4 = screeny.GetPixel(screeny.Width - 6, screeny.Height - 30);
            if (C2.ToArgb() == C3.ToArgb() && C3.ToArgb() == C4.ToArgb())
                return true;
            return false;
        }

        public static void SyncKeys(string Cmd)
        {//Comes in as Caps , NumLock, Scroll-lock
            string[] Items = Cmd.ToLower().Split(' ');
            if (Control.IsKeyLocked(Keys.CapsLock).ToString().ToLower() != Items[1]) Helper.CapsLock();
            if (Control.IsKeyLocked(Keys.NumLock).ToString().ToLower() != Items[2]) Helper.NumLock();
            if (Control.IsKeyLocked(Keys.Scroll).ToString().ToLower() != Items[3]) Helper.ScrollLock();
        }

        public static void ShowTaskmanager()
        {
            try { System.Diagnostics.Process.Start(@"C:\Windows\system32\taskmgr.exe"); } catch {; }
        }

        public static void ShowMetro()
        {//Shows the metro right sidebar menu
            Helper.keybd_event((byte)Keys.LWin, 0, Helper.KEYEVENTF_EXTENDEKEY, 0);
            Helper.keybd_event((byte)Keys.C, 0, Helper.KEYEVENTF_EXTENDEKEY, 0);
            Helper.keybd_event((byte)Keys.LWin, 0, Helper.KEYEVENTF_EXTENDEKEY | Helper.KEYEVENTF_KEYUP, 0);
            Helper.keybd_event((byte)Keys.C, 0, Helper.KEYEVENTF_EXTENDEKEY | Helper.KEYEVENTF_KEYUP, 0);
            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width - 30, Screen.PrimaryScreen.Bounds.Height / 2); Thread.Sleep(20);
        }
        public static void ShowStart()
        {//shows the windows start button
            Helper.keybd_event((byte)Keys.LWin, 0, Helper.KEYEVENTF_EXTENDEKEY, 0);
            Helper.keybd_event((byte)Keys.LWin, 0, Helper.KEYEVENTF_EXTENDEKEY | Helper.KEYEVENTF_KEYUP, 0);
        }

        public static void CapsLock()
        {
            Helper.keybd_event(0x14, 0x45, Helper.KEYEVENTF_EXTENDEKEY, 0);
            Helper.keybd_event(0x14, 0x45, Helper.KEYEVENTF_EXTENDEKEY | Helper.KEYEVENTF_KEYUP, 0);
        }

        public static void NumLock()
        {
            Helper.keybd_event(0x90, 0x45, Helper.KEYEVENTF_EXTENDEKEY, 0);
            Helper.keybd_event(0x90, 0x45, Helper.KEYEVENTF_EXTENDEKEY | Helper.KEYEVENTF_KEYUP, 0);
        }

        public static void ScrollLock()
        {//Not sure why this is commented out you will have to look
            //const int KEYEVENTF_EXTENDEDKEY = 0x1;
            //const int KEYEVENTF_KEYUP = 0x2;
            //Server.keybd_event(0x14, 0x45, Server.KEYEVENTF_EXTENDEKEY, 0);
            //Server.keybd_event(0x14, 0x45, Server.KEYEVENTF_EXTENDEKEY | Server.KEYEVENTF_KEYUP, 0);
        }

        public static void ScrollHorizontal(int Amount)
        {//Scroll up/down
            Helper.mouse_event(Helper.MOUSEEVENTF_HWHEEL, 0, 0, Amount, 0);
        }

        public static void ScrollVertical(int Amount)
        {//Scroll left/right
            Helper.mouse_event(Helper.MOUSEEVENTF_HWHEEL, 0, 0, Amount, 0);
        }

        public static string XorString(string Value, int Shift, bool Outbound)//Shift changes for single keyboard letters
        {//Piss poor encryption but it should fool the tossers at the NSA or at least waste some time for them
            if (Outbound)
                Value = Value.Replace(" ", "#SS#");
            string Output = "";
            int Ch = 0;
            for (int f = 0; f <= Value.Length - 1; f++)
            {
                Ch = Convert.ToInt32(Value[f]);
                if (Outbound && Ch == 113)
                    Ch = Convert.ToInt32('¬');
                else if (!Outbound && Ch == 172)
                    Ch = 113;
                else
                    Ch ^= Shift;
                Output += char.ConvertFromUtf32(Ch);
            }
            if (!Outbound)
                return Output.Replace("#SS#", " ");
            else
                return Output;
        }


        public static bool IsUserAdministrator()
        {//If the server is not run with admin rights then windows will not allow mouse clicks on some windows pages like the task-manager
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)//Yes i know it's not used but i might use it later so learn to live with it
            {

            }
            return false;
        }
    }
}
