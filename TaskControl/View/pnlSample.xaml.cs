using Monitor.TaskControl.Globals;
using Monitor.TaskControl.myEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Threading;
using Monitor.TaskControl.Logger;

using Rectangle = System.Windows.Shapes.Rectangle;
using Color = System.Windows.Media.Color;
using System.Drawing;

using LiveCharts;
using LiveCharts.Wpf;

using Monitor.TaskControl.Utils;
using Monitor.TaskControl.Models;
using System.Windows.Forms.DataVisualization.Charting;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class pnlSample
    {
        //private readonly IList<Item> shapes;
        private Popup_Screen currentMovingShape = new Popup_Screen();

        public bool m_isFirstAppRect, m_isFirstProcessRect, m_isFirstAudioRect;

        public DateTime m_DeltaProcessTime;
        public ClientInfo m_clientInfo;

        //private double m_dRedLine_Pos = 0;
        //private double m_LastItem_Width = 0;

        private Random rnd = new Random();
        public List<ListOfProcessByOrder> m_ProcessList = new List<ListOfProcessByOrder>();
        public List<ListOfProcessByOrder> m_OldProcessList = new List<ListOfProcessByOrder>();
        public List<ListOfAudio> m_OldAudioList = new List<ListOfAudio>();


        /// <summary>
        /// /////////////////////////  ICON GET COLOR
        /// </summary>
        /// 
        public static List<System.Drawing.Color> TenMostUsedColors { get; private set; }
        public static List<int> TenMostUsedColorIncidences { get; private set; }

        public static System.Drawing.Color MostUsedColor { get; private set; }
        public static int MostUsedColorIncidence { get; private set; }

        private static int pixelColor;

        private static Dictionary<int, int> dctColorIncidence;
        private bool m_DataFlag;
        public bool m_DatePickerFlag;
        public double m_Width;

        public bool m_ChangedSize;
        private double m_InitWidth;

        private double m_previousItemWidth, m_previousItemEnd, m_previousAppWidth, m_previousAppEnd, m_previousAudioEnd, m_previousAudioWidth;
        private string m_strPreviousProcess, m_strPreviousApp;

        public double m_SumDisconnectTime, m_SumRestTime, m_SumWorkTime;

        private bool m_ProcessFilter, m_URLFilter;

        private List<string> iconPath = new List<string>();

        private ListOfProcessByOrder _temp;
        private ListOfProcessByOrder _temp1;
        private ListOfProcessByOrder _temp2;
        /// <summary>
        /// ////////////////////////////
        /// </summary>
        private int m_TimeCount;
        public pnlSample(ClientInfo clientInfo, double width)
        {
            InitializeComponent();
            iconPath.Add("C:\\Windows\\System32\\winver.exe");
            iconPath.Add("C:\\Windows\\System32\\mshta.exe");
            iconPath.Add("C:\\Windows\\System32\\isoburn.exe");
            iconPath.Add("C:\\Windows\\System32\\Magnify.exe");
            m_ChangedSize = false;

            m_isFirstAppRect = false;
            m_isFirstProcessRect = false;
            m_isFirstAudioRect = false;
            m_clientInfo = clientInfo;
            m_Width = width;
            m_InitWidth = width;
            grd_Popup.Children.Add(currentMovingShape);
            currentMovingShape.Visibility = Visibility.Hidden;

            message_body.Visibility = Visibility.Hidden;
            m_DataFlag = false;
            m_DatePickerFlag = false;
            m_previousItemWidth = 0;
            m_previousItemEnd = 0;
            m_previousAppWidth = 0;
            m_previousAppEnd = 0;
            m_previousAudioEnd = 0;
            m_previousAudioWidth = 0;
            m_strPreviousProcess = "";
            m_strPreviousApp = "";

            m_SumDisconnectTime = 0;
            m_SumRestTime = 0;
            m_SumWorkTime = 0;

            m_ProcessFilter = false;
            m_URLFilter = false;

            Init(m_clientInfo);
            Height = 30;

            m_InitWidth = m_Width;
            pnlProgTime.Children.Clear();

            //m_dRedLine_Pos = 0;
            m_isFirstAppRect = false;

            GetTimeStatus(m_clientInfo.ProcessList.ToList());


            //SetVoiceStatus();
            InitShow(m_clientInfo);
            DispatcherTimer dtClockTime = new DispatcherTimer();
            dtClockTime.Interval = new TimeSpan(0, 0, 1); //in Hour, Minutes, Second.
            dtClockTime.Tick += TimerTick;
            dtClockTime.Start();
            m_TimeCount = 0;
        }

        private void TimerTick(object sender, EventArgs e)
        {

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.Year == DateTime.Now.Year && Windows.MainWindow.date_Picker.SelectedDate.Value.Month == DateTime.Now.Month && Windows.MainWindow.date_Picker.SelectedDate.Value.Day == DateTime.Now.Day)
            {
                if (m_TimeCount % 3 == 0)
                {
                    if (!m_clientInfo.NetworkState)
                    {
                        eliForbiddenProcess.Fill = new SolidColorBrush(Colors.LightGray);

                    }
                    else
                    {
                        eliForbiddenProcess.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));

                    }
                }

                if (m_TimeCount % 60 == 0)
                {
                    bool bFlag = false;
                    foreach (var item in Settings.Instance.ClientDic.ToList())
                    {
                        if (item.Key == fldIPAddr.Content.ToString())
                        {
                            bFlag = true;
                        }
                    }
                    if (bFlag == false)
                    {
                        return;
                    }
                    m_clientInfo = Settings.Instance.ClientDic[fldIPAddr.Content.ToString()];

                    RefreshData(m_clientInfo);
                    //m_clientInfo.ProcessList.Clear();
                }
                if (m_TimeCount % 299 == 0)
                {
                    Refresh();
                }
            }
            else
            {

                try
                {
                    if (!Settings.Instance.ClientDic[fldIPAddr.Content.ToString()].NetworkState)
                    {
                        //fldState.Content = "Disconnected";
                        fldComputerName.Content = m_clientInfo.PCName;
                        fldOSDate.Content = m_clientInfo.OSDate;
                        eliForbiddenProcess.Fill = new SolidColorBrush(Colors.LightGray);

                        this.AVARTA.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Connect_Off.png")); // "Resource\\Connect_Off.png";

                    }
                    else
                    {
                        //fldState.Content = "Connected";
                        fldComputerName.Content = m_clientInfo.PCName;
                        fldOSDate.Content = m_clientInfo.OSDate;
                        eliForbiddenProcess.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        this.AVARTA.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Connect_On.png"));
                    }
                    int nID = 0;

                    foreach (var processInfo in m_clientInfo.ProcessList.ToList())
                    {
                        foreach (var processName in Settings.Instance.Forbiddenprocess_list.ToList())
                        {
                            if (processInfo.ProcessName.Contains(processName.Key.Trim()))
                            {
                                nID++;
                            }
                        }
                    }
                    fldForbiden.Content = nID.ToString();
                    fldURL.Content = m_clientInfo.URLList.Count;

                    ShowInspectState(m_clientInfo);
                    //if (m_DatePickerFlag)
                    //{
                    //    Init(m_clientInfo);
                    //}
                }
                catch (Exception ex)
                {

                }
            }


            if (m_TimeCount == 1200) m_TimeCount = 0;

            m_TimeCount++;
        }

        private void InitShow(ClientInfo client)
        {

            //fldState.Content = "Disconnected";
            fldName.Content = client.UserName;
            fldIPAddr.Content = client.ClientIP;

            if (!client.NetworkState)
            {
                //fldState.Content = "Disconnected";
                fldComputerName.Content = client.PCName;
                fldOSDate.Content = client.OSDate;
                eliForbiddenProcess.Fill = new SolidColorBrush(Colors.LightGray);

                this.AVARTA.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Connect_Off.png")); // "Resource\\Connect_Off.png";

            }
            else
            {
                //fldState.Content = "Connected";
                fldComputerName.Content = client.PCName;
                fldOSDate.Content = client.OSDate;
                eliForbiddenProcess.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                this.AVARTA.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Connect_On.png"));
            }

            //fldUSB.Content = "0";
            fldAudio.Content = m_clientInfo.AudioList.Count();

            TimeSpan duration = TimeSpan.FromSeconds(m_SumWorkTime);
            string strDuration = duration.ToString(@"hh\:mm");
            workTime.Content = strDuration;


            /******************************* URL Count **************************************/
            fldURL.Content = client.URLList.Count;
            /****************************************************************************/
            int nDownloadCount = 0;

            foreach (var strDownload in Settings.Instance.DownloadList.ToList())
            {
                if (strDownload.Contains(client.ClientIP))
                {
                    nDownloadCount++;
                }
            }
            fldDownload.Content = nDownloadCount.ToString();
            /********************************* Danger URL ***********************************/
            int nDangerURL = 0;
            foreach (var strFU in Settings.Instance.ForbiddenURLList)
            {
                foreach (var urlInfo in from urlList in client.URLList where urlList.strURL.ToLower().Contains(strFU.ToLower()) select urlList)
                {
                    if (!urlInfo.strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.Updating.ToLower()))
                        nDangerURL++;
                }
            }
            dangerURL.Content = nDangerURL.ToString();
            /****************************** Forbidden Process Count *************************************/
            int nForbiddenCount = 0;
            foreach (var processInfo in client.ProcessList.ToList())
            {
                foreach (var processName in Settings.Instance.Forbiddenprocess_list.ToList())
                {
                    if (processInfo.ProcessName.Contains(processName.Key.Trim()))
                    {
                        nForbiddenCount++;
                    }
                }
            }
            fldForbiden.Content = nForbiddenCount.ToString();

            ShowInspectState(client);

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.Year == DateTime.Now.Year && Windows.MainWindow.date_Picker.SelectedDate.Value.Month == DateTime.Now.Month && Windows.MainWindow.date_Picker.SelectedDate.Value.Day == DateTime.Now.Day)
            {
                Settings.Instance.ClientDic[m_clientInfo.ClientIP].URLCount = URLView.Items.Count.ToString();
                Settings.Instance.ClientDic[m_clientInfo.ClientIP].DownloadCount = nDownloadCount.ToString();
                Settings.Instance.ClientDic[m_clientInfo.ClientIP].DangerURLCount = nDangerURL.ToString();
                Settings.Instance.ClientDic[m_clientInfo.ClientIP].ForbiddenProcessCount = nForbiddenCount.ToString();
                Settings.Instance.ClientDic[m_clientInfo.ClientIP].WorkTimeCount = strDuration.ToString();
            }
            else
            {
                Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].URLCount = URLView.Items.Count.ToString();
                Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].DownloadCount = nDownloadCount.ToString();
                Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].DangerURLCount = nDangerURL.ToString();
                Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].ForbiddenProcessCount = nForbiddenCount.ToString();
                Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].WorkTimeCount = strDuration.ToString();
            }


        }

        public void ShowInspectState(ClientInfo client)
        {
            try
            {
                int percent = 0;
                //Settings.Instance.ClientDic[m_clientInfo.ClientIP].ReadCaptureCount;
                string strDate = string.Format("{0}-{1}-{2}", Windows.MainWindow.date_Picker.SelectedDate.Value.Year, Windows.MainWindow.date_Picker.SelectedDate.Value.Month, Windows.MainWindow.date_Picker.SelectedDate.Value.Day);
                int TotalCount = Directory.GetFiles(System.IO.Path.Combine(Settings.Instance.RegValue.BaseDirectory, strDate, fldIPAddr.Content.ToString(), "Capture")).Count();

                if (TotalCount == 0)
                {
                    percent = 0;
                }
                else
                {
                    if (Windows.MainWindow.date_Picker.SelectedDate.Value.Year == DateTime.Now.Year && Windows.MainWindow.date_Picker.SelectedDate.Value.Month == DateTime.Now.Month && Windows.MainWindow.date_Picker.SelectedDate.Value.Day == DateTime.Now.Day)
                    {
                        percent = client.ReadCaptureCount * 100 / TotalCount;
                    }
                    else
                    {
                        percent = Settings.Instance.ClientDic_Temp[m_clientInfo.ClientIP].ReadCaptureCount * 100 / TotalCount;
                    }
                        
                }
                //string strBattery = string.Empty;

                if (percent == 0)
                {
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/0.png"));
                }
                else if (percent > 0 && percent < 11)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/10.png"));
                }
                else if (percent > 10 && percent < 21)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/20.png"));
                }
                else if (percent > 20 && percent < 31)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/30.png"));
                }
                else if (percent > 30 && percent < 41)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/40.png"));
                }
                else if (percent > 40 && percent < 51)
                {
                    //strBattery = "&#xf242; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/50.png"));
                }
                else if (percent > 50 && percent < 61)
                {
                    //strBattery = "&#xf241; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/60.png"));
                }
                else if (percent > 60 && percent < 71)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/70.png"));
                }
                else if (percent > 70 && percent < 81)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/80.png"));
                }
                else if (percent > 80 && percent < 91)
                {
                    //strBattery = "&#xf243; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/90.png"));
                }
                else if (percent > 90 && percent < 100)
                {
                    //strBattery = "&#xf240; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/90.png"));
                }
                else if (percent == 100)
                {
                    //strBattery = "&#xf240; ";
                    progressInspect.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/100.png"));
                }
                inspect.Content = percent + "% (" + client.ReadCaptureCount + "/" + TotalCount + ")";
            }
            catch
            {
                //inspect.Content = percent + "% (" + Settings.Instance.ClientDic[m_clientInfo.ClientIP].ReadCaptureCount + "/" + TotalCount + ")";
            }
        }
        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            UserInfoHeader.Background.Opacity = 1;
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            UserInfoHeader.Background.Opacity = 0.9;
        }

        private void SetMovingShapePosition(MouseEventArgs e, double width)
        {
            var window = e.GetPosition(this);
            //Console.WriteLine("{0},$$$$$$ {1}", window.X, window.Y);
            //Log.Instance.DoLog("-------- XYX ====> ----", Log.LogType.Info);
            if ((this.ActualWidth - window.X) > width)
            {
                Canvas.SetLeft(grd_Popup, window.X);
                Canvas.SetTop(grd_Popup, window.Y + 25);
            }
            else if ((this.ActualWidth - window.X - 10) < width)
            {
                Canvas.SetLeft(grd_Popup, window.X - width);
                Canvas.SetTop(grd_Popup, window.Y + 25);
            }
        }
        public void SetComputerUsageStatus(List<ListOfProcessByOrder> processLists)
        {
            double bWidth = m_Width - 138;
            //if (this.pnlProgTime.Children.Count > 1)
            //{
            //    //m_dRedLine_Pos -= //m_LastItem_Width;
            //    this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
            //}

            foreach (var workRect in processLists)
            {
                Console.WriteLine(" Start Time : {0} ******** End Time : {1}", workRect.ProcessStartTime, workRect.ProcessEndTime);

                double hourS = workRect.ProcessStartTime.Hour * 3600;
                double minS = workRect.ProcessStartTime.Minute * 60;
                double secondS = workRect.ProcessStartTime.Second;
                double StartSecond = hourS + minS + secondS;

                double hourE = workRect.ProcessEndTime.Hour * 3600;
                double minE = workRect.ProcessEndTime.Minute * 60;
                double secondE = workRect.ProcessEndTime.Second;
                double EndSecond = hourE + minE + secondE;

                if (EndSecond - StartSecond < 0)
                    continue;


                if (m_isFirstAppRect == false && workRect.ProcessStartTime.ToShortTimeString() != "00:00:00")
                {
                    Rectangle rectWorkRest1 = new Rectangle();
                    rectWorkRest1.Width = (StartSecond) / (60 * 60 * 24) * bWidth;
                    rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    pnlProgTime.Children.Add(rectWorkRest1);
                    m_isFirstAppRect = true;
                    //m_dRedLine_Pos += rectWorkRest1.Width;
                    //m_LastItem_Width = rectWorkRest1.Width;


                    Rectangle _rectWorkRest1 = new Rectangle();
                    _rectWorkRest1.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;

                        //m_SumRestTime += EndSecond - StartSecond;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;

                        // m_SumDisconnectTime += EndSecond - StartSecond;
                    }
                    else
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        m_strPreviousProcess = "Work";
                    }

                    pnlProgTime.Children.Add(_rectWorkRest1);
                    //m_dRedLine_Pos += _rectWorkRest1.Width;
                    //m_LastItem_Width = _rectWorkRest1.Width;

                    m_previousItemWidth = _rectWorkRest1.Width;
                    m_previousItemEnd = EndSecond;
                    Console.WriteLine(" ########### Start ############");
                    continue;
                }

                if (StartSecond - m_previousItemEnd < 0)
                {
                    Console.WriteLine(" ########### Sub ############");
                    pnlProgTime.Children.RemoveAt(pnlProgTime.Children.Count - 1);



                    Rectangle rectWorkRestSub = new Rectangle();
                    rectWorkRestSub.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;

                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;
                        //m_SumRestTime += EndSecond - StartSecond;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;
                        //m_SumDisconnectTime += EndSecond - StartSecond;
                    }
                    else
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        m_strPreviousProcess = "Work";
                    }


                    pnlProgTime.Children.Add(rectWorkRestSub);
                    //m_dRedLine_Pos += rectWorkRestSub.Width;
                    //m_LastItem_Width = rectWorkRestSub.Width;

                    m_previousItemWidth = rectWorkRestSub.Width;
                    m_previousItemEnd = EndSecond;
                    //m_SumRestTime += EndSecond - StartSecond;
                    continue;
                }
                else if (StartSecond - m_previousItemEnd > 0/* && StartSecond - m_previousItemEnd != 0*/)
                {
                    Console.WriteLine(" ########### Sub ############");
                    Rectangle rectWorkRestSub = new Rectangle();
                    rectWorkRestSub.Width = (StartSecond - m_previousItemEnd) / (60 * 60 * 24) * bWidth;

                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;
                        //m_SumRestTime += StartSecond - m_previousItemEnd;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;
                        //m_SumDisconnectTime += StartSecond - m_previousItemEnd;
                    }
                    else
                    {
                        if (m_strPreviousProcess == "Work")
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(0, 180, 0));
                            m_strPreviousProcess = "Work";
                        }
                        else if (m_strPreviousProcess == Constants.RestProcess)
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                            m_strPreviousProcess = Constants.RestProcess;
                            //m_SumRestTime += StartSecond - m_previousItemEnd;
                        }
                        else if (m_strPreviousProcess == Constants.DisConnect)
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                            m_strPreviousProcess = Constants.DisConnect;
                            //m_SumDisconnectTime += StartSecond - m_previousItemEnd;
                        }


                    }

                    //rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    //m_strPreviousProcess = Constants.RestProcess;

                    pnlProgTime.Children.Add(rectWorkRestSub);
                    //m_dRedLine_Pos += rectWorkRestSub.Width;
                    //m_LastItem_Width = rectWorkRestSub.Width;

                    m_previousItemWidth = rectWorkRestSub.Width;
                    m_previousItemEnd = StartSecond;

                }

                Rectangle rectWorkRest = new Rectangle();
                if (workRect.ProcessName == Constants.RestProcess)
                {
                    Console.WriteLine(" ########### RestProcess ############");
                    if (workRect.ProcessName == m_strPreviousProcess)
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;
                    //m_LastItem_Width = rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = Constants.RestProcess;
                    //m_SumRestTime += EndSecond - StartSecond;
                }
                else if (workRect.ProcessName == Constants.DisConnect)
                {
                    Console.WriteLine(" ########### Disconnect ############");
                    if (workRect.ProcessName == m_strPreviousProcess)
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;
                    //m_LastItem_Width = rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = Constants.DisConnect;

                    //m_SumDisconnectTime += EndSecond - StartSecond;

                }
                else
                {
                    Console.WriteLine(" ########### Work ############");
                    if (m_strPreviousProcess == "Work")
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;
                    //m_LastItem_Width = rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = "Work";
                }
            }

            //m_SumDisconnectTime = (from processes in m_clientInfo.ProcessList where processes.ProcessName == Constants.DisConnect select processes.ProcessEndTime.Subtract(processes.ProcessStartTime).TotalSeconds).Sum();
            //TimeSpan _duration = TimeSpan.FromSeconds(m_SumDisconnectTime);
            //string _strDuration = _duration.ToString(@"hh\:mm\:ss");
            //Disconnect.Text = _strDuration;



            //m_SumRestTime = (from processes in m_clientInfo.ProcessList where processes.ProcessName == Constants.RestProcess select processes.ProcessEndTime.Subtract(processes.ProcessStartTime).TotalSeconds).Sum();

            //TimeSpan _durationRestTime = TimeSpan.FromSeconds(m_SumRestTime);
            //string _strDurationRest = _durationRestTime.ToString(@"hh\:mm\:ss");
            //Rest.Text = _strDurationRest;

            try
            {

                Slide.Text = m_clientInfo.SlideWidth + " X " + m_clientInfo.SlideHeight;
                Capture.Text = m_clientInfo.CaptureHeight + " X " + m_clientInfo.CaptureWidth;

                SessionTime.Text = m_clientInfo.SessionTime + "s";
                CaptureTime.Text = m_clientInfo.CaptureTime + "s";
                ActiveTime.Text = m_clientInfo.ActiveDuration + "s";
            }
            catch (Exception ex) { }

        }
        public void SetProcessStatus(List<ListOfProcessByOrder> processLists)
        {
            double bWidth = m_Width - 138;
            //if (this.pnlProgProcess.Children.Count > 1)
            //{
            //    this.pnlProgProcess.Children.RemoveAt(this.pnlProgProcess.Children.Count - 1);
            //}
            foreach (var app in processLists)
            {
                double hourS = app.ProcessStartTime.Hour * 3600;
                double minS = app.ProcessStartTime.Minute * 60;
                double secondS = app.ProcessStartTime.Second;
                double StartSecond = hourS + minS + secondS;

                double hourE = app.ProcessEndTime.Hour * 3600;
                double minE = app.ProcessEndTime.Minute * 60;
                double secondE = app.ProcessEndTime.Second;
                double EndSecond = hourE + minE + secondE;

                if (EndSecond - StartSecond < 0)
                    continue;


                if (m_isFirstProcessRect == false && app.ProcessStartTime.ToShortTimeString() != "00:00:00")
                {
                    //double hour0 = app.ProcessStartTime.Hour * 3600;
                    //double min0 = app.ProcessStartTime.Minute * 60;
                    //double second0 = app.ProcessStartTime.Second;
                    //double StartSecond0 = hour0 + min0 + second0;

                    Canvas rectProcess1 = new Canvas();
                    rectProcess1.Width = (StartSecond) / (60 * 60 * 24) * bWidth;
                    rectProcess1.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    this.pnlProgProcess.Children.Add(rectProcess1);

                    m_isFirstProcessRect = true;

                    Canvas rectProcess2 = new Canvas();
                    rectProcess2.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    if (app.ProcessName == Constants.RestProcess)
                    {
                        rectProcess2.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                        m_strPreviousApp = Constants.RestProcess;
                    }
                    else if (app.ProcessName == Constants.DisConnect)
                    {
                        rectProcess2.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousApp = Constants.DisConnect;
                    }
                    else
                    {
                        if (app.ProcessColor == Constants.Default)
                        {
                            string path = app.ProcessPath;
                            if (path == "Unknown")
                            {
                                path = iconPath[rnd.Next(0, 4)];
                            }
                            if (!File.Exists(path))
                            {
                                path = iconPath[rnd.Next(0, 4)];
                            }
                            System.Drawing.Color tmpColor = GetColor(path);
                            //rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + //m_LastItem_Width;
                            rectProcess2.Background = new SolidColorBrush(Color.FromRgb(tmpColor.R, tmpColor.G, tmpColor.B));
                        }
                        else if (app.ProcessColor != "")
                        {
                            string[] strRGB = app.ProcessColor.Split(',');
                            //Int16.Parse(strRGB[0]);
                            try
                            {
                                rectProcess2.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(strRGB[0]), Convert.ToByte(strRGB[1]), Convert.ToByte(strRGB[2])));
                            }
                            catch
                            {
                                rectProcess2.Background = new SolidColorBrush(Color.FromRgb(176, 224, 230));
                            }
                        }

                        m_strPreviousApp = app.ProcessName;
                    }
                    this.pnlProgProcess.Children.Add(rectProcess2);

                    m_previousAppWidth = rectProcess2.Width;
                    m_previousAppEnd = EndSecond;
                    // m_processStart = (StartSecond0) / (60 * 60 * 24) * bWidth;
                    continue;
                }

                //if (StartSecond - m_previousAppEnd < 0) continue;

                if (StartSecond - m_previousAppEnd < 0)
                {
                    Console.WriteLine(" ########### Sub ############");
                    pnlProgProcess.Children.RemoveAt(pnlProgProcess.Children.Count - 1);
                    Canvas rectProcessSub = new Canvas();
                    rectProcessSub.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousAppWidth;

                    if (app.ProcessName == Constants.RestProcess)
                    {
                        rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                        m_strPreviousApp = Constants.RestProcess;
                    }
                    else if (app.ProcessName == Constants.DisConnect)
                    {
                        rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousApp = Constants.DisConnect;
                    }
                    else
                    {
                        //rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                        m_strPreviousApp = app.ProcessName;

                        if (app.ProcessColor == Constants.Default)
                        {
                            string path = app.ProcessPath;
                            if (path == "Unknown")
                            {
                                path = iconPath[rnd.Next(0, 4)];
                            }
                            if (!File.Exists(path))
                            {
                                path = iconPath[rnd.Next(0, 4)];
                            }
                            System.Drawing.Color tmpColor = GetColor(path);
                            //rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + //m_LastItem_Width;
                            rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(tmpColor.R, tmpColor.G, tmpColor.B));
                        }
                        else if (app.ProcessColor != "")
                        {
                            string[] strRGB = app.ProcessColor.Split(',');
                            Int16.Parse(strRGB[0]);
                            try
                            {
                                rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(strRGB[0]), Convert.ToByte(strRGB[1]), Convert.ToByte(strRGB[2])));
                            }
                            catch
                            {
                                rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(176, 224, 230));
                            }
                        }
                    }

                    //rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    //m_strPreviousApp = Constants.RestProcess;

                    pnlProgProcess.Children.Add(rectProcessSub);

                    m_previousAppWidth = rectProcessSub.Width;
                    m_previousAppEnd = EndSecond;


                    continue;
                }
                else if (StartSecond - m_previousAppEnd > 0/* && StartSecond - m_previousItemEnd != 0*/)
                {
                    Console.WriteLine(" ########### Sub ############");
                    Canvas rectProcessSub = new Canvas();
                    rectProcessSub.Width = (StartSecond - m_previousAppEnd) / (60 * 60 * 24) * bWidth;

                    if (app.ProcessName == Constants.RestProcess)
                    {
                        rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                        m_strPreviousApp = Constants.RestProcess;
                    }
                    else if (app.ProcessName == Constants.DisConnect)
                    {
                        rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousApp = Constants.DisConnect;
                    }
                    else
                    {
                        if (m_strPreviousApp == Constants.RestProcess)
                        {
                            rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                            m_strPreviousApp = Constants.RestProcess;
                        }
                        else if (m_strPreviousApp == Constants.DisConnect)
                        {
                            rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                            m_strPreviousApp = Constants.DisConnect;
                        }
                        else
                        {
                            m_strPreviousApp = app.ProcessName;

                            if (app.ProcessColor == Constants.Default)
                            {
                                string path = app.ProcessPath;
                                if (path == "Unknown")
                                {
                                    path = iconPath[rnd.Next(0, 4)];
                                }
                                if (!File.Exists(path))
                                {
                                    path = iconPath[rnd.Next(0, 4)];
                                }
                                System.Drawing.Color tmpColor = GetColor(path);
                                //rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + //m_LastItem_Width;
                                rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(tmpColor.R, tmpColor.G, tmpColor.B));
                            }
                            else if (app.ProcessColor != "")
                            {
                                string[] strRGB = app.ProcessColor.Split(',');
                                Int16.Parse(strRGB[0]);
                                try
                                {
                                    rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(strRGB[0]), Convert.ToByte(strRGB[1]), Convert.ToByte(strRGB[2])));
                                }
                                catch
                                {
                                    rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(176, 224, 230));
                                }
                            }
                        }
                        //rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));

                    }


                    //rectProcessSub.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    //m_strPreviousApp = Constants.RestProcess;

                    pnlProgProcess.Children.Add(rectProcessSub);

                    m_previousAppWidth = rectProcessSub.Width;
                    m_previousAppEnd = StartSecond;

                }


                Canvas rectProcess = new Canvas();
                //rectProcess.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rnd.Next(255)), Convert.ToByte(rnd.Next(256)), Convert.ToByte(rnd.Next(256))));
                if (app.ProcessName == Constants.RestProcess)
                {
                    if (app.ProcessName == m_strPreviousApp)
                    {
                        this.pnlProgProcess.Children.RemoveAt(this.pnlProgProcess.Children.Count - 1);
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousAppWidth;
                    }
                    else
                    {
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectProcess.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));

                    m_previousAppWidth = rectProcess.Width;
                    m_previousAppEnd = EndSecond;

                    m_strPreviousApp = Constants.RestProcess;
                }
                else if (app.ProcessName == Constants.DisConnect)
                {
                    if (app.ProcessName == m_strPreviousApp)
                    {
                        this.pnlProgProcess.Children.RemoveAt(this.pnlProgProcess.Children.Count - 1);
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousAppWidth;
                    }
                    else
                    {
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectProcess.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));

                    m_previousAppWidth = rectProcess.Width;
                    m_previousAppEnd = EndSecond;

                    m_strPreviousApp = Constants.DisConnect;
                }
                else
                {
                    if (app.ProcessName == m_strPreviousApp)
                    {
                        this.pnlProgProcess.Children.RemoveAt(this.pnlProgProcess.Children.Count - 1);
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousAppWidth;
                    }
                    else
                    {
                        rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }


                    if (app.ProcessColor == Constants.Default)
                    {
                        string path = app.ProcessPath;
                        if (path == "Unknown")
                        {
                            path = iconPath[rnd.Next(0, 4)];
                        }
                        if (!File.Exists(path))
                        {
                            path = iconPath[rnd.Next(0, 4)];
                        }
                        System.Drawing.Color tmpColor = GetColor(path);
                        //rectProcess.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + //m_LastItem_Width;
                        rectProcess.Background = new SolidColorBrush(Color.FromRgb(tmpColor.R, tmpColor.G, tmpColor.B));
                    }
                    else if (app.ProcessColor != "")
                    {
                        string[] strRGB = app.ProcessColor.Split(',');
                        Int16.Parse(strRGB[0]);
                        try
                        {
                            rectProcess.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(strRGB[0]), Convert.ToByte(strRGB[1]), Convert.ToByte(strRGB[2])));
                        }
                        catch
                        {
                            rectProcess.Background = new SolidColorBrush(Color.FromRgb(176, 224, 230));
                        }

                    }

                    m_previousAppWidth = rectProcess.Width;
                    m_previousAppEnd = EndSecond;
                    m_strPreviousApp = app.ProcessName;

                }
                try { pnlProgProcess.Children.Add(rectProcess); }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }

            }
        }
        public void SetVoiceStatus(List<ListOfAudio> audioLists)
        {
            double bWidth = m_Width - 138;
            double m_dDelta = 300;

            foreach (var audioList in audioLists)
            {
                double hourS = audioList.ProcessStartTime.Hour * 3600;
                double minS = audioList.ProcessStartTime.Minute * 60;
                double secondS = audioList.ProcessStartTime.Second;
                double StartSecond = hourS + minS + secondS;

                double hourE = audioList.ProcessEndTime.Hour * 3600;
                double minE = audioList.ProcessEndTime.Minute * 60;
                double secondE = audioList.ProcessEndTime.Second;
                double EndSecond = hourE + minE + secondE;

                if (EndSecond - StartSecond < 0)
                    continue;

                if (m_isFirstAudioRect == false && audioList.ProcessStartTime.ToShortTimeString() != "00:00:00")
                {
                    pnlProgAudio.Children.Clear();

                    m_isFirstAudioRect = true;

                    Rectangle rectWorkRest1 = new Rectangle();
                    rectWorkRest1.Width = (StartSecond) / (60 * 60 * 24) * bWidth;
                    rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    pnlProgAudio.Children.Add(rectWorkRest1);

                    Rectangle _rectWorkRest1 = new Rectangle();

                    if ((EndSecond - StartSecond) / (60 * 60 * 24) * bWidth < 0) continue;

                    if (EndSecond - StartSecond < m_dDelta)
                    {
                        _rectWorkRest1.Width = m_dDelta / (60 * 60 * 24) * bWidth;
                        m_previousAudioEnd = StartSecond + m_dDelta;
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(22, 86, 224));
                    }
                    else
                    {
                        _rectWorkRest1.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(27, 157, 195));
                        m_previousAudioEnd = EndSecond;
                    }

                    pnlProgAudio.Children.Add(_rectWorkRest1);

                    continue;
                }




                Rectangle rectWorkRest2 = new Rectangle();
                if ((StartSecond - m_previousAudioEnd) / (60 * 60 * 24) * bWidth < 0) continue;
                rectWorkRest2.Width = (StartSecond - m_previousAudioEnd) / (60 * 60 * 24) * bWidth;
                rectWorkRest2.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                pnlProgAudio.Children.Add(rectWorkRest2);


                Rectangle _rectWorkRest2 = new Rectangle();
                if ((EndSecond - StartSecond) / (60 * 60 * 24) * bWidth < 0) continue;



                if (EndSecond - StartSecond < m_dDelta)
                {
                    _rectWorkRest2.Width = m_dDelta / (60 * 60 * 24) * bWidth;
                    m_previousAudioEnd = StartSecond + m_dDelta;
                    _rectWorkRest2.Fill = new SolidColorBrush(Color.FromRgb(22, 86, 224));
                }
                else
                {
                    _rectWorkRest2.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    m_previousAudioEnd = EndSecond;
                    _rectWorkRest2.Fill = new SolidColorBrush(Color.FromRgb(27, 157, 195));
                }

                pnlProgAudio.Children.Add(_rectWorkRest2);

            }

            Rectangle spaceRect = new Rectangle();

            if (m_previousAppEnd - m_previousAudioEnd / (60 * 60 * 24) * bWidth < 0) return;

            spaceRect.Width = m_previousAppEnd - m_previousAudioEnd / (60 * 60 * 24) * bWidth;
            m_previousAudioEnd += spaceRect.Width / bWidth * 60 * 60 * 24;
            spaceRect.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
            pnlProgAudio.Children.Add(spaceRect);

            //if (audioLists.Count == 0)
            //{
            //    Rectangle spaceRect = new Rectangle();
            //    spaceRect.Width = //m_dRedLine_Pos - m_previousAudioEnd / (60 * 60 * 24) * bWidth + nVirtualWidth;
            //    spaceRect.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
            //    pnlProgAudio.Children.Add(spaceRect);

            //}


        }
        public void ShowProcessTotal(List<ListOfProcessByOrder> processLists)
        {
            if (m_ProcessFilter) return;

            list2View.Items.Clear();
            //var processTotalLists = from proTotalList in Settings.Instance.ProcessList select proTotalList;
            var processTemp = processLists.OrderByDescending(x => x.ProcessEndTime).Select(x => new { ProcessName = x.ProcessName, ProcessWindow = x.ProcessWindow, Duration = x.ProcessEndTime.Subtract(x.ProcessStartTime).TotalSeconds, Path = x.ProcessPath }).ToList();
            var processTotal = processTemp.GroupBy(t => t.ProcessName).Select(g => new { ProcessName = g.Key, ProcessWindow = g.First().ProcessWindow, Duration = g.Sum(u => u.Duration), Path = g.First().Path });

            foreach (var process in processTotal)
            {
                if (process.ProcessName == Constants.RestProcess || process.ProcessName == Constants.DisConnect || process.ProcessName == Constants.HideProcess_IDLE || process.ProcessName == Constants.HideProcess_APH || process.ProcessName == Constants.HideProcess_LockApp)
                    continue;
                TimeSpan _duration = TimeSpan.FromSeconds(process.Duration);
                string _strDuration = "  " + _duration.ToString(@"hh\:mm\:ss");

                if (_strDuration == "  00:00:00")
                    continue;
                Icon icon = null;
                try
                {
                    if (process.Path == "Unknown")
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                    }
                    else if (!File.Exists(process.Path))
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                    }
                    else
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(process.Path);
                    }
                }
                catch
                {
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                }

                BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                string strState = "";

                foreach (var strFP in Settings.Instance.Forbiddenprocess_list.ToList())
                {
                    if (process.ProcessWindow.ToLower().Contains(strFP.Value.ToLower()))
                    {
                        strState = "Danger";
                        break;
                    }
                }
                string strWindowTemp = process.ProcessWindow;
                if (strWindowTemp == "" || strWindowTemp == Constants.Unknown)
                {
                    strWindowTemp = process.ProcessName;
                }
                list2View.Items.Add(new ProcessTotalItem { List2Icon = bitmapSource, Process = strWindowTemp, Time = _strDuration, State = strState });
            }

            TimeSpan totalTime = new TimeSpan();
            string strTotalTime = "";
            for (int i = 0; i < list2View.Items.Count; i++)
            {
                totalTime += TimeSpan.Parse((list2View.Items[i] as ProcessTotalItem).Time);
            }

            strTotalTime = totalTime.ToString(@"hh\:mm\:ss");

            //TotalTime.Text = Constants.TotalWTime + strTotalTime;
            m_SumWorkTime = totalTime.TotalSeconds;

        }

        public void ShowURL(List<ListOfUrl> urlLists)
        {
            if (m_URLFilter) return;

            string ChromePath32 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            string ChromePath64 = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

            string FirefoxPath32 = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            string FirefoxPath64 = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";


            string IEPath = "C:\\Program Files\\Internet Explorer\\iexplore.exe";
            string Edge = "";
            Icon icon = null;
            URLView.Items.Clear();
            bool bURL = true;
            string _strAllowUrl = "";
            try
            {
                var urlList = urlLists.OrderByDescending(x => x.URLEndTime).ToList();
                //urlLists = urlLists.OrderByDescending()
                foreach (var urlInfo in urlList)
                {
                    foreach (string strAllowURL in Settings.Instance.AllowURLList)
                    {
                        _strAllowUrl = strAllowURL;
                        if (strAllowURL.Contains("www"))
                        {
                            _strAllowUrl = strAllowURL.Remove(0, strAllowURL.IndexOf("www") + 4);
                        }
                        else if (strAllowURL.Contains("http"))
                        {
                            _strAllowUrl = strAllowURL.Remove(0, strAllowURL.IndexOf("//") + 2);
                        }
                        _strAllowUrl = _strAllowUrl.Replace("/", "");
                        if (urlInfo.strURL.Contains(_strAllowUrl.Trim()))
                        {
                            bURL = false;
                            break;
                        }
                        bURL = true;
                    }

                    if (bURL)
                    {
                        if (urlInfo.BrowserType == 1)
                        {
                            if (File.Exists(ChromePath32))
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath32);
                            }
                            else
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath64);
                            }
                        }
                        else if (urlInfo.BrowserType == 2)
                        {
                            if (File.Exists(FirefoxPath32))
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath32);
                            }
                            else
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath64);
                            }

                        }
                        else if (urlInfo.BrowserType == 3)
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(Edge);
                        }
                        else if (urlInfo.BrowserType == 4)
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(IEPath);
                        }

                        BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());

                        string strState = "";
                        foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                        {
                            if (urlInfo.strURL.ToLower().Contains(urlFURL.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.Updating.ToLower()))
                            {
                                strState = "Danger";
                                break;
                            }

                        }
                        URLView.Items.Add(new URL { List2Icon = bitmapSource, strURL = urlInfo.strURL, StartTime = urlInfo.URLStartTime.ToLongTimeString(), EndTime = urlInfo.URLEndTime.ToLongTimeString(), State = strState });

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public static BitmapSource ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Pbgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        public static void GetMostUsedColor(Bitmap theBitMap)
        {
            TenMostUsedColors = new List<System.Drawing.Color>();
            TenMostUsedColorIncidences = new List<int>();

            MostUsedColor = System.Drawing.Color.Empty;
            MostUsedColorIncidence = 0;

            // does using Dictionary<int,int> here
            // really pay-off compared to using
            // Dictionary<Color, int> ?

            // would using a SortedDictionary be much slower, or ?

            dctColorIncidence = new Dictionary<int, int>();

            // this is what you want to speed up with unmanaged code
            for (int row = 0; row < theBitMap.Size.Width; row++)
            {
                for (int col = 0; col < theBitMap.Size.Height; col++)
                {
                    pixelColor = theBitMap.GetPixel(row, col).ToArgb();

                    if (dctColorIncidence.Keys.Contains(pixelColor))
                    {
                        dctColorIncidence[pixelColor]++;
                    }
                    else
                    {
                        dctColorIncidence.Add(pixelColor, 1);
                    }
                }
            }

            // note that there are those who argue that a
            // .NET Generic Dictionary is never guaranteed
            // to be sorted by methods like this
            var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // this should be replaced with some elegant Linq ?
            foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow.Take(10))
            {
                TenMostUsedColors.Add(System.Drawing.Color.FromArgb(kvp.Key));
                TenMostUsedColorIncidences.Add(kvp.Value);
            }

            MostUsedColor = System.Drawing.Color.FromArgb(dctSortedByValueHighToLow.First().Key);
            MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
        }

        private System.Drawing.Color GetColor(string Path)
        {
            Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(Path);
            Bitmap bMap = ico.ToBitmap();
            //   GetMostUsedColor(bMap);
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bMap.Width; x++)
            {
                for (int y = 0; y < bMap.Height; y++)
                {
                    System.Drawing.Color clr = bMap.GetPixel(x, y);
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;
                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;
            System.Drawing.Color color = bMap.GetPixel(bMap.Width / 2, bMap.Height / 2);
            //   System.Drawing.Color color = MostUsedColorIncidence;
            return color;
            //return System.Drawing.Color.FromArgb(r, g, b);
        }
        public void Init(ClientInfo clientinfo)
        {
            int nBeforeCount = m_clientInfo.ProcessList.Count;
            int n1 = m_clientInfo.ProcessServerList.Count;

            //MergeProcessList(m_clientInfo);

            int nAfterCount = m_clientInfo.ProcessList.Count;
            //MergeProcessList(clientinfo);

            SetComputerUsageStatus(clientinfo.ProcessList.OrderBy(x => x.ProcessStartTime).ToList());
            SetProcessStatus(clientinfo.ProcessList.OrderBy(x => x.ProcessStartTime).ToList());
            m_OldProcessList = clientinfo.ProcessList;
            SetVoiceStatus(clientinfo.AudioList);
            m_OldAudioList = clientinfo.AudioList;

            ShowProcessTotal(clientinfo.ProcessList);
            ShowURL(clientinfo.URLList);
            m_DataFlag = true;

            SetRedLinePos();

            m_DatePickerFlag = false;
        }

        public void RefreshData(ClientInfo client)
        {
            //string strBase = Settings.Instance.RegValue.BaseDirectory;
            //strBase = strBase.Substring(0, 1);
            //DriveInfo dDrive = new DriveInfo(strBase);

            //// When the drive is accessible..
            //if (dDrive.IsReady)
            //{
            //    if (dDrive.AvailableFreeSpace < 1073741824)
            //        Windows.MainWindow.DiskSpace.Content = "There is less than 1 GB of free space on the disk " + strBase + ".";
            //    else
            //        Windows.MainWindow.DiskSpace.Content = "";

            //}
            ////1073741824

            InitShow(client);

            if (!client.NetworkState) return;

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.Year != DateTime.Now.Year || Windows.MainWindow.date_Picker.SelectedDate.Value.Month != DateTime.Now.Month || Windows.MainWindow.date_Picker.SelectedDate.Value.Day != DateTime.Now.Day)
                return;



            List<ListOfProcessByOrder> tmpProcessList = new List<ListOfProcessByOrder>();
            if (client.ProcessList.Count == 0)
                return;
            //*****************//
            if (!m_DataFlag)
            {
                try
                {
                    Init(client);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exExit, ex);
                }

                m_DataFlag = true;
                return;
            }
            //*****************//


            if (m_OldProcessList.Count == 0)
            {
                tmpProcessList = client.ProcessList.ToList();
            }
            else
            {
                for (int i = m_OldProcessList.Count; i < client.ProcessList.Count; i++)
                {
                    tmpProcessList.Add(client.ProcessList[i]);
                }
            }


            List<ListOfAudio> tmpAudioList = new List<ListOfAudio>();

            if (m_OldAudioList.Count == 0)
            {
                tmpAudioList = client.AudioList.ToList();
            }
            else
            {
                for (int i = m_OldAudioList.Count; i < client.AudioList.Count; i++)
                {
                    tmpAudioList.Add(client.AudioList[i]);
                }
            }

            SetComputerUsageStatus(tmpProcessList);
            SetProcessStatus(tmpProcessList);
            SetVoiceStatus(tmpAudioList);
            ShowProcessTotal(client.ProcessList.ToList());
            ShowURL(client.URLList);
            SetRedLinePos();
            m_OldProcessList = client.ProcessList.ToList();
            m_OldAudioList = client.AudioList.ToList();
        }
        private void SetRedLinePos()
        {
            //Canvas.SetLeft(grd_RedLine, //m_dRedLine_Pos + 13);
        }
        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Height == 30)
            {
                m_ChangedSize = true;
                this.Height = 500;


                UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 160, 160));

            }
            else
            {
                m_ChangedSize = false;
                this.Height = 30;

                UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 139, 139));
            }


        }
        private void WorkTime_Move(object sender, MouseEventArgs e)
        {
            currentMovingShape.Visibility = Visibility.Hidden;

            if (DateTime.Now.Subtract(m_DeltaProcessTime).TotalMilliseconds > 50)
            {
                GetWorkRestInfo(e);
                m_DeltaProcessTime = DateTime.Now;
            }
            Console.WriteLine("AUDIO=======> {0}", e.GetPosition(this).Y);
            //if (e.GetPosition(this).Y > 140 || e.GetPosition(this).Y < 127 /*|| e.GetPosition(this).X > m_previousAppEnd+5*/)
            //{
            //    try
            //    {
            //        currentMovingShape.Visibility = Visibility.Hidden;
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
        }
        private void WorkTime_Leave(object sender, MouseEventArgs e)
        {
            currentMovingShape.Visibility = Visibility.Hidden;
        }

        public void GetWorkRestInfo(MouseEventArgs e)
        {
            double bWidth = m_Width - 138;
            double totalSecond = (e.GetPosition(this).X - 106) / bWidth * 60 * 60 * 24;

            if (double.IsNaN(totalSecond)) return;

            TimeSpan tTime = TimeSpan.FromSeconds(totalSecond);

            string strTime = tTime.ToString(@"hh\-mm\-ss");
            string strSTime = tTime.ToString(@"hh\:mm\:ss");

            DateTime dateTime;
            DateTime.TryParse(Windows.MainWindow.date_Picker.Text + " " + strSTime, out dateTime);

            var _itemInfos = from item in m_clientInfo.ProcessList where item.ProcessStartTime <= dateTime && item.ProcessEndTime >= dateTime select item;
            var _itemInfos_Disconnect = from item in m_clientInfo.ProcessList where item.ProcessName == Constants.DisConnect select item;

            string strStatus = "";
            //string strFirstTime = "";
            //string strEndTime = "";
            DateTime FirstTime = DateTime.Now, EndTime = DateTime.Now;

            if (_itemInfos.ToList().Count < 0)
            {
                currentMovingShape.Visibility = Visibility.Hidden;
                return;
            }


            foreach (var itemInfos in _itemInfos)
            {
                if (itemInfos.ProcessName == "RestProcess")
                {
                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime) where workInfos.ProcessStartTime <= dateTime select workInfos)
                    {
                        //strFirstTime = item.ProcessEndTime.ToString("HH:mm:ss");
                        FirstTime = item.ProcessEndTime;
                        if (item.ProcessName == "RestProcess") continue;

                        strStatus = Constants.Rest;

                        break;
                    }

                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderBy(x => x.ProcessStartTime) where workInfos.ProcessStartTime >= dateTime select workInfos)
                    {
                        //strEndTime = item.ProcessStartTime.ToString("HH:mm:ss");
                        EndTime = item.ProcessStartTime;
                        if (item.ProcessName == "RestProcess") continue;

                        strStatus = Constants.Rest;

                        break;
                    }
                }
                else if (itemInfos.ProcessName == Constants.DisConnect)
                {
                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime) where workInfos.ProcessStartTime <= dateTime select workInfos)
                    {
                        //strFirstTime = item.ProcessEndTime.ToString("HH:mm:ss");
                        FirstTime = item.ProcessEndTime;
                        if (item.ProcessName == Constants.DisConnect) continue;

                        strStatus = Constants.DisConnect;

                        break;
                    }

                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderBy(x => x.ProcessStartTime) where workInfos.ProcessStartTime >= dateTime select workInfos)
                    {
                        //strEndTime = item.ProcessStartTime.ToString("HH:mm:ss");
                        EndTime = item.ProcessStartTime;
                        if (item.ProcessName == Constants.DisConnect) continue;

                        strStatus = Constants.DisConnect;

                        break;
                    }
                }
                else
                {
                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime) where workInfos.ProcessStartTime <= dateTime select workInfos)
                    {
                        //strFirstTime = item.ProcessEndTime.ToString("HH:mm:ss");
                        FirstTime = item.ProcessEndTime;
                        if (item.ProcessName == Constants.DisConnect || item.ProcessName == "RestProcess")
                        {

                            strStatus = "Work";

                            break;
                        }

                    }

                    foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderBy(x => x.ProcessStartTime) where workInfos.ProcessStartTime >= dateTime select workInfos)
                    {
                        //strEndTime = item.ProcessStartTime.ToString("HH:mm:ss");
                        EndTime = item.ProcessStartTime;
                        if (item.ProcessName == Constants.DisConnect || item.ProcessName == "RestProcess")
                        {

                            strStatus = "Work";

                            break;
                        }
                    }
                }

                currentMovingShape.TimeText.Text = "Time : " + strSTime;
                currentMovingShape.ProcessText.Text = /*"End Time : " + */ "Duration : " + TimeSpan.FromSeconds(EndTime.Subtract(FirstTime).TotalSeconds).ToString(@"hh\:mm\:ss");
                currentMovingShape.PathText.Text = "Status : " + strStatus;
                currentMovingShape.WindowText.Text = /*"Start Time : " + */"From To : " + FirstTime.ToLongTimeString() + " ~ " + EndTime.ToLongTimeString();

                currentMovingShape.SetScale(240, 50);
                currentMovingShape.img_Screenshot.Source = null;
                SetMovingShapePosition(e, currentMovingShape.ActualWidth);
                currentMovingShape.Visibility = Visibility.Visible;
                //break;
                return;
            }


            foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime) where workInfos.ProcessStartTime <= dateTime select workInfos)
            {
                //strFirstTime = item.ProcessEndTime.ToString("HH:mm:ss");
                FirstTime = item.ProcessEndTime;
                if (item.ProcessName == Constants.DisConnect) continue;

                strStatus = Constants.DisConnect;

                break;
            }

            foreach (var item in from workInfos in m_clientInfo.ProcessList.OrderBy(x => x.ProcessStartTime) where workInfos.ProcessStartTime >= dateTime select workInfos)
            {
                //strEndTime = item.ProcessStartTime.ToString("HH:mm:ss");
                EndTime = item.ProcessStartTime;
                if (item.ProcessName == Constants.DisConnect) continue;

                strStatus = Constants.DisConnect;

                break;
            }

            currentMovingShape.TimeText.Text = "Time : " + strSTime;
            currentMovingShape.ProcessText.Text = /*"End Time : " + */ "Duration : " + TimeSpan.FromSeconds(EndTime.Subtract(FirstTime).TotalSeconds).ToString(@"hh\:mm\:ss");
            currentMovingShape.PathText.Text = "Status : " + strStatus;
            currentMovingShape.WindowText.Text = /*"Start Time : " + */"From To : " + FirstTime.ToLongTimeString() + " ~ " + EndTime.ToLongTimeString();

            currentMovingShape.SetScale(240, 70);
            currentMovingShape.img_Screenshot.Source = null;
            SetMovingShapePosition(e, currentMovingShape.ActualWidth);
            currentMovingShape.Visibility = Visibility.Visible;


        }
        public void GetAudioInfo(MouseEventArgs e)
        {
            double bWidth = m_Width - 138;
            double totalSecond = (e.GetPosition(this).X - 106) / bWidth * 60 * 60 * 24;


            if (double.IsNaN(totalSecond)) return;

            int nCount = -360;
            while (nCount <= 300)
            {
                nCount += 60;
                //TimeSpan tTime = TimeSpan.FromSeconds(totalSecond + nCount);

                //string strHour = tTime.Hours.ToString();
                //string strMinute = tTime.Minutes.ToString();
                //string strSecond = tTime.Seconds.ToString();

                //string strDate = string.Format("{0}-{1}-{2}", Windows.MainWindow.date_Picker.SelectedDate.Value.Year, Windows.MainWindow.date_Picker.SelectedDate.Value.Month, Windows.MainWindow.date_Picker.SelectedDate.Value.Day);

                if (totalSecond + nCount < 0) continue;

                TimeSpan tTime = TimeSpan.FromSeconds(totalSecond + nCount);

                string strTime = tTime.ToString(@"hh\-mm\-ss");
                string strSTime = tTime.ToString(@"hh\:mm\:ss");

                try
                {
                    var _strAudioInfo = from item in m_clientInfo.AudioList where item.ProcessStartTime.ToString("HH:mm:ss").StartsWith(strSTime.Remove(5, 3)) select item;


                    if (_strAudioInfo.Count() > 0)
                    {
                        foreach (var audioInfo in _strAudioInfo)
                        {
                            currentMovingShape.TimeText.Text = "Time : " + strTime.Replace('-', ':') + "  ( " + audioInfo.ProcessStartTime.ToString("HH:mm") + "  ~  " + audioInfo.ProcessEndTime.ToString("HH:mm") + "  )        ";
                            currentMovingShape.ProcessText.Text = "Process Name : " + audioInfo.ProcessName + "        ";
                            currentMovingShape.PathText.Text = "Window Title : " + audioInfo.ProcessWindow + "        ";
                            currentMovingShape.WindowText.Text = "Size : " + audioInfo.FileSize;

                            currentMovingShape.SetScale(240, 70);
                            currentMovingShape.img_Screenshot.Source = null;
                            SetMovingShapePosition(e, currentMovingShape.ActualWidth);
                            currentMovingShape.Visibility = Visibility.Visible;

                            break;
                        }

                        break;
                    }
                    else
                    {
                        DateTime dateTime;
                        DateTime.TryParse(Windows.MainWindow.date_Picker.Text + " " + strSTime, out dateTime);
                        var audioInfos = from item in m_clientInfo.AudioList where item.ProcessStartTime <= dateTime && item.ProcessEndTime >= dateTime select item;

                        if (audioInfos.Count() > 0)
                        {
                            foreach (var audioInfo in audioInfos)
                            {

                                if (string.IsNullOrWhiteSpace(audioInfo.ProcessStartTime.ToString("HH:mm:ss")))
                                {
                                    currentMovingShape.Visibility = Visibility.Hidden;
                                    return;
                                }
                                currentMovingShape.TimeText.Text = "Time : " + strTime.Replace('-', ':') + "  ( " + audioInfo.ProcessStartTime.ToString("HH:mm") + "  ~  " + audioInfo.ProcessEndTime.ToString("HH:mm") + "  )        ";
                                currentMovingShape.ProcessText.Text = "Process Name : " + audioInfo.ProcessName + "        ";
                                currentMovingShape.PathText.Text = "Window Title : " + audioInfo.ProcessWindow + "        ";
                                currentMovingShape.WindowText.Text = "Size : " + audioInfo.FileSize;

                                currentMovingShape.SetScale(240, 70);
                                currentMovingShape.img_Screenshot.Source = null;
                                SetMovingShapePosition(e, currentMovingShape.ActualWidth);
                                currentMovingShape.Visibility = Visibility.Visible;

                                break;
                            }

                        }
                        else
                        {
                            currentMovingShape.Visibility = Visibility.Hidden;
                            //return;
                        }

                    }

                }
                catch (Exception ex) { return; }
            }

            //if (totalSecond + nCount < 0) continue;




            //TimeSpan tTime = TimeSpan.FromSeconds(totalSecond);

            //string strTime = tTime.ToString(@"hh\-mm\-ss");
            //string strSTime = tTime.ToString(@"hh\:mm\:ss");

            //DateTime dateTime;
            //DateTime.TryParse(Windows.MainWindow.date_Picker.Text + " " + strSTime, out dateTime);

            //var audioInfos = from item in m_clientInfo.AudioList where item.ProcessStartTime <= dateTime && item.ProcessEndTime >= dateTime select item;

            //if (audioInfos.Count() > 0)
            //{
            //    foreach (var audioInfo in audioInfos)
            //    {

            //        if (string.IsNullOrWhiteSpace(audioInfo.ProcessStartTime.ToString("HH:mm:ss")))
            //        {
            //            currentMovingShape.Visibility = Visibility.Hidden;
            //            return;
            //        }
            //        currentMovingShape.TimeText.Text = "Time : " + strTime.Replace('-', ':') + "  ( " + audioInfo.ProcessStartTime.ToString("HH:mm") + "  ~  " + audioInfo.ProcessEndTime.ToString("HH:mm") + "  )        ";
            //        currentMovingShape.ProcessText.Text = "Process Name : " + audioInfo.ProcessName + "        ";
            //        currentMovingShape.PathText.Text = "Window Title : " + audioInfo.ProcessWindow + "        ";
            //        currentMovingShape.WindowText.Text = "Size : " + audioInfo.FileSize;

            //        currentMovingShape.SetScale(240, 70);
            //        currentMovingShape.img_Screenshot.Source = null;
            //        SetMovingShapePosition(e, currentMovingShape.ActualWidth);
            //        currentMovingShape.Visibility = Visibility.Visible;

            //        break;
            //    }
            //}
            //else
            //{
            //    currentMovingShape.Visibility = Visibility.Hidden;
            //    //return;
            //}

        }
        public void GetProcessScreen(MouseEventArgs e)
        {
            double bWidth = m_Width - 138;
            double totalSecond = (e.GetPosition(this).X - 106) / bWidth * 60 * 60 * 24;
            TimeSpan tTime = TimeSpan.FromSeconds(totalSecond);

            string strHour = tTime.Hours.ToString();
            string strMinute = tTime.Minutes.ToString();
            string strSecond = tTime.Seconds.ToString();

            string strDate = string.Format("{0}-{1}-{2}", Windows.MainWindow.date_Picker.SelectedDate.Value.Year, Windows.MainWindow.date_Picker.SelectedDate.Value.Month, Windows.MainWindow.date_Picker.SelectedDate.Value.Day);

            string strSlideFileName = "";

            tTime = TimeSpan.FromSeconds(totalSecond);

            string strTime = tTime.ToString(@"hh\-mm\-ss");
            string strSTime = tTime.ToString(@"hh\:mm\:ss");
            DateTime dateTime;
            DateTime.TryParse(Windows.MainWindow.date_Picker.Text + " " + strSTime, out dateTime);

            strHour = tTime.Hours.ToString();
            strMinute = tTime.Minutes.ToString();
            strSecond = tTime.Seconds.ToString();

            string strFileName1 = System.IO.Path.Combine(Settings.Instance.RegValue.BaseDirectory, strDate, fldIPAddr.Content.ToString(), "Slide", strHour + "-" + strMinute + "-" + strSecond + ".jpg");

            string strTempPath = System.IO.Path.Combine(Settings.Instance.RegValue.BaseDirectory, strDate, fldIPAddr.Content.ToString(), "Slide");
            string strTempTime = strHour + "-" + strMinute + "-" + strSecond + "." + Constants.strImgExtension;

            string strFileName2 = Md5Crypto.OnImageFileNameChange(strTempPath, strTime);
            //strFileName2 = strFileName2 + "." + Constants.strImgExtension;

            string _strFileName = strFileName2.Remove(0, strFileName2.IndexOf("Slide") + 6);
            _strFileName = _strFileName.Remove(5, 3);
            //List<string> strFiles = Directory.EnumerateFiles(strTempPath, "*.psl", SearchOption.AllDirectories);
            if (!Directory.Exists(strTempPath)) return;

            var _strRealFiles = from file in Directory.EnumerateFiles(strTempPath, "*.psl", SearchOption.AllDirectories) where file.StartsWith(System.IO.Path.Combine(strTempPath, _strFileName)) select file;

            try
            {
                if (_strRealFiles.Count() > 0)
                {
                    string[] strPSLFile = _strRealFiles.ToArray();

                    byte[] temp = Md5Crypto.OnReadImgFile(strPSLFile[0]);

                    using (MemoryStream ms = new MemoryStream(temp))
                    {

                        using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                        {


                            string strProcessName = "", strPath = "", strTimeDuration = "", strWindow = "";

                            foreach (var Process in (from Processes in m_clientInfo.ProcessList where Processes.ProcessStartTime <= dateTime && Processes.ProcessEndTime >= dateTime select Processes))
                            {
                                if (Process.ProcessName == Constants.RestProcess || Process.ProcessName == Constants.DisConnect || Process.ProcessName == "") continue;


                                strProcessName = Process.ProcessName;
                                strPath = "Path : " + Process.ProcessPath;
                                strTimeDuration = "  ( " + Process.ProcessStartTime.ToLongTimeString() + " ~ " + Process.ProcessEndTime.ToLongTimeString() + " )";
                                strWindow = Process.ProcessWindow;


                                if (Process.ProcessName.Trim() == "chrome.exe" || Process.ProcessName.Trim() == "firefox.exe")
                                {
                                    foreach (var strURLs in from urlList in m_clientInfo.URLList where urlList.URLStartTime <= dateTime || urlList.URLEndTime >= dateTime select urlList.strURL)
                                    {
                                        if (strURLs == "") continue;

                                        strPath = "URL : " + strURLs;
                                        break;
                                    }

                                }
                                if (strProcessName == "") continue;


                                break;
                            }

                            if (strProcessName == "") return;

                            currentMovingShape.TimeText.Text = "Time : " + strTime.Replace('-', ':') + strTimeDuration;
                            currentMovingShape.ProcessText.Text = "Process : " + strProcessName;
                            currentMovingShape.PathText.Text = strPath;
                            currentMovingShape.WindowText.Text = "Window : " + strWindow;
                            try
                            {
                                BitmapImage imageSource = new BitmapImage();
                                imageSource.BeginInit();
                                MemoryStream ms1 = new MemoryStream(temp);
                                imageSource.StreamSource = ms1;
                                imageSource.EndInit();
                                currentMovingShape.img_Screenshot.Source = imageSource;
                            }
                            catch (Exception ex)
                            {

                            }

                            //if (img.Width > img.Height * 2)
                            //    currentMovingShape.SetScale(710, 200);
                            //else
                            currentMovingShape.SetScale(img.Width, img.Height);
                            SetMovingShapePosition(e, currentMovingShape.ActualWidth);
                            currentMovingShape.Visibility = Visibility.Visible;
                            //Log.Instance.DoLog("Screen Path OK =======>>>>" + strSlideFileName, Log.LogType.Info);
                        }
                    }
                }
                else
                {
                    currentMovingShape.Visibility = Visibility.Hidden;
                    return;
                    //strMinute = (tTime.Minutes + 1).ToString();
                    //goto AA;
                }
            }
            catch (Exception ex)
            {
                return;

            }



        }

        private void Progress_Move(object sender, MouseEventArgs e)
        {
            if (DateTime.Now.Subtract(m_DeltaProcessTime).TotalMilliseconds > 50)
            {
                GetProcessScreen(e);
                m_DeltaProcessTime = DateTime.Now;
            }
            Console.WriteLine("=======> {0}", e.GetPosition(this).Y);
            if (e.GetPosition(this).Y > 115 || e.GetPosition(this).Y < 96 /*|| e.GetPosition(this).X > m_previousAppEnd+5*/)
            {
                try
                {
                    currentMovingShape.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void Progress_Leave(object sender, MouseEventArgs e)
        {
            currentMovingShape.Visibility = Visibility.Hidden;
        }

        private void SlideButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string strIP = this.fldIPAddr.Content.ToString();
                string strUser = this.fldName.Content.ToString();
                DateTime selectedDate = Windows.MainWindow.date_Picker.SelectedDate.Value;
                string strDatePath = selectedDate.Year.ToString() + "-" + selectedDate.Month.ToString() + "-" + selectedDate.Day.ToString();
                string strPath = Settings.Instance.Directories.WorkDirectory + @"\" + strDatePath + "\\" + strIP + "\\" + "Capture";
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string[] fileEntries = Directory.GetFiles(strPath);
                if (fileEntries.Length == 0)
                {
                    CustomMsg msg = new CustomMsg("There is no the images to see this user's history.");
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {

                        SlideWindow window = new SlideWindow(strPath, strIP, strUser, string.Format("{0}-{1}-{2}", selectedDate.Year, selectedDate.Month, selectedDate.Day));

                        //window.start(strPath);
                        window.ShowDialog();
                    });
                }
                //SlideWindow window = new SlideWindow();

            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
                CustomMsg msg = new CustomMsg("There is no the Image Data.");
            }



        }
        private void Grd_Popup_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.GetPosition(this).Y > 170 || e.GetPosition(this).Y < 138)
            {
                currentMovingShape.Visibility = Visibility.Hidden;
            }
        }
        private void BtnShowState_Click(object sender, RoutedEventArgs e)
        {
            //CustomMsg message = new CustomMsg("Remote function is now developing ....");
            try
            {
                string strIP = this.fldIPAddr.Content.ToString();
                if (Settings.Instance.ClientDic[strIP].NetworkState == true)
                {
                    CommProc.Instance.SendaAnalysis(strIP, "Start", Constants.Se_VidCapture);
                    Thread.Sleep(500);
                    Windows.CaptureWindow = new RemoteWindow(strIP);
                    Windows.CaptureWindow.ShowDialog();

                }
                else
                {
                    CustomMsg msg = new CustomMsg("This user is currently disconnected.");
                }
            }
            catch
            {
                CustomMsg msg = new CustomMsg("This user is currently disconnected.");
            }



        }
        public void GetTimeStatus(List<ListOfProcessByOrder> processLists)
        {
            double bWidth = m_Width - 138;
            //if (this.pnlProgTime.Children.Count > 1)
            //{
            //    //m_dRedLine_Pos -= //m_LastItem_Width;
            //    this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
            //}
            m_SumDisconnectTime = 0;
            m_SumRestTime = 0;
            foreach (var workRect in processLists)
            {
                Console.WriteLine(" Start Time : {0} ******** End Time : {1}", workRect.ProcessStartTime, workRect.ProcessEndTime);

                double hourS = workRect.ProcessStartTime.Hour * 3600;
                double minS = workRect.ProcessStartTime.Minute * 60;
                double secondS = workRect.ProcessStartTime.Second;
                double StartSecond = hourS + minS + secondS;

                double hourE = workRect.ProcessEndTime.Hour * 3600;
                double minE = workRect.ProcessEndTime.Minute * 60;
                double secondE = workRect.ProcessEndTime.Second;
                double EndSecond = hourE + minE + secondE;

                if (EndSecond - StartSecond < 0)
                    continue;


                if (m_isFirstAppRect == false && workRect.ProcessStartTime.ToShortTimeString() != "00:00:00")
                {
                    Rectangle rectWorkRest1 = new Rectangle();
                    rectWorkRest1.Width = (StartSecond) / (60 * 60 * 24) * bWidth;
                    rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                    pnlProgTime.Children.Add(rectWorkRest1);
                    m_isFirstAppRect = true;
                    //m_dRedLine_Pos += rectWorkRest1.Width;

                    Rectangle _rectWorkRest1 = new Rectangle();
                    _rectWorkRest1.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;

                        //m_SumRestTime += EndSecond - StartSecond;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;

                        // m_SumDisconnectTime += EndSecond - StartSecond;
                    }
                    else
                    {
                        _rectWorkRest1.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        m_strPreviousProcess = "Work";
                    }

                    pnlProgTime.Children.Add(_rectWorkRest1);
                    //m_dRedLine_Pos += _rectWorkRest1.Width;

                    m_previousItemWidth = _rectWorkRest1.Width;
                    m_previousItemEnd = EndSecond;
                    Console.WriteLine(" ########### Start ############");
                    continue;
                }

                if (StartSecond - m_previousItemEnd < 0)
                {
                    Console.WriteLine(" ########### Sub ############");
                    pnlProgTime.Children.RemoveAt(pnlProgTime.Children.Count - 1);



                    Rectangle rectWorkRestSub = new Rectangle();
                    rectWorkRestSub.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;

                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;
                        m_SumRestTime += EndSecond - StartSecond;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;
                        m_SumDisconnectTime += EndSecond - StartSecond;
                    }
                    else
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                        m_strPreviousProcess = "Work";
                    }


                    pnlProgTime.Children.Add(rectWorkRestSub);
                    //m_dRedLine_Pos += rectWorkRestSub.Width;

                    m_previousItemWidth = rectWorkRestSub.Width;
                    m_previousItemEnd = EndSecond;
                    //m_SumRestTime += EndSecond - StartSecond;
                    continue;
                }
                else if (StartSecond - m_previousItemEnd > 0/* && StartSecond - m_previousItemEnd != 0*/)
                {
                    Console.WriteLine(" ########### Sub ############");
                    Rectangle rectWorkRestSub = new Rectangle();
                    rectWorkRestSub.Width = (StartSecond - m_previousItemEnd) / (60 * 60 * 24) * bWidth;

                    if (workRect.ProcessName == Constants.RestProcess)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                        m_strPreviousProcess = Constants.RestProcess;
                        m_SumRestTime += StartSecond - m_previousItemEnd;
                    }
                    else if (workRect.ProcessName == Constants.DisConnect)
                    {
                        rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                        m_strPreviousProcess = Constants.DisConnect;
                        m_SumDisconnectTime += StartSecond - m_previousItemEnd;
                    }
                    else
                    {
                        if (m_strPreviousProcess == "Work")
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(0, 180, 0));
                            m_strPreviousProcess = "Work";
                        }
                        else if (m_strPreviousProcess == Constants.RestProcess)
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                            m_strPreviousProcess = Constants.RestProcess;
                            //m_SumRestTime += StartSecond - m_previousItemEnd;
                        }
                        else if (m_strPreviousProcess == Constants.DisConnect)
                        {
                            rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                            m_strPreviousProcess = Constants.DisConnect;
                            //m_SumDisconnectTime += StartSecond - m_previousItemEnd;
                        }


                    }

                    //rectWorkRestSub.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    //m_strPreviousProcess = Constants.RestProcess;

                    pnlProgTime.Children.Add(rectWorkRestSub);
                    //m_dRedLine_Pos += rectWorkRestSub.Width;

                    m_previousItemWidth = rectWorkRestSub.Width;
                    m_previousItemEnd = StartSecond;

                }

                Rectangle rectWorkRest = new Rectangle();
                if (workRect.ProcessName == Constants.RestProcess)
                {
                    Console.WriteLine(" ########### RestProcess ############");
                    if (workRect.ProcessName == m_strPreviousProcess)
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = Constants.RestProcess;
                    m_SumRestTime += EndSecond - StartSecond;
                }
                else if (workRect.ProcessName == Constants.DisConnect)
                {
                    Console.WriteLine(" ########### Disconnect ############");
                    if (workRect.ProcessName == m_strPreviousProcess)
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = Constants.DisConnect;

                    m_SumDisconnectTime += EndSecond - StartSecond;

                }
                else
                {
                    Console.WriteLine(" ########### Work ############");
                    if (m_strPreviousProcess == "Work")
                    {
                        this.pnlProgTime.Children.RemoveAt(this.pnlProgTime.Children.Count - 1);
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth + m_previousItemWidth;
                    }
                    else
                    {
                        rectWorkRest.Width = (EndSecond - StartSecond) / (60 * 60 * 24) * bWidth;
                    }

                    rectWorkRest.Fill = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                    this.pnlProgTime.Children.Add(rectWorkRest);
                    //m_dRedLine_Pos += rectWorkRest.Width;

                    m_previousItemWidth = rectWorkRest.Width;
                    m_previousItemEnd = EndSecond;
                    m_strPreviousProcess = "Work";
                }
            }

            //m_SumDisconnectTime = (from processes in m_clientInfo.ProcessList where processes.ProcessName == Constants.DisConnect select processes.ProcessEndTime.Subtract(processes.ProcessStartTime).TotalSeconds).Sum();
            TimeSpan _duration = TimeSpan.FromSeconds(m_SumDisconnectTime);
            string _strDuration = _duration.ToString(@"hh\:mm\:ss");
            Disconnect.Text = _strDuration;



            //m_SumRestTime = (from processes in m_clientInfo.ProcessList where processes.ProcessName == Constants.RestProcess select processes.ProcessEndTime.Subtract(processes.ProcessStartTime).TotalSeconds).Sum();

            TimeSpan _durationRestTime = TimeSpan.FromSeconds(m_SumRestTime);
            string _strDurationRest = _durationRestTime.ToString(@"hh\:mm\:ss");
            Rest.Text = _strDurationRest;

            TimeSpan duration = TimeSpan.FromSeconds(m_SumWorkTime);
            string strDuration = duration.ToString(@"hh\:mm");
            TotalTime.Text = strDuration;

            workTime.Content = strDuration;

            try
            {
                Slide.Text = m_clientInfo.SlideWidth + " X " + m_clientInfo.SlideHeight;
                Capture.Text = m_clientInfo.CaptureHeight + " X " + m_clientInfo.CaptureWidth;

                SessionTime.Text = m_clientInfo.SessionTime + "s";
                CaptureTime.Text = m_clientInfo.CaptureTime + "s";
                ActiveTime.Text = m_clientInfo.ActiveDuration + "s";



                double dFree = 86400 - m_SumWorkTime - m_SumRestTime - m_SumDisconnectTime;

                if (dFree < 0) dFree = 0;


                WorkPie.Values = new ChartValues<double> { m_SumWorkTime };
                RestPie.Values = new ChartValues<double> { m_SumRestTime };
                DisconnectPie.Values = new ChartValues<double> { m_SumDisconnectTime };
                FreePie.Values = new ChartValues<double> { dFree };

                /**********************************************************************/
                WorkRect.Width = 80 * m_SumWorkTime / 86400;
                unWorkRect.Width = 80 - WorkRect.Width;

                RestRect.Width = 80 * m_SumRestTime / 86400;
                unRestRect.Width = 80 - RestRect.Width;

                DisconnectRect.Width = 80 * m_SumDisconnectTime / 86400;
                unDisconnectRect.Width = 80 - DisconnectRect.Width;

                workText.Text = Convert.ToInt32(WorkRect.Width / 80 * 100) + "%";
                restText.Text = Convert.ToInt32(RestRect.Width / 80 * 100) + "%";
                if ((100 - Convert.ToInt32(WorkRect.Width / 80 * 100) - Convert.ToInt32(RestRect.Width / 80 * 100) - Convert.ToInt32(DisconnectRect.Width / 80 * 100)) < 0)
                    disconnectText.Text = (100 - Convert.ToInt32(WorkRect.Width / 80 * 100) - Convert.ToInt32(RestRect.Width / 80 * 100) - Convert.ToInt32(80 * dFree / 86400 / 80 * 100)) + "%";
                else
                    disconnectText.Text = Convert.ToInt32(DisconnectRect.Width / 80 * 100) + "%";
            }
            catch (Exception ex) { }


        }

        private void URLView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            int i = 0;

        }

        private void ExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            string strIP = this.fldIPAddr.Content.ToString();
            string strUser = this.fldName.Content.ToString();
            DateTime selectedDate = Windows.MainWindow.date_Picker.SelectedDate.Value;
            string strDate = selectedDate.Year.ToString() + "-" + selectedDate.Month.ToString() + "-" + selectedDate.Day.ToString();
            string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + strIP + "\\" + "Capture";
            if (!Directory.Exists(strPath))
            {
                CustomMsg msg = new CustomMsg("There are no images.");
                return;
            }
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ExplorerWindow window = new ExplorerWindow(strPath, strIP, strUser, strDate);
                //window.Window_Loaded1(strPath, strIP, strUser, strDate);
                window.ShowDialog();
            });
            //CustomMsg msg = new CustomMsg("This function is developing now.");
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_ChangedSize)
            {
                m_ChangedSize = false;
                return;
            }

            if (m_InitWidth == m_Width)
            {
                return;
            }

            m_InitWidth = m_Width;
            pnlProgTime.Children.Clear();
            pnlProgProcess.Children.Clear();
            pnlProgAudio.Children.Clear();

            //m_dRedLine_Pos = 0;
            m_isFirstAppRect = false;
            m_isFirstProcessRect = false;
            m_isFirstAudioRect = false;

            double width = m_Width;

            int nBeforeCount = m_clientInfo.ProcessList.Count;

            //MergeProcessList(m_clientInfo);

            int nAfterCount = m_clientInfo.ProcessList.Count;

            SetComputerUsageStatus(m_clientInfo.ProcessList.ToList());
            SetProcessStatus(m_clientInfo.ProcessList.ToList());
            m_OldProcessList = m_clientInfo.ProcessList.ToList();
            SetVoiceStatus(m_clientInfo.AudioList);
            m_OldAudioList = m_clientInfo.AudioList.ToList();

            //SetRedLinePos();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CopyURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get the clicked MenuItem
                var menuItem = (MenuItem)sender;

                //Get the ContextMenu to which the menuItem belongs
                var contextMenu = (ContextMenu)menuItem.Parent;

                //Find the placementTarget
                var item = (DataGrid)contextMenu.PlacementTarget;
                var urlItem = item.CurrentItem as URL;

                if (urlItem == null) return;

                string strURL = urlItem.strURL;

                Clipboard.SetText(strURL.Trim());
            }
            catch (Exception ex)
            {

            }

        }

        private void GoToURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get the clicked MenuItem
                var menuItem = (MenuItem)sender;

                //Get the ContextMenu to which the menuItem belongs
                var contextMenu = (ContextMenu)menuItem.Parent;

                //Find the placementTarget
                var item = (DataGrid)contextMenu.PlacementTarget;
                var urlItem = item.CurrentItem as URL;

                if (urlItem == null) return;

                string strURL = urlItem.strURL;

                System.Diagnostics.Process.Start(strURL.Trim());
            }
            catch (Exception ex)
            {

            }

        }

        private void URLView_ContextMenuOpening_1(object sender, ContextMenuEventArgs e)
        {
            if (URLView.Items.Count == 0)
                menu.Visibility = Visibility.Hidden;
            else
                menu.Visibility = Visibility.Visible;
        }

        private void ProcessFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(processFilter.Text))
                processFilter.Text = "Filter by process";
        }

        private void ProcessFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (processFilter.Text == "Filter by process")
            {
                processFilter.Text = "";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                FilterByProcess();
            }
        }

        private void ALLProcessCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ALLProcessCheckBox.IsChecked == true)
            {
                FilterAllProcess();
            }
            else
            {
                list2View.Items.Clear();
                m_ProcessFilter = false;
                ShowProcessTotal(m_clientInfo.ProcessList.ToList());
                //if (Windows.MainWindow.date_Picker.SelectedDate.Value.Year == DateTime.Now.Year && date_Picker.SelectedDate.Value.Month == DateTime.Now.Month && date_Picker.SelectedDate.Value.Day == DateTime.Now.Day)
                //    ShowProcessDetail(Settings.Instance.ProcessList.ToList());
                //else
                //    ShowProcessDetail(Settings.Instance.ProcessList_Day.ToList());
            }
        }

        private void FilterByProcess()
        {
            list2View.Items.Clear();

            if (processFilter.Text.Trim() == "")
            {
                m_ProcessFilter = false;
                ShowProcessTotal(m_clientInfo.ProcessList.ToList());

                return;
            }

            m_ProcessFilter = true;

            var processTemp = m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime).Select(x => new { ProcessName = x.ProcessName, ProcessWindow = x.ProcessWindow, Duration = x.ProcessEndTime.Subtract(x.ProcessStartTime).TotalSeconds, Path = x.ProcessPath }).ToList();
            var processTotal = processTemp.GroupBy(t => t.ProcessWindow).Select(g => new { ProcessName = g.Key, ProcessWindow = g.First().ProcessWindow, Duration = g.Sum(u => u.Duration), Path = g.First().Path });

            foreach (var process in from processes in processTotal where processes.ProcessWindow.ToLower().Contains(processFilter.Text.ToLower().Trim()) select processes)
            {
                if (process.ProcessName == Constants.RestProcess || process.ProcessName == Constants.DisConnect || process.ProcessName == Constants.HideProcess_IDLE || process.ProcessName == Constants.HideProcess_APH || process.ProcessName == Constants.HideProcess_LockApp)
                    continue;
                TimeSpan _duration = TimeSpan.FromSeconds(process.Duration);
                string _strDuration = _duration.ToString(@"hh\:mm\:ss");

                if (_strDuration == "00:00:00")
                    continue;
                Icon icon = null;
                try
                {
                    if (process.Path == "Unknown")
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                    }
                    else if (!File.Exists(process.Path))
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                    }
                    else
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(process.Path);
                    }
                }
                catch
                {
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                }
                BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                string strState = "";
                foreach (var strFP in Settings.Instance.Forbiddenprocess_list.ToList())
                {
                    if (process.ProcessName.ToLower().Contains(strFP.Value.ToLower()))
                    {
                        strState = "Danger";
                        break;
                    }
                }
                string strWindowTemp = process.ProcessWindow;
                if (strWindowTemp == "" || strWindowTemp == Constants.Unknown)
                {
                    strWindowTemp = process.ProcessName;
                }
                list2View.Items.Add(new ProcessTotalItem { List2Icon = bitmapSource, Process = strWindowTemp, Time = _strDuration, State = strState });
            }

        }

        private void FilterAllProcess()
        {
            m_ProcessFilter = true;
            list2View.Items.Clear();

            foreach (var strFP in Settings.Instance.Forbiddenprocess_list.ToList())
            {
                var processTemp = m_clientInfo.ProcessList.OrderByDescending(x => x.ProcessEndTime).Select(x => new { ProcessName = x.ProcessName, ProcessWindow = x.ProcessWindow, Duration = x.ProcessEndTime.Subtract(x.ProcessStartTime).TotalSeconds, Path = x.ProcessPath }).ToList();
                var processTotal = processTemp.GroupBy(t => t.ProcessWindow).Select(g => new { ProcessName = g.Key, ProcessWindow = g.First().ProcessWindow, Duration = g.Sum(u => u.Duration), Path = g.First().Path });

                foreach (var process in from processList in processTotal where processList.ProcessWindow.ToLower().Contains(strFP.Value.ToLower()) select processList)
                {
                    if (process.ProcessName == Constants.RestProcess || process.ProcessName == Constants.DisConnect || process.ProcessName == Constants.HideProcess_IDLE || process.ProcessName == Constants.HideProcess_APH || process.ProcessName == Constants.HideProcess_LockApp)
                        continue;
                    TimeSpan _duration = TimeSpan.FromSeconds(process.Duration);
                    string _strDuration = _duration.ToString(@"hh\:mm\:ss");

                    if (_strDuration == "00:00:00")
                        continue;
                    Icon icon = null;
                    try
                    {
                        if (process.Path == "Unknown")
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                        }
                        else if (!File.Exists(process.Path))
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                        }
                        else
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(process.Path);
                        }
                    }
                    catch
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath[rnd.Next(0, 4)]);
                    }
                    BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());

                    string strWindowTemp = process.ProcessWindow;
                    if (strWindowTemp == "" || strWindowTemp == Constants.Unknown)
                    {
                        strWindowTemp = process.ProcessName;
                    }
                    list2View.Items.Add(new ProcessTotalItem { List2Icon = bitmapSource, Process = strWindowTemp, Time = _strDuration, State = "Danger" });
                }
            }
        }
        private void UrlFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (urlFilter.Text == "Filter by URL")
            {
                urlFilter.Text = "";
            }
        }

        private void PnlProgAudio_MouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.Now.Subtract(m_DeltaProcessTime).TotalMilliseconds > 50)
            {
                GetAudioInfo(e);
                m_DeltaProcessTime = DateTime.Now;
            }
            Console.WriteLine("AUDIO=======> {0}", e.GetPosition(this).Y);
            if (e.GetPosition(this).Y > 140 || e.GetPosition(this).Y < 127 /*|| e.GetPosition(this).X > m_previousAppEnd+5*/)
            {
                try
                {
                    currentMovingShape.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void PnlProgAudio_MouseLeave(object sender, MouseEventArgs e)
        {
            currentMovingShape.Visibility = Visibility.Hidden;
        }

        private void UrlFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlFilter.Text))
                urlFilter.Text = "Filter by URL";
        }

        private void UrlFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                FilterByURL();
            }
        }

        private void ALLURLCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ALLURLCheckBox.IsChecked == true)
            {
                FilterAllURL();
            }
            else
            {
                m_URLFilter = false;
                ShowURL(m_clientInfo.URLList.ToList());
                //if (date_Picker.SelectedDate.Value.Year == DateTime.Now.Year && date_Picker.SelectedDate.Value.Month == DateTime.Now.Month && date_Picker.SelectedDate.Value.Day == DateTime.Now.Day)
                //    ShowURL(Settings.Instance.URLList.ToList());
                //else
                //    ShowURL(Settings.Instance.URLList_Day.ToList());
            }
        }
        private void FilterAllURL()
        {
            m_URLFilter = true;

            string ChromePath32 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            string ChromePath64 = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

            string FirefoxPath32 = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            string FirefoxPath64 = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

            string IEPath = "C:\\Program Files\\Internet Explorer\\iexplore.exe";
            string Edge = "";
            Icon icon = null;
            URLView.Items.Clear();

            try
            {
                foreach (var strFU in Settings.Instance.ForbiddenURLList)
                {

                    foreach (var urlInfo in from urlList in m_clientInfo.URLList.OrderByDescending(x => x.URLEndTime) where urlList.strURL.ToLower().Contains(strFU.ToLower()) select urlList)
                    {
                        if (urlInfo.BrowserType == 1)
                        {
                            if (File.Exists(ChromePath32))
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath32);
                            }
                            else
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath64);
                            }
                        }
                        else if (urlInfo.BrowserType == 2)
                        {
                            if (File.Exists(FirefoxPath32))
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath32);
                            }
                            else
                            {
                                icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath64);
                            }

                        }
                        else if (urlInfo.BrowserType == 3)
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(Edge);
                        }
                        else if (urlInfo.BrowserType == 4)
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(IEPath);
                        }

                        BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                        if (!urlInfo.strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.Updating.ToLower()))
                            URLView.Items.Add(new URL { List2Icon = bitmapSource, strURL = urlInfo.strURL, StartTime = urlInfo.URLStartTime.ToLongTimeString(), EndTime = urlInfo.URLEndTime.ToLongTimeString(), State = "Danger" });

                    }
                }
            }
            catch (Exception ex) { }


        }

        private void FilterByURL()
        {
            string ChromePath32 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            string ChromePath64 = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

            string FirefoxPath32 = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            string FirefoxPath64 = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

            string IEPath = "C:\\Program Files\\Internet Explorer\\iexplore.exe";
            string Edge = "";
            Icon icon = null;
            URLView.Items.Clear();

            if (urlFilter.Text.Trim() == "")
            {
                m_URLFilter = false;
                ShowURL(m_clientInfo.URLList.ToList());

                return;
            }

            m_URLFilter = true;

            try
            {
                var urlLists = from urlList in m_clientInfo.URLList.OrderByDescending(x => x.URLEndTime) where urlList.strURL.ToLower().Contains(urlFilter.Text.ToLower().Trim()) select urlList;
                foreach (var urlInfo in urlLists)
                {
                    if (urlInfo.BrowserType == 1)
                    {
                        if (File.Exists(ChromePath32))
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath32);
                        }
                        else
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(ChromePath64);
                        }
                    }
                    else if (urlInfo.BrowserType == 2)
                    {
                        if (File.Exists(FirefoxPath32))
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath32);
                        }
                        else
                        {
                            icon = System.Drawing.Icon.ExtractAssociatedIcon(FirefoxPath64);
                        }

                    }
                    else if (urlInfo.BrowserType == 3)
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(Edge);
                    }
                    else if (urlInfo.BrowserType == 4)
                    {
                        icon = System.Drawing.Icon.ExtractAssociatedIcon(IEPath);
                    }

                    BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());

                    string strState = "";
                    foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                    {
                        if (urlInfo.strURL.ToLower().Contains(urlFURL.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !urlInfo.strURL.ToLower().Contains(Constants.Updating.ToLower()))
                        {
                            strState = "Danger";
                            break;
                        }
                    }

                    URLView.Items.Add(new URL { List2Icon = bitmapSource, strURL = urlInfo.strURL, StartTime = urlInfo.URLStartTime.ToLongTimeString(), EndTime = urlInfo.URLEndTime.ToLongTimeString(), State = strState });

                }
            }
            catch (Exception ex)
            {

            }

        }
        private void Refresh()
        {
            m_InitWidth = m_Width;
            pnlProgTime.Children.Clear();
            pnlProgProcess.Children.Clear();
            pnlProgAudio.Children.Clear();

            //m_dRedLine_Pos = 0;
            m_isFirstAppRect = false;
            m_isFirstProcessRect = false;
            m_isFirstAudioRect = false;

            double width = m_Width;

            //MergeProcessList(m_clientInfo);

            GetTimeStatus(m_clientInfo.ProcessList.ToList());
            SetProcessStatus(m_clientInfo.ProcessList.ToList());
            m_OldProcessList = m_clientInfo.ProcessList.ToList();
            SetVoiceStatus(m_clientInfo.AudioList);
            m_OldAudioList = m_clientInfo.AudioList.ToList();
        }

        private void GrdHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Height == 30)
            {
                UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 139, 139));
            }
            else
            {
                //UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(200, 0, 175, 175));
                //UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 160, 160));
                //UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(238, 34, 139, 34));
                UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 139, 139));
            }

        }

        private void GrdHeader_MouseMove(object sender, MouseEventArgs e)
        {
            UserInfoHeader.Background = new SolidColorBrush(Color.FromArgb(221, 0, 170, 170));

        }

        private void GrdHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Style style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));

            style.Setters.Add(new Setter
            {
                Property = ForegroundProperty,
                Value = System.Windows.Media.Brushes.White
            });

            style.Setters.Add(new Setter
            {
                Property = BackgroundProperty,
                Value = System.Windows.Media.Brushes.DarkGray
            });
            style.Setters.Add(new Setter
            {
                Property = System.Windows.Controls.Control.HorizontalContentAlignmentProperty,
                Value = HorizontalAlignment.Center

            });


        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(e.Parameter as string);
        }

        private void CopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter as string))
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        public void MergeProcessList(ClientInfo clientinfo)
        {
            if (clientinfo.ProcessServerList.Count == 0) return;

            int j = 0;
            for (int i = 0; i < clientinfo.ProcessList.Count; i++)
            {

                //                if (clientinfo.ProcessList[i].ProcessName == Constants.RestProcess || clientinfo.ProcessList[i].ProcessName == Constants.DisConnect)
                //                {
                //if (clientinfo.ProcessServerList[j].ProcessName == Constants.RestProcess)
                //{
                //    j++;
                //    if (j == clientinfo.ProcessServerList.Count)
                //    {
                //        break;
                //    }

                //    continue;
                //}

                if (clientinfo.ProcessList[i].ProcessStartTime > clientinfo.ProcessServerList[j].ProcessStartTime)
                {
                    for (int k = j; k < clientinfo.ProcessServerList.Count; k++)
                    {
                        if (clientinfo.ProcessList[i].ProcessStartTime <= clientinfo.ProcessServerList[k].ProcessStartTime)
                        {

                            continue;
                        }
                        clientinfo.ProcessList.Insert(i, clientinfo.ProcessServerList[k]);

                        i++;
                        j++;
                        if (j == clientinfo.ProcessServerList.Count)
                        {
                            break;
                        }
                    }
                    if (j == clientinfo.ProcessServerList.Count)
                    {
                        break;
                    }
                }

                if (clientinfo.ProcessList[i].ProcessStartTime <= clientinfo.ProcessServerList[j].ProcessStartTime && clientinfo.ProcessList[i].ProcessEndTime >= clientinfo.ProcessServerList[j].ProcessEndTime)
                {
                    if (clientinfo.ProcessList[i].ProcessStartTime != clientinfo.ProcessServerList[j].ProcessStartTime)
                    {
                        _temp1.ProcessName = Constants.RestProcess;
                        _temp1.ProcessStartTime = clientinfo.ProcessList[i].ProcessStartTime;
                        _temp1.ProcessEndTime = clientinfo.ProcessServerList[j].ProcessStartTime;
                        _temp1.ProcessPath = clientinfo.ProcessServerList[j].ProcessPath;
                        _temp1.ProcessWindow = clientinfo.ProcessServerList[j].ProcessWindow;
                        _temp1.ProcessColor = clientinfo.ProcessServerList[j].ProcessColor;
                    }
                    else
                    {
                        _temp1.ProcessName = "-1";
                    }

                    if (clientinfo.ProcessList[i].ProcessEndTime != clientinfo.ProcessServerList[j].ProcessEndTime)
                    {
                        _temp2.ProcessName = Constants.RestProcess;
                        _temp2.ProcessStartTime = clientinfo.ProcessServerList[j].ProcessEndTime;
                        _temp2.ProcessEndTime = clientinfo.ProcessList[i].ProcessEndTime;
                        _temp2.ProcessPath = clientinfo.ProcessServerList[j].ProcessPath;
                        _temp2.ProcessWindow = clientinfo.ProcessServerList[j].ProcessWindow;
                        _temp2.ProcessColor = clientinfo.ProcessServerList[j].ProcessColor;
                    }
                    else
                    {
                        _temp2.ProcessName = "-1";
                    }

                    clientinfo.ProcessList.RemoveAt(i);

                    if (_temp1.ProcessName != "-1")
                    {
                        clientinfo.ProcessList.Insert(i, _temp1);
                        i++;
                    }

                    clientinfo.ProcessList.Insert(i, clientinfo.ProcessServerList[j]);
                    i++;

                    if (_temp2.ProcessName != "-1")
                    {
                        clientinfo.ProcessList.Insert(i, _temp2);
                    }
                    i--;
                    j++;
                    if (j == clientinfo.ProcessServerList.Count)
                    {
                        break;
                    }
                }

                //               }

            }

            int z = clientinfo.ProcessList.Count - 1;
            if (j != clientinfo.ProcessServerList.Count)
            {
                if (clientinfo.ProcessList[z].ProcessEndTime < clientinfo.ProcessServerList[j].ProcessStartTime)
                {
                    for (int k = j; k < clientinfo.ProcessServerList.Count; k++)
                    {
                        //if (clientinfo.ProcessList[clientinfo.ProcessList.Count-1].ProcessStartTime <= clientinfo.ProcessServerList[k].ProcessStartTime)
                        //{

                        //    continue;
                        //}
                        clientinfo.ProcessList.Insert(z, clientinfo.ProcessServerList[k]);

                        z++;
                        j++;
                        if (j == clientinfo.ProcessServerList.Count)
                        {
                            break;
                        }
                    }
                    
                }
            }
            
        }
    }
    public class Item
    {
        public Rectangle m_Shape;
        public string m_strURI;
        public Item(Canvas canvas, string strUri)
        {
            ImageSource imageSource;
            //string str = @"D:\Work\result.jpg";
            try
            {
                imageSource = new BitmapImage(new Uri(strUri));
                m_Shape = new Rectangle
                {
                    Width = imageSource.Width * 0.5,
                    Height = imageSource.Height * 0.5,
                    Fill = new ImageBrush
                    {
                        //ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resource/result.jpg"))
                        ImageSource = new BitmapImage(new Uri(strUri))
                    }
                };
                canvas.Children.Add(m_Shape);

            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
                //MessageBox.Show("this is Exception", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //SetPosition(0.0, 0.0);
        }

        public void RemoveShape(Canvas canvas)
        {
            canvas.Children.Remove(m_Shape);
        }
        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(m_Shape, x);
            Canvas.SetTop(m_Shape, y);
        }





    }
    public class ProcessTotalItem
    {
        public BitmapSource List2Icon { get; set; }
        public string Process { get; set; }
        public string Time { get; set; }
        public string State { get; set; }
    }
    public class URL
    {
        public BitmapSource List2Icon { get; set; }
        public string strURL { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string State { get; set; }
    }
}
