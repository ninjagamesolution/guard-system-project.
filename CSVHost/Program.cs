using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Management;
using CSVHost.Model;

namespace CSVHost
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static Timer t;
        

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
            
            t = new Timer(TimerCallback, null, 0, 1000);
            AutoPatch patch = new AutoPatch();
            ZeroPatch zeroPatch = new ZeroPatch();
            UsbDetect usbDetect = new UsbDetect();
            FileSend fileSend = new FileSend();

            SendArp arp = new SendArp();
        }

        

        

        private static void RunProcess()
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = "C:\\Users\\Public\\ClientSetup.exe";
                process.StartInfo.UseShellExecute = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                //File.AppendAllText("csvhostlog.txt", ex.Message);
            }
        }

        

        private static void TimerCallback(Object o)
        {
            var processes = FindProcess("CSVAuto");
            if (processes.Length == 0)
            {
                try
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = "C:\\Users\\Public\\RM\\Client\\CSVAuto.exe";
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
