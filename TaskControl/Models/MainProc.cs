using System;
using System.IO;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.View;
using Monitor.TaskControl.Utils;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;

namespace Monitor.TaskControl.Models
{
    public class MainProc
    {
        private static System.Timers.Timer CheckTimer { get; set; }
        public static Thread InspectThread { get; set; }
        public string strServerIP = "";
        System.Timers.Timer InspectTimer;
        //public ScreenCaptures ScrCapture;
        //public SaveProcessInfo PrcInfo;        

        private static MainProc _instance;
        private int m_TimeCount;
        public static MainProc Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainProc();
                }

                return _instance;
            }
        }
        private MainProc()
        {
            //ScrCapture = new ScreenCaptures();
            //PrcInfo = new SaveProcessInfo();

            
        }       

        public string GetIPAddress(Socket socket)
        {
            return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        }
        static MainProc()
        {
            
        }
        public void AllProcessStart()
        {
            Settings.Instance.IsSending = false;
            //ScrCapture = new ScreenCaptures();
            //PrcInfo = new SaveProcessInfo();
            //InspectThread = new Thread(new ThreadStart(Inspect));
            //InspectThread.Start();
            DispatcherTimer dtClockTime = new DispatcherTimer();
            dtClockTime.Interval = new TimeSpan(0, 1, 0); //in Hour, Minutes, Second.
            dtClockTime.Tick += TimerTick;
            dtClockTime.Start();
            m_TimeCount = 0;

            Settings.Instance.CheckedDates.Clear();

            LoadRestInspect();

            InspectTimer = new System.Timers.Timer(30 * 60 * 1000);
            InspectTimer.Elapsed += InspectTimerCallback;
            InspectTimer.Start();

            //            Thread thread = new Thread(new ThreadStart(DoWork));
            //            thread.Start();
        }

        private void LoadRestInspect()
        {

            if (!File.Exists(Globals.Constants.BaseDirectory + "\\" + Globals.Constants.RestInspectFileName))
            {
                return;
            }
            Settings.Instance.CheckedDates = File.ReadAllLines(Globals.Constants.BaseDirectory + "\\" + Globals.Constants.RestInspectFileName).ToList();
        }

        private void InspectTimerCallback(Object source, ElapsedEventArgs e)
        {

            int totCnt = 0, percent = 0, checkedCnt = 0;

            foreach (KeyValuePair<string, ClientInfo> entry in Settings.Instance.ClientDic)
            {
                totCnt++;
                
                string strPath = Settings.Instance.Directories.TodayDirectory + "\\" + entry.Key + "\\Capture";

                if (Directory.Exists(strPath))
                {
                    entry.Value.TotalCaptureCount = Directory.GetFiles(strPath, "*", SearchOption.TopDirectoryOnly).Length;
                }
                else
                {
                    entry.Value.TotalCaptureCount = 0;
                }
                    

                if (entry.Value.TotalCaptureCount == 0 || entry.Value.ReadCaptureCount == 0)
                {
                    continue;
                }

                percent += entry.Value.ReadCaptureCount * 100 / entry.Value.TotalCaptureCount;
                checkedCnt++;
            }

            if (totCnt == 0 )
            {
                percent = 0;
            }
            else
            {
                percent /= totCnt;
            }

            

            while (true)
            {
                if (Settings.Instance.IsSending) continue;

                Settings.Instance.IsSending = true;

                try
                {
                    DateTime curDate = DateTime.Now;
                    string strCurDate = string.Format("{0}-{1}-{2}", curDate.Year, curDate.Month, curDate.Day);

                    string strSend = strCurDate + "*" + percent + "%" + "*" + checkedCnt + "/" + totCnt;
                    

                    Thread InspectThread = new Thread(new ParameterizedThreadStart(Inspect));
                    InspectThread.Start(strSend);
                    break;
                }
                catch
                {
                    //Settings.Instance.InspectSocket.Dispose();
                    //Thread InspectThread = new Thread(new ThreadStart(Inspect));
                    //InspectThread.Start();
                }

                
            }

            if (Settings.Instance.CheckedDates == null || Settings.Instance.CheckedDates.Count == 0)
            {
                return;
            }
            
            foreach (string strDate in Settings.Instance.CheckedDates)
            {
                DateTime curDate = DateTime.Now;
                string strCurDate = string.Format("{0}-{1}-{2}", curDate.Year, curDate.Month, curDate.Day);

                if (strDate == strCurDate)
                {
                    continue;
                }

                totCnt = percent = checkedCnt = 0;

                List<string> strListTemp = new List<string>();
                string strFileName = Settings.Instance.Directories.WorkDirectory + @"\" + strDate + @"\" + Constants.UserFileName;
                if (!File.Exists(strFileName))
                {
                    strFileName = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\UserInformation.lib";
                    if (File.Exists(strFileName))
                    {
                        strListTemp = File.ReadAllLines(strFileName).ToList();
                    }
                    else
                    {
                        strFileName = Settings.Instance.Directories.WorkDirectory + "\\UserInformation.lib";
                        if (File.Exists(strFileName))
                        {
                            strListTemp = File.ReadAllLines(strFileName).ToList();
                        }
                    }
                }
                else
                {
                    strListTemp = Md5Crypto.ReadCryptoFile(strFileName);
                }
                foreach (string user in strListTemp)
                {
                    totCnt++;
                    if (user == "")
                    {
                        continue;
                    }
                    string[] items = user.Split('*');
                    ClientInfo client1 = new ClientInfo()
                    {
                        ClientIP = items[0],
                        UserName = items[1],
                        Password = items[2],
                        Company = items[3],
                        SessionTime = Convert.ToInt32(items[4]),
                        CaptureTime = Convert.ToInt32(items[5]),
                        SlideWidth = Convert.ToInt32(items[6]),
                        SlideHeight = Convert.ToInt32(items[7]),
                        CaptureWidth = Convert.ToInt32(items[8]),
                        CaptureHeight = Convert.ToInt32(items[9]),
                        PCName = items[10],
                        OSDate = items[11]
                    };

                    List<string> checkedList = new List<string>();

                    strFileName = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\" + Constants.InspectFileName;

                    if (File.Exists(strFileName))
                    {
                        checkedList = Md5Crypto.ReadCryptoFile(strFileName);
                    }

                    int readCaptureCount = checkedList.Count, totCaptureCount = 0;

                    if (Directory.Exists(Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\Capture"))
                    {
                        totCaptureCount = Directory.GetFiles(Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\Capture", "*", SearchOption.TopDirectoryOnly).Length;
                    }
                    else
                    {
                        totCaptureCount = 0;
                    }

                    if (readCaptureCount == 0 || totCaptureCount == 0)
                    {
                        continue;
                    }
                    percent += readCaptureCount * 100 / totCaptureCount;
                    checkedCnt++;
                }

                if (totCnt == 0)
                {
                    percent = 0;
                }
                else
                {
                    percent /= totCnt;
                }

                while (true)
                {
                    if (Settings.Instance.IsSending) continue;

                    Settings.Instance.IsSending = true;

                    try
                    {
                        Thread.Sleep(1000);

                        string strSend = strDate + "*" + percent + "%" + "*" + checkedCnt + "/" + totCnt;
                        
                        Thread InspectThread = new Thread(new ParameterizedThreadStart(Inspect));
                        InspectThread.Start(strSend);
                        break;
                    }
                    catch
                    {
                        //Settings.Instance.InspectSocket.Dispose();
                        //Thread InspectThread = new Thread(new ThreadStart(Inspect));
                        //InspectThread.Start();
                    }


                }
            }

            Settings.Instance.CheckedDates.Clear();
            while (true)
            {
                try
                {
                    File.Delete(Globals.Constants.BaseDirectory + "\\" + Globals.Constants.RestInspectFileName);
                    break;

                }
                catch
                {

                }
            }
            
        }

        public void Inspect(object obj)
        {
            string strSend = (string)obj;
            strServerIP = Constants.InspectServer;
            IPAddress ipAddress = IPAddress.Parse(strServerIP);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Constants.ServerPort);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(localEndPoint);
                byte[] sendData = Encoding.UTF8.GetBytes(strSend);

                socket.Send(sendData, sendData.Length, System.Net.Sockets.SocketFlags.None);

                socket.Dispose();

                Settings.Instance.IsSending = false;
            }
            catch (Exception ex)
            {
                Settings.Instance.IsSending = false;
                //Thread thread = new Thread(new ThreadStart(Reconnect));
                //thread.Start();
            }
        }

        private void Reconnect()
        {
            //Inspect();
        }

        public void DoWork()
        {
            int nCount = 0;
            Thread.Sleep(500);

            while (true)
            {

                if (nCount % (Constants.CaptureTime / Constants.SessionTime) == 0)   //CaptureTime
                {
                    //ScrCapture = new ScreenCaptures();
                    //PrcInfo = new SaveProcessInfo();                    
                }

                if (nCount % (60 / Constants.SessionTime) == 0)  // 1 minuts
                {
                    ReplaceNewDay();
                }                

                if (nCount == 10000) nCount = 0;
                Thread.Sleep(Constants.SessionTime * 1000);
                nCount++;
            }
        }

        private static void ReplaceNewDay()
        {
            string today = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            string todayDir = Path.Combine(Settings.Instance.Directories.WorkDirectory, today);
            //List<string> _dirs = new List<string>(Directory.EnumerateDirectories(todayDir));
            string strD1 = DateTime.Now.ToShortTimeString();
            string strD2 = DateTime.Now.ToLocalTime().ToString();
            string strD3 = DateTime.Now.ToLongTimeString();

            if (!Directory.Exists(todayDir) || DateTime.Now.ToString("HH:mm") == "00:00" || DateTime.Now.ToShortTimeString() == "12:00 AM")
            {
                try
                {
                    List<string> dirs = new List<string>(Directory.EnumerateDirectories(Settings.Instance.Directories.WorkDirectory));
                    foreach (var dir in dirs)
                    {
                        string[] TempDate = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1).Split('-');
                        DateTime lastDay = new DateTime(int.Parse(TempDate[0]), int.Parse(TempDate[1]), int.Parse(TempDate[2]));
                        double daysDiff = DateTime.Today.Subtract(lastDay).TotalDays;
                        if (daysDiff > Settings.Instance.RegValue.DelDataDay && Settings.Instance.RegValue.DelDataDay > 0)
                        {
                            Directory.Delete(dir, true);
                            Thread.Sleep(150);

                        }
                    }
                    Windows.MainWindow.DeleteTryIcon();
                    EnvironmentHelper.Restart();

                    //foreach (var client in Settings.Instance.ClientDic)
                    //{
                    //    client.Value.ProcessList.Clear(); // Process list clear
                    //    client.Value.URLList.Clear();
                    //}

                    //Settings.Instance.Directories.CreateDirectories();
                    //foreach(var user in Settings.Instance.ClientDic)
                    //{
                    //    if(user.Value.NetworkState == true)
                    //    {
                    //        Windows.MainWindow.SaveUserInformation(user.Value);
                    //    }
                    //}
                    //Settings.Instance.ClientDic.Clear();
                    //if (Windows.MainWindow != null)
                    //{
                    //    Windows.MainWindow.ShowNewDay();
                    //}
                    //Windows.MainWindow.LoadUserList();
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                    Windows.MainWindow.DeleteTryIcon();
                    EnvironmentHelper.Restart();
                }
            }
            
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ReplaceNewDay();
            //if (m_TimeCount % 2 == 0)
            //    ReplaceNewDay();

            //if (m_TimeCount == 10000) m_TimeCount = 0;

            //Thread.Sleep(Constants.SessionTime * 1000);
            //m_TimeCount++;
        }

    }

        
    }
