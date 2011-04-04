using PatchService.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatchService.Model
{
    public class ZeroPatchProc
    {
        public string strPath;
        List<IPItem> IPList = new List<IPItem>();
        public byte[] byteSend;
        public int nPatchPort;
        public string strVersion;
        public ZeroPatchProc(string path, List<IPItem> list, int port, string version)
        {
            strPath = path;
            IPList = list;
            nPatchPort = port;
            strVersion = version;
        }

        public void StartUp()
        {
            byteSend = File.ReadAllBytes(strPath);

            //Stopwatch total = new Stopwatch();
            //total.Start();
            foreach (var item in IPList)
            {
                if (item.IsChecked == true)
                {
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(SendFile), item);
                    Thread thread = new Thread(new ParameterizedThreadStart(SendFile));
                    thread.Start(item);
                    Thread.Sleep(3000);
                }
            }
        }
        private void SendFile(object state)
        {
            IPItem item = (IPItem)state;
            var ip = IPAddress.Parse(item.IPAddress);
            byte[] recvBuff = new byte[1024];
            IPEndPoint endPoint = new IPEndPoint(ip, nPatchPort);
            Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //Stopwatch time = new Stopwatch();
            //time.Start();
            try
            {


                clientSock.Connect(endPoint);

                byte[] sendVersion = Encoding.ASCII.GetBytes(Constants.Se_Version + strVersion.Trim());
                clientSock.Send(sendVersion);
                while (true)
                {
                    int dataByte = clientSock.Receive(recvBuff, SocketFlags.None);
                    string vcheck = Encoding.UTF8.GetString(recvBuff, 0, dataByte);
                    if (vcheck == Constants.Re_VersionSame)
                    {

                        item.Status = "Installed";
                        ////time.Stop();
                        ////var druation = time.ElapsedMilliseconds;
                        //if (druation > 1000)
                        //{
                        //    druation = druation / 1000;
                        //    item.During = druation.ToString() + "s";
                        //}
                        //item.During = druation.ToString() + "ms";
                        //item.IconImage = "pack://application:,,,/Resource/same.png";
                        //ipList.Add(item);
                        clientSock.Close();

                        Console.WriteLine(ip + ":Same");
                        break;
                    }
                    else
                    {

                        clientSock.Send(byteSend);
                        clientSock.Close();

                        //item.Status = "Patch Success";
                        //time.Stop();
                        ////var druation = time.ElapsedMilliseconds;
                        //if (druation > 1000)
                        //{
                        //    druation = druation / 1000;
                        //    item.During = druation.ToString() + "s";
                        //}
                        //item.During = druation.ToString() + "ms";
                        //item.IconImage = "pack://application:,,,/Resource/success.png";
                        Console.WriteLine(ip + ":Success");
                        break;
                        //ipList.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {

                clientSock.Close();
                //item.Status = "Patch Failure";
                //time.Stop();
                //var druation = time.ElapsedMilliseconds;
                //if (druation > 1000)
                //{
                //    druation = druation / 1000;
                //    item.During = druation.ToString() + "s";
                //}
               // item.During = druation.ToString() + "ms";
                //item.IconImage = "pack://application:,,,/Resource/fail.png";
                Console.WriteLine(ip + ":Fail");
                Console.WriteLine(ip + ":" + ex.Message);
            }
            //Thread.Sleep(500);
            //this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            //{
            //    ValidList.ItemsSource = null;
            //    ValidList.ItemsSource = myList;
            //});
           //Thread.Sleep(300);
        }
    }
}
