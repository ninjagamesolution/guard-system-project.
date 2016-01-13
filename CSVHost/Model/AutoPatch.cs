using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CSVHost.Model
{
    public class AutoPatch
    {
        public string current_version = "";
        public string strSysCreated = "";
        public AutoPatch()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key = key.OpenSubKey("RMClient");
                current_version = key.GetValue("Version").ToString();
                key.Close();
            }
            catch
            {

            }

            Run();

            System.Timers.Timer timer = new System.Timers.Timer(Constants.AutoPatchLoop);
            timer.Elapsed += AutoPatchTimerCallback;
            timer.AutoReset = true;
            timer.Enabled = true;


        }

        
        private void StartUp()
        {
            //Thread thred = new Thread(new ThreadStart(AutoPatchTimerCallback));
            //while (true)
            //{
            //    thred.Start();
            //    Thread.Sleep(1000 * 60 * 1);
            //}

        }

        public void Run()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress serverIP = IPAddress.Parse(Constants.AutoPatchServerIP);
                byte[] byteData = new byte[1024 * 1024];
                bool bRun = false;
                try
                {
                    socket.Connect(serverIP, Constants.PatchPort);
                    string IP = GetIPAddress().ToString();

                    string data = IP + "*" + current_version;
                    socket.Send(Encoding.UTF8.GetBytes(data));
                    string path = "C:\\Users\\Public\\";
                    string file = "ClientSetup.exe";
                    Thread.Sleep(500);
                    while (true)
                    {
                        int size = 0;
                        if ((size = socket.Receive(byteData)) > 0)
                        {
                            string prefix = Encoding.UTF8.GetString(byteData, 0, 4);
                            byte[] temp = new byte[size - 4];
                            Array.Copy(byteData, 4, temp, 0, size - 4);
                            if (prefix == Constants.Se_Version)
                            {
                                break;
                            }
                            else if (prefix == Constants.Se_AutoVersion)
                            {
                                if (File.Exists(Path.Combine(path, file)))
                                {
                                    File.Delete(Path.Combine(path, file));
                                }
                                string new_version = Encoding.UTF8.GetString(temp);
                                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                                key = key.CreateSubKey("RMClient");
                                key.SetValue("Version", new_version);
                                key.Close();
                            }
                            else
                            {
                                using (FileStream stream = new FileStream(Path.Combine(path, file), FileMode.Append))
                                {
                                    stream.Write(byteData, 0, size);
                                }
                                bRun = true;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                    socket.Close();

                }
                catch
                {
                    socket.Close();
                }
                try
                {
                    if (bRun == true)
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
                        }
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }
        }

        private void AutoPatchTimerCallback(Object source, ElapsedEventArgs e)
        {

            Run();


        }

        public static IPAddress GetIPAddress()
        {
            IPAddress m_IPAddress = null;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (ni.Name.StartsWith("Ethernet"))
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                m_IPAddress = ip.Address;
                                break;
                            }
                        }
                    }

                }
            }
            return m_IPAddress;
        }
    }
}
