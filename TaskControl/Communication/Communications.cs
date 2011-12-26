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
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.Logger;
using Monitor.TaskControl.Utils;
using System.Windows.Threading;
using Monitor.TaskControl.View;

namespace Monitor.TaskControl.Communication
{

    public class Communications
    {
        private static int bytePerSize = 1024;
        private static byte[] buffer = new byte[1024 * 1024 * 5];
        private static byte[] byteData = new byte[bytePerSize];
        
        public static ManualResetEvent allDone = new ManualResetEvent(false);


        public Communications()
        {

        }
        public static void StartUpSocket()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress m_IPAddress = GetIPAddress();
            IPEndPoint localEndPoint = new IPEndPoint(m_IPAddress, Constants.Port);

            Socket m_Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                m_Sock.Bind(localEndPoint);
                m_Sock.Listen(100);

                Thread thread = new Thread(new ParameterizedThreadStart(AcceptThread));
                thread.Start(m_Sock);

            }
            catch (Exception e)
            {
                CustomEx.DoExecption(Constants.exRepair, e);
                DisConnect();

            }

        }

        public static IPAddress GetIPAddress()
        {
            IPAddress m_IPAddress = null;
            
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    Log.Instance.DoLog(ni.Name, Log.LogType.Info);
                    if (ni.Name.StartsWith("Ethernet"))
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                Log.Instance.DoLog(ip.Address.ToString(), Log.LogType.Info);
                                string[] strIP = ip.Address.ToString().Split('.');
                                if (Convert.ToInt32(strIP[0]) == 192 && Convert.ToInt32(strIP[1]) == 168 )
                                {
                                    if (Convert.ToInt32(strIP[2]) >= 103 && Convert.ToInt32(strIP[2]) <= 105)
                                    {
                                        m_IPAddress = ip.Address;
                                        return m_IPAddress;
                                    }

                                    if (Convert.ToInt32(strIP[2]) >= 107 && Convert.ToInt32(strIP[2]) <= 110)
                                    {
                                        m_IPAddress = ip.Address;
                                        return m_IPAddress;
                                    }
                                    
                                    
                                }
                                
                            }
                        }
                    }

                }
            }
            return m_IPAddress;
        }


        public static void AcceptThread(Object obj)
        {
            Socket m_Sock = (Socket)obj;
            while (true)
            {
                try
                {
                    Socket socket = m_Sock.Accept();
                    try
                    {
                        string IP = GetIPAddressOfClient(socket).ToString();
                        foreach (var item in Settings.Instance.SocketDic)
                        {
                            if (item.Value.IPAddress == IP)
                            {
                                Settings.Instance.SocketDic.Remove(item.Key);
                                item.Value.Close();
                            }
                            if (Settings.Instance.SocketDic.Count == 0)
                            {
                                break;
                            }
                        }

                        handleClient client = new handleClient();
                        Settings.Instance.SocketDic.Add(socket, client);

                        client.StartClient(socket);

                        // This point UserDisPlay()
                    }
                    catch (Exception e)
                    {
                        CustomEx.DoExecption(Constants.exRepair, e);
                        Disconnect(socket);
                    }
                }
                catch(Exception ex)
                {
                    
                    
                    DisConnect();
                    m_Sock.Close();
                    StartUpSocket();
                    Thread.CurrentThread.Abort();
                }
                
            }


        }

        public static void Send(Socket socket, byte[] data)
        {
            try
            {
                socket.Send(data, SocketFlags.None);
            }
            catch (Exception e)
            {
                Disconnect(socket);
                CustomEx.DoExecption(Constants.exRepair, e);
            }
        }

        public static void Disconnect(Socket socket)
        {
            try
            {
                handleClient client = Settings.Instance.SocketDic[socket];
                string client_ip = GetIPAddressOfClient(socket);
                Settings.Instance.SocketDic.Remove(socket);
                Settings.Instance.ClientDic[client_ip].NetworkState = false;
                //Settings.Instance.ClientDic_Temp[client_ip].NetworkState = false;
                client.Close();
                socket.Close();
            }
            catch
            {

            }
            

        }

        public static void DisConnect()
        {
            try
            {
                foreach (var item in Settings.Instance.SocketDic.ToList())
                {
                    Disconnect(item.Key);
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        public static string GetIPAddressOfClient(Socket socket)
        {
            return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        }

    }
    public class handleClient
    {
        public Socket clientSocket;
        public ClientInfo clientInfo;
        public string IPAddress;
        public void StartClient(Socket socket)
        {
            try
            {
                this.clientSocket = socket;
                this.IPAddress = GetIPAddress(socket);
                Thread ctThread = new Thread(Receive);
                ctThread.Start();
            }
            catch(Exception ex)
            {

            }
            
        }
        public void Receive()
        {

            byte[] buffer = null;
            string prefix = "";
            try
            {
                while (true)
                {
                    bool bFlag = clientSocket.Poll(5 * 1000 * 60, SelectMode.SelectRead);
                    if(bFlag && clientSocket.Available == 0)
                    {
                        Close();
                        Thread.CurrentThread.Abort();
                    }

                    if (!clientSocket.Connected)
                    {
                        Close();
                        Thread.CurrentThread.Abort();
                    }
                    buffer = new byte[1024 * 1000 * 5];
                    int byteData = clientSocket.Receive(buffer, SocketFlags.None);

                    if (byteData > 0)
                    {
                        prefix = Encoding.UTF8.GetString(buffer, 0, 4);
                        if (prefix == Constants.Re_ClientInfo)
                        {
                            string strPassword = "";
                            if (Settings.Instance.RegValue.ExistsGetValue())
                            {
                                strPassword = Settings.Instance.RegValue.Password;
                            }
                            else
                            {
                                strPassword = Constants.InitPassword;
                            }
                            string strTemp = Constants.Version + ":" + strPassword + ":" + Constants.SessionTime + ":" + Constants.SlideWidth + ":" + Constants.SlideHeight + ":" + Constants.CaptureWidth + ":" + Constants.CaptureHeight + ":" + Constants.CaptureTime + ":" + DateTime.Now.ToString("MM/dd/yyyy HH-mm-ss tt"); 
                            byte[] buff = Encoding.UTF8.GetBytes(Constants.Se_ClientInfo + strTemp);
                            clientSocket.Send(buff);

                        }else if(prefix == Constants.Re_End)
                        {
                            Close();
                            Thread.CurrentThread.Abort();
                            
                        }
                        CommProc.Instance.RecDataAnalysis(buffer, byteData, GetIPAddress(clientSocket));
                    }
                }
            }
            catch (Exception e)
            {
                CustomEx.DoExecption(Constants.exRepair, e);
                Close();
                Thread.CurrentThread.Abort();
                

            }

        }

        public void Send(string strMessage)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(strMessage);
                this.clientSocket.Send(data);
            }
            catch (Exception e)
            {
                CustomEx.DoExecption(Constants.exRepair, e);
                Close();
            }

        }
        public string GetIPAddress(Socket socket)
        {
            try
            {
                return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            }
            catch
            {

            }
            return "";
        }

        public void Close()
        {
            try
            {
                string clientip = Settings.Instance.ClientDic[GetIPAddress(clientSocket)].ClientIP;
                Settings.Instance.SocketDic.Remove(clientSocket);
                Settings.Instance.ClientDic[clientip].NetworkState = false;
                clientSocket.Close();
                this.Close();
                //Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }

        }
    }
}
