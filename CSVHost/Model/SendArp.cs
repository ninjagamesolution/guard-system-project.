using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public class SendArp
    {
        public string strSysCreated = "";
        public string ipAddress = "";
        public SendArp()
        {
            strSysCreated = getSystemInstalledDate() + getPartitioninformation();
            //string[] ips = GetIPAddress().ToString().Split('.');
            //if (ips[3].Length == 1) ips[3] = "00" + ips[3];
            //if (ips[3].Length == 2) ips[3] = "0" + ips[3];
            //ipAddress = string.Join(".", ips);

            ipAddress = GetIPAddress().ToString();

            Sending();
            System.Timers.Timer timer = new System.Timers.Timer(Constants.ArpInfoLoop);
            timer.Elapsed += AutoSendArpProc;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void AutoSendArpProc(object sender, ElapsedEventArgs e)
        {
            Sending();
        }

        private void Sending()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress serverIP = IPAddress.Parse(Constants.AutoPatchServerIP);
            try
            {
                socket.Connect(serverIP, Constants.ArpInfoPort);
                string arpData = $"{ipAddress}*client*{strSysCreated}";
                socket.Send(Encoding.UTF8.GetBytes(arpData));
                using (StreamWriter sw = File.AppendText("c:\\logc.txt"))            /// last will change this part ***
                {
                    sw.WriteLine(arpData + "---" + DateTime.Now.ToString());
                }
            }
            catch
            {
                socket.Close();
            }
            finally
            {
                socket.Close();
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

        public string getSystemInstalledDate()
        {
            var key = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            key = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
            if (key != null)
            {
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                object objValue = key.GetValue("InstallDate");
                string stringValue = objValue.ToString();
                Int64 regVal = Convert.ToInt64(stringValue);

                DateTime installDate = startDate.AddSeconds(regVal);
                return installDate.ToString();
            }
            return DateTime.MinValue.ToString();
        }
        public string getPartitioninformation()
        {
            int Primary = 0, BootOS = 0, Hidden = 0;
            SelectQuery wmi_dp = new SelectQuery("Select * from Win32_DiskPartition");
            ManagementObjectSearcher wmi_mbs = new ManagementObjectSearcher(wmi_dp);
            foreach (ManagementObject wmi_pros in wmi_mbs.Get())
            {
                if ((bool)wmi_pros["Bootable"])
                {
                    BootOS++;
                }
                if ((bool)wmi_pros["PrimaryPartition"])
                {
                    Primary++;
                }
                if (wmi_pros["Type"].ToString().Equals("Unknown"))
                {
                    Hidden++;
                }

            }
            string pcName = Environment.MachineName;
            return $"*{BootOS}*{Primary}*{Hidden}*{pcName}";
        }



    }
}
