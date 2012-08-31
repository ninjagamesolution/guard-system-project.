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

namespace CMonSetup
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
            //delete service if it exists
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = @"C:\Windows\System32";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            string killservice = "/c sc stop CMonSvc & sc delete CMonSvc";
            startInfo.Arguments = killservice;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            progressBar1.Value = 20;
            Thread.Sleep(2000);

            var hostname = "CSVHost.exe";
            var monitorname = "Monitor.TaskView.exe";
            string oldpath = @"C:\Program Files (x86)\RM\Client\";
            string path = "C:\\Users\\Public\\RM\\Client\\";
            // kill all running processes
            var processes = FindProcess("CSVHost");
            KillProcess(processes);

            processes = FindProcess("CSVAuto");
            KillProcess(processes);

            processes = FindProcess("CSVHostController");
            KillProcess(processes);
            processes = FindProcess("Monitor.TaskView");
            KillProcess(processes);
            Thread.Sleep(1000);
            progressBar1.Value = 30;
            // delete all old files
            try
            {
                if (File.Exists(Path.Combine(oldpath, "Monitor.TaskView.exe")))
                {
                    File.Delete(Path.Combine(oldpath, "Monitor.TaskView.exe"));
                }
                try
                {
                    if (File.Exists(Path.Combine(oldpath, "CSVHostController.exe")))
                    {
                        File.Delete(Path.Combine(oldpath, "CSVHostController.exe"));
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
            catch (Exception ex)
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CMonSetup.CSVHost.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "CSVHost.exe"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
            }
            catch (Exception ex)
            {
                //File.AppendAllText("log.txt", ex.Message);
            }
            progressBar1.Value = 60;
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CMonSetup.Monitor.TaskView.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "Monitor.TaskView.exe"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
            }
            catch (Exception ex)
            {
                //File.AppendAllText("log.txt", ex.Message);
            }
           //  File.AppendAllText("log.txt", "Copied new files");
            //register for the start up running
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp.GetValue("CSVHost") == null)
            {
                rkApp.SetValue("CSVHost", "C:\\Users\\Public\\RM\\Client\\CSVHost.exe");
            }
            progressBar1.Value = 100;
            //run command line
            process = new System.Diagnostics.Process();
            startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = @"C:\Windows\System32";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            string command = "/c netsh advfirewall firewall delete rule name=\"CSVHost\"";
            command += " & netsh advfirewall firewall add rule name=\"CSVHost\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Client\\CSVHost.exe\" enable=yes";
            command += " & netsh advfirewall firewall add rule name=\"MonitorView\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Client\\Monitor.TaskView.exe\" enable=yes";
            startInfo.Arguments = command;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            MessageBox.Show("Installed successfully!");
            
            
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = "C:\\Users\\Public\\RM\\Client\\CSVHost.exe";
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
