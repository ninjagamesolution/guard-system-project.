using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public class FileSend
    {
        public Socket m_socket;
        public int FIle_Size = 0;
        public string date = "";
        public string speed = "";
        public bool bflag = false;
        public FileSend()
        {
            Thread thread = new Thread(new ThreadStart(StartUp));
            thread.Start();
        }

        private void StartUp()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress m_IPAddress = GetIPAddress();
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localPoint = new IPEndPoint(m_IPAddress, Constants.FilePort);
            m_socket.Bind(localPoint);
            m_socket.Listen(10);
            Thread thread = new Thread(new ThreadStart(AccepThread));
            thread.Start();
        }

        private void AccepThread()
        {
            try
            {
                while (true)
                {
                    Socket socket = m_socket.Accept();
                    Thread thread = new Thread(new ParameterizedThreadStart(ReceiveThread));
                    thread.Start(socket);
                }
            }
            catch
            {

            }
        }

        private void ReceiveThread(object obj)
        {
            Socket socket = (Socket)obj;
            byte[] byteData = new byte[1024 * 1024];
            while (true)
            {
                try
                {

                    int size = socket.Receive(byteData);
                    string prefix = Encoding.UTF8.GetString(byteData, 0, 4);
                    byte[] temp = new byte[size - 4];
                    Array.Copy(byteData, 4, temp, 0, size - 4);
                    if (prefix == Constants.Se_AutoFileInfo)
                    {
                        if (bflag == true)
                        {
                            socket.Send(Encoding.UTF8.GetBytes(Constants.Re_FileAlready));
                        }
                        else
                        {
                            string data = Encoding.UTF8.GetString(temp);
                            string[] list = data.Split('*');
                            date = list[1];
                            speed = list[0];
                            Thread thread = new Thread(new ParameterizedThreadStart(SendFile));
                            thread.Start(socket);
                        }

                    }

                }
                catch
                {
                    socket.Close();
                    break;
                }
            }

        }

        private void SendFile(object obj)
        {
            Socket socket = (Socket)obj;

            int bytePerSecond = Convert.ToInt32(speed) * 1000;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            var rk = key.OpenSubKey(Constants.RegPath);
            string baseFolder = rk.GetValue("BaseDirectory").ToString();
            string captureFolder = Path.Combine(baseFolder, date);

            while (true)
            {
                try
                {
                    string[] directoryList = Directory.GetDirectories(captureFolder);
                    foreach(var directory in directoryList)
                    {
                        socket.Send(Encoding.UTF8.GetBytes(Constants.Re_DrectoryName + Path.GetFileName(directory)));
                        Thread.Sleep(1000);
                        string[] fileList = Directory.GetFiles(Path.Combine(directory, "Capture"));
                        foreach (var file in fileList)
                        {
                            string FileName = Path.GetFileName(file);
                            socket.Send(Encoding.UTF8.GetBytes(Constants.Re_FileName + FileName.Substring(0, FileName.Length - 4)));
                            Thread.Sleep(1000);
                            byte[] byteData = File.ReadAllBytes(file);
                            int length = byteData.Length;
                            byte[] sendData = new byte[length - 4];
                            Array.Copy(byteData, 4, sendData, 0, length - 4);

                            int realLength = sendData.Length;
                            int position = 0;
                            if (bytePerSecond > length)
                            {
                                socket.Send(sendData);
                            }
                            else
                            {
                                while (realLength > 0)
                                {
                                    if (realLength <= 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        byte[] temp;
                                        if (realLength > bytePerSecond)
                                        {
                                            temp = new byte[bytePerSecond];
                                            Array.Copy(sendData, position, temp, 0, bytePerSecond);
                                            socket.Send(temp, bytePerSecond, SocketFlags.None);
                                        }
                                        else
                                        {
                                            temp = new byte[realLength];
                                            Array.Copy(sendData, position, temp, 0, realLength);
                                            socket.Send(temp, realLength, SocketFlags.None);
                                        }

                                        position += bytePerSecond;
                                        realLength -= bytePerSecond;
                                    }
                                    Thread.Sleep(1000);
                                }
                            }
                            socket.Send(Encoding.UTF8.GetBytes(Constants.Re_FileEnd));
                        }
                    }
                    Thread.Sleep(3000);
                    socket.Send(Encoding.UTF8.GetBytes(Constants.Re_End));
                    Thread.Sleep(500);
                    bflag = false;
                    socket.Close();
                    break;
                }
                catch
                {
                    socket.Close();
                    break;
                }
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
