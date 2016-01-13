using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMonSetup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            try
            {
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string killservice = "/c sc stop SMonSvc & sc delete SMonSvc";
                startInfo.Arguments = killservice;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }
            progressBar1.Value = 20;
            Thread.Sleep(2000);
            var hostname = "SSVHost.exe";
            var monitorname = "Monitor.TaskControl.exe";
            string oldpath = @"C:\Program Files (x86)\RM\Server\";
            string path = "C:\\Users\\Public\\RM\\Server\\";
            // kill all running processes
            var processes = FindProcess("SSVHost");
            KillProcess(processes);
            processes = FindProcess("SSVHostController");
            KillProcess(processes);
            processes = FindProcess("Monitor.TaskControl");
            KillProcess(processes);
            Thread.Sleep(1000);
            progressBar1.Value = 30;
           
            
            // delete all old files
            try
            {
                if(File.Exists(Path.Combine(oldpath, "Monitor.TaskControl.exe"))) {
                    File.Delete(Path.Combine(oldpath, "Monitor.TaskControl.exe"));
                }
                try
                {
                    if (File.Exists(Path.Combine(oldpath, "SSVHostController.exe")))
                    {
                        File.Delete(Path.Combine(oldpath, "SSVHostController.exe"));
                    }
                }
                catch (Exception ex)
                {
                    //File.AppendAllText("log.txt", ex.Message);
                }
                if (Directory.Exists(oldpath))
                {
                    Directory.Delete(oldpath, true);
                }
            }
            catch(Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            try
            {
                if (File.Exists(Path.Combine(path, hostname)))
                {
                    File.Delete(Path.Combine(path, hostname));
                }
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            try
            {
                if (File.Exists(Path.Combine(path, monitorname)))
                {
                    File.Delete(Path.Combine(path, monitorname));
                }
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            
            // File.AppendAllText("log.txt", "deleted old files");
            progressBar1.Value = 40;
            // check directory and create.
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // File.AppendAllText("Error while creating directory", ex.Message);
                }
            }
            //copy files
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonSetup.SSVHost.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "SSVHost.exe"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            progressBar1.Value = 60;
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonSetup.Monitor.TaskControl.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "Monitor.TaskControl.exe"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonSetup.DlibDotNet.dll");
                FileStream fileStream = new FileStream(Path.Combine(path, "DlibDotNet.dll"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonSetup.DlibDotNetNative.dll");
                FileStream fileStream = new FileStream(Path.Combine(path, "DlibDotNetNative.dll"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonSetup.DlibDotNetNativeDnn.dll");
                FileStream fileStream = new FileStream(Path.Combine(path, "DlibDotNetNativeDnn.dll"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }
            // File.AppendAllText("log.txt", "Copied new files");
            progressBar1.Value = 80;
            //register for the start up running
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            
            rkApp.SetValue("SSVHost", "C:\\Users\\Public\\RM\\Server\\SSVHost.exe");
            //run command line
            process = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = @"C:\Windows\System32";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            string command = "/c netsh advfirewall firewall delete rule name=\"SSVHost\"";
            command += "& netsh advfirewall firewall add rule name=\"SSVHost\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Server\\SSVHost.exe\" enable=yes";
            command += " & netsh advfirewall firewall add rule name=\"MonitorControl\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Server\\Monitor.TaskControl.exe\" enable=yes";
            startInfo.Arguments = command;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            progressBar1.Value = 100;
            MessageBox.Show("Installed successfully!");
            //delete service if it exists
            
           
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = "C:\\Users\\Public\\RM\\Server\\SSVHost.exe";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch
            {

            }
            button1.Enabled = true;
            Close();
        }
        public void KillProcess(Process[] processes)
        {
            for (int i = 0; i < processes.Length; i++)
            {
                try
                {
                    processes[i].Kill();
                }
                catch
                {

                }
            }
        }
        public Process[] FindProcess(string processName)
        {
            Process[] processList = Process.GetProcessesByName(processName);

            return processList;
        }
    }
}
