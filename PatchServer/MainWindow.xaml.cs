using Microsoft.Win32;
using Monitor.PatchServer.Globals;
using Monitor.PatchServer.Model;
using Monitor.PatchServer.Utils;
using PatchServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Monitor.PatchServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public int port = 9997;
        //All List
        public List<IPItem> myList = new List<IPItem>();
        public List<IPItem> allList = new List<IPItem>();
        private OpenFileDialog m_openFileDialog;
        public string g_fileName;
        public string g_Version;
        public byte[] _recieveBuffer = new byte[1024];
        public static int PORT;
        public Socket clientSock;
        public string safe_file = "";
        public bool flag = false;
        public string g_startIP;
        public string g_endIP;
        public static int MAX_SIZE = 10;
        public MainWindow()
        {
            InitializeComponent();
            AllowMove = true;
            StartIP.Text = "Enter Start IP Address here...";
            EndIP.Text = "Enter End IP Address here...";
            PatchFile.Text = "File Browser";
            Version.Text = "Enter Version here...";
            Port.SelectedIndex = 0;
            CheckButton.IsEnabled = true;
            m_openFileDialog = new OpenFileDialog()
            {
                Filter = "Executable files (*.exe)|*.exe",
                Title = "Select executable  file"
            };
            

        }

        
        public bool AllowMove { get; set; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 0, UseAeroCaptionButtons = false });
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && AllowMove)
            {
                DragMove();
            }
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            EnvironmentHelper.ShutDown();
        }

        

        private void StartIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StartIP.Text == "Enter Start IP Address here...")
            {
                StartIP.Text = "";
            }
        }

        private void StartIP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartIP.Text))
                StartIP.Text = "Enter Start IP Address here...";
        }

        private void EndIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EndIP.Text == "Enter End IP Address here...")
            {
                EndIP.Text = "";
            }
        }

        private void EndIP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EndIP.Text))
                EndIP.Text = "Enter End IP Address here...";
        }

        private void Version_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Version.Text == "Enter Version here...")
            {
                Version.Text = "";
            }
        }

        private void Version_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Version.Text))
                Version.Text = "Enter Version here...";
        }


        private void PatchFile_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PatchFile.Text == "File Browser.")
            {
                PatchFile.Text = "";
            }
        }

        private void PatchFile_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Version.Text))
                Version.Text = "Enter Version here...";
        }

        private void Port_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Port.SelectedIndex == 0)
            {
                port = Constants.ClientZeroPort;
            }
            else
            {
                port = Constants.ServerZeroPort;
            }
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            
            g_Version = Version.Text;
            g_startIP = StartIP.Text;
            g_endIP = EndIP.Text;
            if (g_fileName == "")
            {
                CustomMsg msg = new CustomMsg("Please Select File");
                return;
            }
            if (g_startIP == ""|| g_startIP == null || g_startIP == "Enter Start IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter Start IP Address here.");
                return;
            }

            if (g_endIP == "" || g_endIP == null || g_endIP == "Enter End IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter End IP Address here.");
                return;
            }
            if (g_Version == "" || g_Version == "Enter Version here...")
            {
                CustomMsg msg = new CustomMsg("Please Enter Version");
                return;
            }
            if (flag == true)
            {
                myList.Clear();
                IPAddress start = IPAddress.Parse(g_startIP);
                IPAddress end = IPAddress.Parse(g_endIP);
                if(start.Address > end.Address)
                {
                    CustomMsg msg = new CustomMsg("Please Insert the IP Address Correctly.");
                    return;
                }
                var range = new IPAddressRange(start, end);
                foreach (var ip in range)
                {
                    IPItem temp = new IPItem { IPAddress = ip.ToString(), IsChecked = true, Status = "Connected", During = "00:00:00",IconImage= "pack://application:,,,/Resource/connect.png"};
                    myList.Add(temp);
                }
            }
            if (myList.Count == 0)
            {
                CustomMsg msg = new CustomMsg("Please Check IP");
                return;
            }
            
            Thread thread = new Thread(new ThreadStart(AllocThread));
            thread.Start();
            //ShowList();
        }
        public static byte[] byteSend;
        public void AllocThread()
        {

            try
            {
                string path = g_fileName.Replace("\\", "/");
                byteSend = File.ReadAllBytes(path);

                Stopwatch total = new Stopwatch();
                total.Start();
                foreach (var item in myList)
                {
                    if (item.IsChecked == true)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SendFile), item);
                        //ThreadPool.GetMaxThreads(out workers, out ports);
                        //ThreadPool.SetMaxThreads(30, ports);
                        Thread.Sleep(1500);
                    }
                }
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    ValidList.ItemsSource = null;
                    ValidList.ItemsSource = myList;
                });

                Thread.Sleep(1000);
                total.Stop();
                var count = myList.Where(m => m.Status == "Patch Success").Count();
                IPItem total_time = new IPItem { IPAddress ="Total count : " + myList.Count.ToString(), During = "Total time : " + (total.ElapsedMilliseconds/1000).ToString() + "s", IsChecked = true, Status = "Success : " + count.ToString() ,IconImage= "pack://application:,,,/Resource/success.png" };
                myList.Add(total_time);
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    ValidList.ItemsSource = null;
                    ValidList.ItemsSource = myList;
                });
            }
            catch
            {

            }
            
        }

        public void SendFile(Object obj)
        {
            IPItem item = (IPItem)obj;
            var ip = IPAddress.Parse(item.IPAddress);
            byte[] recvBuff = new byte[100];
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            Stopwatch time = new Stopwatch();
            time.Start();
            try
            {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                clientSock.Connect(endPoint);

                byte[] sendVersion = Encoding.ASCII.GetBytes(Constants.Se_Version + g_Version.Trim());
                clientSock.Send(sendVersion);

                int dataByte = clientSock.Receive(recvBuff);
                string vcheck = Encoding.UTF8.GetString(recvBuff, 0, dataByte);
                if (vcheck == Constants.Re_VersionSame)
                {

                    item.Status = "Installed";
                    time.Stop();
                    var druation = time.ElapsedMilliseconds;
                    if(druation > 1000)
                    {
                        druation = druation / 1000;
                        item.During = druation.ToString() + "s";
                    }
                    item.During = druation.ToString() + "ms";
                    item.IconImage = "pack://application:,,,/Resource/same.png";
                    //ipList.Add(item);
                    clientSock.Close();

                    Console.WriteLine(ip+ ":Same");
                }
                else
                {

                    clientSock.Send(byteSend);
                    clientSock.Close();

                    item.Status = "Patch Success";
                    time.Stop();
                    var druation = time.ElapsedMilliseconds;
                    if (druation > 1000)
                    {
                        druation = druation / 1000;
                        item.During = druation.ToString() + "s";
                    }
                    item.During = druation.ToString() + "ms";
                    item.IconImage = "pack://application:,,,/Resource/success.png";
                    Console.WriteLine(ip + ":Success");
                    //ipList.Add(item);
                }
            }
            catch (Exception ex)
            {

                clientSock.Close();
                item.Status = "Patch Failure";
                time.Stop();
                var druation = time.ElapsedMilliseconds;
                if (druation > 1000)
                {
                    druation = druation / 1000;
                    item.During = druation.ToString() + "s";
                }
                item.During = druation.ToString() + "ms";
                item.IconImage = "pack://application:,,,/Resource/fail.png";
                Console.WriteLine(ip + ":Fail");
                Console.WriteLine(ip + ":" + ex.Message);
            }
            Thread.Sleep(500);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ValidList.ItemsSource = null;
                ValidList.ItemsSource = myList;
            });
            Thread.Sleep(300);
        }

        

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string str_startIP = StartIP.Text;
            string str_endIP = EndIP.Text;
            
            if(str_startIP == "" || str_startIP == "Enter Start IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter Start IP Address");
                return;
            }
            if(str_endIP == "" || str_endIP == "Enter End IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter End IP Address");
                return;
            }

            IPAddress start = IPAddress.Parse(str_startIP);
            IPAddress end = IPAddress.Parse(str_endIP);
            if (start.Address > end.Address)
            {
                CustomMsg msg = new CustomMsg("Please Insert the IP Address Correctly.");
                return;
            }
            CheckButton.IsEnabled = false;
            ValidList.ItemsSource = null;
            var item =  new IPList(str_startIP, str_endIP);
            item.RunPingSweep_Async();
            Thread.Sleep(1000);
            myList.Clear();
            myList = item.GetRunIPAddress();

            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ValidList.ItemsSource = null;
                ValidList.ItemsSource = myList;
            });
           

            CheckButton.IsEnabled = true;


        }


       

        private void File_Click(object sender, RoutedEventArgs e)
        {
            if (m_openFileDialog.ShowDialog()  == true)
            {
                g_fileName = m_openFileDialog.FileName;
                safe_file = m_openFileDialog.SafeFileName;
                PatchFile.Text = g_fileName;
                
            }
        }

        private void FieldDataGridChecked(object sender, RoutedEventArgs e)
        {
            if (ValidList.ItemsSource == null)
                return;
            foreach (IPItem c in ValidList.ItemsSource)
            {
                c.IsChecked = true;
            }
        }

        private void FieldDataGridUnchecked(object sender, RoutedEventArgs e)
        {
            if (ValidList.ItemsSource == null)
                return;
            foreach (IPItem c in ValidList.ItemsSource)
            {
                c.IsChecked = false;
            }
            
        }
        public void AllThread()
        {
            

        }

       
        private void PatchSocketButton_Click(object sender, RoutedEventArgs e)
        {
            g_startIP = StartIP.Text;
            g_endIP = EndIP.Text;
            g_Version = Version.Text;
            if (g_fileName == "")
            {
                CustomMsg msg = new CustomMsg("Please Select File");
                return;
            }
            if(g_startIP == "" || g_startIP == "Enter Start IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter Start IP Address here.");
                return;
            }

            if (g_endIP == "" || g_endIP == "Enter End IP Address here...")
            {
                CustomMsg msg = new CustomMsg("Enter End IP Address here.");
                return;
            }

            if (g_Version == "" || g_Version == "Enter Version here...")
            {
                CustomMsg msg = new CustomMsg("Please Enter Version");
                return;
            }

            allList.Clear();

             Thread thread = new Thread(new ThreadStart(AllThread));
            thread.Start();
        }

        private void MaxmizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            myList.Clear();
            CheckButton.IsEnabled = false;
            flag = true;
            //allList.Clear();
            ValidList.ItemsSource = null;

        }

        private void Ping_Click(object sender, RoutedEventArgs e)
        {
            myList.Clear();
            flag = false;
            //allList.Clear();
            ValidList.ItemsSource = null;
            CheckButton.IsEnabled = true;
            
        }
    }

}
