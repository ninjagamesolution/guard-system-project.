using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSVHost.Model
{
    public class UsbDetect
    {
        public UsbDetect()
        {
            Thread UsbThread = new Thread(new ThreadStart(UsbDetectThread));
            UsbThread.Start();
        }

        private void UsbDetectThread()
        {
            UsbState usb = new UsbState();
            BackgroundWorker bgwDriveDetector = new BackgroundWorker();
            bgwDriveDetector.DoWork += usb.bgwDriveDetector_DoWork;
            bgwDriveDetector.RunWorkerAsync();
            bgwDriveDetector.WorkerReportsProgress = true;
            bgwDriveDetector.WorkerSupportsCancellation = true;
        }

    }
    public class UsbState
    {

        public UsbState()
        {

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

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                string strData = "";
                strData = GetIPAddress().ToString();
                IPAddress serverIP = IPAddress.Parse(Constants.AutoPatchServerIP);
                IPEndPoint localEndPoint = new IPEndPoint(serverIP, Constants.USBPort);
                socket.Connect(localEndPoint);
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                foreach (var property in instance.Properties)
                {
                    if (property.Name == "DeviceID")
                    {
                        strData += ("*" + property.Value);
                    }
                    if (property.Name == "SystemName")
                    {
                        strData += ("*" + property.Value);
                    }
                }
                strData += "*" + "Insert" + "*" + DateTime.Now.ToShortDateString();
                byte[] data = Encoding.UTF8.GetBytes(Constants.Re_UsbDetect + strData);
                socket.Send(data);
                Thread.Sleep(1000);
                socket.Send(Encoding.UTF8.GetBytes(Constants.Re_UsbEnd));
                socket.Close();
            }
            catch
            {
                socket.Close();
            }

        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                string strData = "";
                strData = GetIPAddress().ToString();
                IPAddress serverIP = IPAddress.Parse(Constants.AutoPatchServerIP);
                IPEndPoint localEndPoint = new IPEndPoint(serverIP, Constants.USBPort);
                socket.Connect(localEndPoint);
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                foreach (var property in instance.Properties)
                {
                    if (property.Name == "DeviceID")
                    {
                        strData += ("*" + property.Value);
                    }
                    if (property.Name == "SystemName")
                    {
                        strData += ("*" + property.Value);
                    }
                }
                strData += "*" + "Remove" + "*" + DateTime.Now.ToShortDateString();
                byte[] data = Encoding.UTF8.GetBytes(Constants.Re_UsbDetect + strData);
                socket.Send(data);
                Thread.Sleep(1000);
                socket.Send(Encoding.UTF8.GetBytes(Constants.Re_UsbEnd));
                socket.Close();
            }
            catch
            {
                socket.Close();
            }
        }

        public void bgwDriveDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();
        }
    }
}
