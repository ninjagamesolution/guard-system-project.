using Microsoft.Win32;
using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace Monitor.TaskControl.Models
{
    public class PatchProc
    {
        public PatchProc()
        {
            
            //Timer AutoPatchTimer = new Timer(AutoPatchTimerCallback, null, 0, 1000 *10);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 5, 5);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            var rk = key.OpenSubKey("RMServer");
            string new_version = "";
            if (rk == null)
            {
                key = key.CreateSubKey("RMClient");
                key.SetValue("Version", "1.0");
            }
            else
            {
                new_version = rk.GetValue("Version").ToString();
            }
            if (new_version != Constants.current_version)
            {
                Windows.MainWindow.ShowAlarm();
            }
            
        }
        public void DownloadPatch()
        {
            Thread thread = new Thread(new ThreadStart(StartUp));
            thread.Start();
            
        }

        private void StartUp()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress serverIP = IPAddress.Parse(Constants.AutoPatchServerIP);
            byte[] byteData = new byte[1024 * 1024];
            bool bRun = false;
            try
            {
                socket.Connect(serverIP, Constants.AutoPatchPort);
                while (true)
                {
                    try
                    {
                        string path = "C:\\Users\\Public\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string file = "ServerSetup.exe";
                        int recvData = socket.Receive(byteData);
                        if (recvData <= 0)
                        {
                            socket.Close();
                            break;
                        }
                        if (recvData > 0)
                        {
                            string prefix = Encoding.UTF8.GetString(byteData, 0, 4);
                            byte[] temp = new byte[recvData - 4];
                            Array.Copy(byteData, 4, temp, 0, recvData - 4);
                            if (prefix == Constants.Se_AutoVersion)
                            {
                                string version = Encoding.UTF8.GetString(temp);
                                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                                var rk = key.OpenSubKey("RMServer");
                                if (rk == null)
                                {
                                    rk = key.CreateSubKey("RMServer");
                                    
                                }
                                rk.SetValue("Version", version);

                            }
                            else
                            {
                                using (FileStream stream = new FileStream(Path.Combine(path, file), FileMode.Append))
                                {
                                    stream.Write(byteData, 0, recvData);
                                }
                                bRun = true;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        socket.Close();
                        break;
                    }
                }
                if (bRun == true)
                {
                    Process process = new Process();
                    try
                    {
                        process.StartInfo.FileName = "C:\\Users\\Public\\ServerSetup.exe";
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            catch
            {
                socket.Close();
            }
        }
    }
}
