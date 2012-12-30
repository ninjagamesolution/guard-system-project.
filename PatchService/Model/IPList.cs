using PatchService.Globals;
using PatchService.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PatchService.Model
{
    public class IPList
    {
        public static string start_ip { get; set; }
        public static string end_ip { get; set; }
        public static List<IPItem> ipList = new List<IPItem>();
        public bool bFlag = false;
        static object lockObj = new object();
        Stopwatch stopWatch = new Stopwatch();
        public IPList(string str_startIP, string str_endIP)
        {
            start_ip = str_startIP;
            end_ip = str_endIP;
            //RunPingSweep_Async();
        }

        public async void RunPingSweep_Async()
        {
            var tasks = new List<Task>();
            var startIP = IPAddress.Parse(start_ip);
            var endIP = IPAddress.Parse(end_ip);
            var IPRange = new IPAddressRange(startIP, endIP);
            //TimeSpan ts;
            //stopWatch.Start();
            foreach (var item in IPRange)
            {
                Ping p = new Ping();
                var task = PingAndUpdateAsync(p, item.ToString());
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ContinueWith(t =>
            {
                stopWatch.Stop();
                //long second = stopWatch.ElapsedMilliseconds;
                //File.WriteAllText("ping1.log", "ping" + second);
            });
            

        }
        private async Task PingAndUpdateAsync(Ping ping, string ip)
        {
            //this.AVARTA.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Connect_On.png"));
            var reply = await ping.SendPingAsync(ip, 10);
            if(reply.Status == IPStatus.Success)
            {
                
                ipList.Add(new IPItem { IPAddress = ip, Status = "DisConnected", During="0", IsChecked=false, IconImage = "pack://application:,,,/Resource/connect.png" });
                
            }
        }

        public List<IPItem> GetRunIPAddress()
        {
            
            return ipList;
           

        }
    }
}
