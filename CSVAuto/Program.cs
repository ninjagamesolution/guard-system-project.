using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSVAuto
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static int port = 9997;
      

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            try
            {
                //File.AppendAllText("C:\\Users\\Public\\RM\\Client\\csvautolog.txt", "---start---");
            }
            catch
            {
                // File.AppendAllText("D:\\log.txt", "---start---");
            }

            ShowWindow(handle, SW_HIDE);
            Timer t = new Timer(TimerCallback, null, 0, 1000);

            
            Console.ReadLine();
        }
        private static void TimerCallback(Object o)
        {
            var processes = FindProcess("CSVHost");
            if (processes.Length == 0)
            {
                try
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = "C:\\Users\\Public\\RM\\Client\\CSVHost.exe";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
                catch
                {
                    //File.WriteAllText("D:\\log.txt", ex.Message);
                }

            }
            else if (processes.Length > 1)
            {
                try
                {
                    processes[0].Kill();
                }
                catch
                {

                }

            }
            processes = FindProcess("Monitor.TaskView");
            if (processes.Length == 0)
            {
                try
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = "C:\\Users\\Public\\RM\\Client\\Monitor.TaskView.exe";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
                catch
                {

                }

            }
            else if (processes.Length > 1)
            {
                try
                {
                    processes[0].Kill();
                }
                catch
                {

                }

            }
            GC.Collect();
        }

        public static Process[] FindProcess(string processName)
        {
            Process[] processList = Process.GetProcessesByName(processName);

            return processList;
        }

    }
}
