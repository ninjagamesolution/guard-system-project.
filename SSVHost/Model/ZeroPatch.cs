using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSVHost.Model
{
    public class ZeroPatch
    {

        public ZeroPatch()
        {
            Thread StartThread = new Thread(new ThreadStart(StartUp));
            StartThread.Start();
        }

        private void StartUp()
        {
            
            IPAddress m_IPAddress = GetIPAddress();

            IPEndPoint localEndPoint = new IPEndPoint(m_IPAddress, Constants.ZeroPatchPort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(localEndPoint);
                socket.Listen(100);

                Thread MainThread = new Thread(new ParameterizedThreadStart(MainStartup));
                MainThread.Start(socket);

            }
            catch (Exception e)
            {
                
            }
        }
        public void MainStartup(object obj)
        {
            Socket socket = (Socket)obj;
            bool bRun = false;
            while (true)
            {
                try
                {
                    Socket client = socket.Accept();
                    try
                    {

                        byte[] byteData = new byte[1024 * 1024 * 50];

                        while (true)
                        {
                            try
                            {
                                int recvData = client.Receive(byteData);
                                string prefix = Encoding.UTF8.GetString(byteData, 0, 4);
                                byte[] temp = new byte[recvData - 4];
                                Array.Copy(byteData, 4, temp, 0, recvData - 4);
                                if (recvData <= 0)
                                {
                                    break;
                                }
                                if (recvData > 0)
                                {
                                    string data = Encoding.UTF8.GetString(byteData, 0, recvData);
                                    string path = "C:\\Users\\Public\\";
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                    string file = "ServerSetup.exe";


                                    if (prefix == Constants.Se_Version)
                                    {

                                        string new_version = Encoding.UTF8.GetString(temp);
                                        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                                        var rk = key.OpenSubKey("RMServer");
                                        if (rk == null || rk.GetValue("Version").ToString() != new_version)
                                        {
                                            key = key.CreateSubKey("RMServer");
                                            key.SetValue("Version", new_version);
                                            if (File.Exists("C:\\Users\\Public\\SMonAutoSetup.exe"))
                                            {
                                                try
                                                {
                                                    File.Delete("C:\\Users\\Public\\SMonAutoSetup.exe");
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            if (File.Exists("C:\\Users\\Public\\ServerSetup.exe"))
                                            {
                                                try
                                                {
                                                    File.Delete("C:\\Users\\Public\\ServerSetup.exe");
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            byte[] sendData = Encoding.UTF8.GetBytes(Constants.Re_VersionOther);
                                            client.Send(sendData, sendData.Length, SocketFlags.None);
                                        }
                                        else if (rk.GetValue("Version").ToString() == new_version)
                                        {
                                            byte[] sendData = Encoding.UTF8.GetBytes(Constants.Re_VersionSame);
                                            client.Send(sendData, sendData.Length, SocketFlags.None);
                                            break;
                                        }
                                    }
                                    else if (prefix == Constants.Se_FileEnd)
                                    {
                                        RunProcess();
                                        client.Close();
                                        break;
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
                                //socket.Close();
                                break;
                            }

                        }
                        try
                        {
                            client.Close();
                        }
                        catch
                        {

                        }
                        Thread.Sleep(150);
                        if(bRun == true)
                        {
                            Process process = new Process();
                            try
                            {
                                process.StartInfo.FileName = "C:\\Users\\Public\\ServerSetup.exe";
                                process.StartInfo.UseShellExecute = true;
                                process.Start();
                                process.WaitForExit();
                                bRun = false;
                               
                            }
                            catch (Exception ex)
                            {
                                //File.AppendAllText("csvhostlog.txt", ex.Message);
                            }
                        }
                        break;

                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            try
            {
                socket.Close();
                Thread m = new Thread(new ThreadStart(StartUp));
                m.Start();
            }
            catch
            {
                
            }
        }

        private void RunProcess()
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
                //File.AppendAllText("csvhostlog.txt", ex.Message);
            }
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
