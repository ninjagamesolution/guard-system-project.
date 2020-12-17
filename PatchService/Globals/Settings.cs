using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net.Sockets;
//using Monitor.TaskControl.Models;
//using Monitor.TaskControl.Communication;

namespace PatchService.Globals
{
    [Serializable]
    public class Settings
    {
    //    private static Timer SaveTimer { get; set; }
    //    public static bool IsMetro = false;
    //    public SettingsDirectories Directories { get; private set; }
    //    public RegisterValue RegValue { get; set; }
    //    public Dictionary<string, ClientInfo> ClientDic { get; set; }
    //    public Dictionary<string, ClientInfo> ClientDic_Temp { get; set; }
    //    public List<CustomMessage> MessageList { get; set; }
    //    public Dictionary<Socket, handleClient> SocketDic { get; set; }
    //    public ClientInfo clientInfo { get; set; }

    //    public Dictionary<string, string> Forbiddenprocess_list { get; set; }

    //    public List<string> AllowURLList;

    //    public List<string> DownloadList;

    //    public static object _syncMsgLock = new object();
    //    public static object _syncHistoryLock = new object();

    //    public System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

    //    private static Settings _instance;
    //    public static Settings Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                _instance = new Settings();
    //            }

    //            return _instance;
    //        }
    //    }
    //    static Settings()
    //    {
    //        SaveTimer = new Timer(60000);
    //        SaveTimer.Elapsed += OnSaveTimerElapsed;
    //        SaveTimer.Start();
    //    }
    //    private static void OnSaveTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    //    {
    //        Save();
    //    }

    //    public static void Save()
    //    {

    //    }

    //    public static void Load()
    //    {

    //    }
    //    private Settings()
    //    {
    //        RegValue = new RegisterValue();
    //        Directories = new SettingsDirectories();
    //        ClientDic = new Dictionary<string, ClientInfo>();
    //        ClientDic_Temp = new Dictionary<string, ClientInfo>();
    //        SocketDic = new Dictionary<Socket, handleClient>();
    //        MessageList = new List<CustomMessage>();
    //        Directories.Verify();
    //        Forbiddenprocess_list = new Dictionary<string, string>();
    //        AllowURLList = new List<string>();
    //        DownloadList = new List<string>();
    //    }
    //}
    
    //public class RegisterValue
    //{
    //    public string BaseDirectory { get; set; }
    //    public string Password { get; set; }
    //    public int MessageListCount { get; set; }
    //    public int DelDataDay { get; set; }

    //    public  void OnInIt()
    //    {
    //        BaseDirectory = Constants.BaseDirectory;
    //        Password = Constants.InitPassword;
    //        MessageListCount = Constants.MessageListCount;
    //        DelDataDay = Constants.DelDataDay;
    //    }

    //    public RegisterValue()
    //    {
    //        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
    //        var rk = key.OpenSubKey(Constants.RegPath);
    //        if (rk == null)
    //        {
    //            OnInIt();
    //        }
    //        else if ((string)rk.GetValue("BaseDirectory") != null)
    //        {
    //            try
    //            {
    //                BaseDirectory = (string)rk.GetValue("BaseDirectory");

    //                Password = (string)rk.GetValue("Password");

    //                MessageListCount = (int)rk.GetValue("MessageListCount");

    //                DelDataDay = (int)rk.GetValue("DelDataDay");
    //            }
    //            catch
    //            {
    //                OnInIt();
    //            }
    //        }
    //    }
    //    public void WriteValue()
    //    {
    //        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
    //        key = key.CreateSubKey(Constants.RegPath);
    //        key.SetValue("BaseDirectory", BaseDirectory);
    //        key.SetValue("Password", Password);
    //        key.SetValue("MessageListCount", MessageListCount);
    //        key.SetValue("DelDataDay", DelDataDay);
    //        key.Close();
    //    }
    //    public bool ExistsGetValue()
    //    {
    //        try
    //        {
    //            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
    //            var rk = key.OpenSubKey(Constants.RegPath);
    //            if (rk == null)
    //                return false;
    //            BaseDirectory = (string)rk.GetValue("BaseDirectory");
    //            if (BaseDirectory == null)
    //                return false;
                
    //            Password = (string)rk.GetValue("Password");

    //            MessageListCount = (int)rk.GetValue("MessageListCount");

    //            DelDataDay = (int)rk.GetValue("DelDataDay");
    //        }
    //        catch
    //        {
    //            OnInIt();
    //        }

    //        return true;
    //    }

    //}

    //public class ClientInfo
    //{
    //    public string ClientIP { get; set; }
    //    public string UserName { get; set; }
    //    public string Password { get; set; }
    //    public int SessionTime { get; set; }
    //    public int ActiveDuration { get; set; }
    //    public int CaptureTime { get; set; }
    //    public string Company { get; set; }
    //    public int SlideWidth { get; set; }
    //    public int SlideHeight { get; set; }
    //    public int CaptureWidth { get; set; }
    //    public int CaptureHeight { get; set; }
    //    public bool NetworkState { get; set; }
    //    public string PCName { get; set; }
    //    public string OSDate { get; set; }
    //    public bool bFirst { get; set; }
    //    public bool bFirstUrl { get; set; }

    //    public ListOfProcessByOrder LOPBO = new ListOfProcessByOrder();
    //    public List<ListOfProcessByOrder> ProcessList = new List<ListOfProcessByOrder>();

    //    public ListOfUrl LOU = new ListOfUrl();
    //    public List<ListOfUrl> URLList = new List<ListOfUrl>();
    //    public ClientInfo()
    //    {
    //        ClientIP = "127.0.0.1";
    //        NetworkState = false;
    //        UserName = "";
    //        Password = "";
    //        Company = "Single";
    //        SessionTime = Constants.SessionTime;
    //        ActiveDuration = Constants.ActiveDuration;
    //        CaptureTime = Constants.CaptureTime;
    //        SlideWidth = Constants.SlideWidth;
    //        SlideHeight = Constants.SlideHeight;
    //        CaptureWidth = Constants.CaptureWidth;
    //        CaptureHeight = Constants.CaptureHeight;
    //        PCName = "";
    //        OSDate = "";
    //        bFirst = true;
    //        bFirstUrl = true;
    //    }
    //}
    //public class SettingsDirectories
    //{
    //    public string TodayDirectory { get; set; }
    //    public string WorkDirectory { get; set; }
    //    public string SlideDirectory { get; set; }
    //    public string CaptureDirectory { get; set; }
    //    public string CurrentDirectory
    //    {
    //        get { return AppDomain.CurrentDomain.BaseDirectory; }
    //    }
    //    public string SetupPath
    //    {
    //        get { return Constants.SetupPath; }
    //    }


    //    internal SettingsDirectories()
    //    {
    //        //    CreateDirectories();
    //    }


    //    internal void Verify()
    //    {

    //        //   CreateDirectories();
    //    }

    //    public void CreateDirectories()
    //    {
    //        DateTime localDate = DateTime.Now;
    //        WorkDirectory = Settings.Instance.RegValue.BaseDirectory;
    //        if (!Directory.Exists(WorkDirectory))
    //            Directory.CreateDirectory(WorkDirectory);

    //        string today = localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString();
    //        TodayDirectory = Path.Combine(WorkDirectory, today);
    //        if (!Directory.Exists(TodayDirectory))
    //            Directory.CreateDirectory(TodayDirectory);

    //        SlideDirectory = Path.Combine(TodayDirectory, "Slide");
    //        if (!Directory.Exists(SlideDirectory))
    //            Directory.CreateDirectory(SlideDirectory);

    //        CaptureDirectory = Path.Combine(TodayDirectory, "Capture");
    //        if (!Directory.Exists(CaptureDirectory))
    //            Directory.CreateDirectory(CaptureDirectory);
    //    }
    }

    public struct ListOfProcessByOrder
    {
        public string ProcessName;
        public string ProcessWindow;
        public string ProcessPath;
        public string ProcessColor;
        public DateTime ProcessStartTime;
        public DateTime ProcessEndTime;
    }

    public struct ListOfUrl
    {
        public byte BrowserType;
        public string strWindow;
        public string strURL;
        public DateTime URLStartTime;
        public DateTime URLEndTime;
    }

    public class CustomMessage
    {
        public string index { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public string clientIP { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public bool state { get; set; }
    }
    public class USBModel
    {
        public string ClientIP { get; set; }
        public string USBID { get; set; }
        public string USBType { get; set; }
        public string Event { get; set; }
        public string Date { get; set; }
        public string SystemDate { get; set; }
    }
    
}
