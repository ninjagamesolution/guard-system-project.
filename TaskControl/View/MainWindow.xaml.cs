using Monitor.TaskControl.Globals;
using Monitor.TaskControl.myEvents;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Windows.Threading;
using System.Windows.Interop;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.Logger;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Data;
using System.Diagnostics;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;

        const uint SC_CLOSE = 0xF060;


        public List<pnlSample> m_UserInfoList;
 //       public int m_TimeCount;
        public bool fVersion = false;
        public string strClientIP = "";
        SaveProcessInfo saveProcessInfo = new SaveProcessInfo();
        URLProc urlProc = new URLProc();
        AudioProc audioInfo = new AudioProc();
        public PatchProc patchProc;
        bool bDatePicker;
        List<string> strListTemp = new List<string>();

        public DispatcherPriority Make { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            
            bDatePicker = false;
            Events.RaiseOnMainProc();
            SetTrayIcon();
            m_UserInfoList = new List<pnlSample>();

            date_Picker.SelectedDate = DateTime.Now;


            ForbiddenProc.checkForbiddenFile();
            ForbiddenProc.LoadForbiddenProcessList();

            AllowURLProc.checkAllowURLFile();
            AllowURLProc.LoadAllowURLList();

            ForbiddenURLProc.checkForbiddenURLFile();
            ForbiddenURLProc.LoadForbiddenURLList();

            patchProc = new PatchProc();
            //DispatcherTimer dtClockTime = new DispatcherTimer();
            //dtClockTime.Interval = new TimeSpan(0, 0, 1); //in Hour, Minutes, Second.
            //dtClockTime.Tick += TimerTick;
            //dtClockTime.Start();
            //m_TimeCount = 0;

        }



        private void imgBackButton_Click(object sender, MouseEventArgs e)
        {
            //pnlPanels.Children.Clear();
            //m_UserInfoList.Clear();
            bDatePicker = true;
            DateTime tempDate = (DateTime)date_Picker.SelectedDate;
            date_Picker.SelectedDate = tempDate.AddDays(-1);
            //tempDate = (DateTime)date_Picker.SelectedDate;
            OnShowUrlPage();
            OnShowSettingPage();
            OnShowDownloadPage();
            OnShowAudio();
            return;
        }
        private void imgForwardButton_Click(object sender, MouseEventArgs e)
        {
            if (date_Picker.SelectedDate.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                return;
            }
            bDatePicker = true;
            //pnlPanels.Children.Clear();
            //m_UserInfoList.Clear();
            DateTime tempDate = (DateTime)date_Picker.SelectedDate;
            date_Picker.SelectedDate = tempDate.AddDays(1);
            //tempDate = (DateTime)date_Picker.SelectedDate;
            OnShowUrlPage();
            OnShowSettingPage();
            OnShowDownloadPage();
            OnShowAudio();
            return;

        }

        private void imgTodayButton_Click(object sender, RoutedEventArgs e)
        {
            bDatePicker = true;
            //pnlPanels.Children.Clear();
            //m_UserInfoList.Clear();
            //DateTime tempDate = (DateTime)date_Picker.SelectedDate;
            // tempDate = (DateTime)date_Picker.SelectedDate;

            date_Picker.SelectedDate = DateTime.Now;
            OnShowUrlPage();
            OnShowSettingPage();
            OnShowDownloadPage();
            OnShowAudio();
            return;

        }

        void imgBackMouseMove(object sender, MouseEventArgs e)
        {
            imgBack.Opacity = 0.6;
        }

        void imgBackMouseLeave(object sender, MouseEventArgs e)
        {
            imgBack.Opacity = 1;
        }

        void imgForwardMouseMove(object sender, MouseEventArgs e)
        {
            imgForward.Opacity = 0.6;
        }

        void imgForwardMouseLeave(object sender, MouseEventArgs e)
        {
            imgForward.Opacity = 1;
        }

        private void PnlMessages_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
        }


        private void OnShowData(string strDate)
        {
            string strDBFilePath = "";
            string strDBServerFilePath = "";
            string strUrlFilePath = "";
            string strDownloadPath = "";
            string strAudioPath = "";
            string strToday = string.Format("{0}-{1}-{2}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            //Log.Instance.DoLog("OnShowData =====>Before LoadUserListPerDay ", Log.LogType.Info);
            LoadUserListPerDay(strDate);
            ///Log.Instance.DoLog("OnShowData =====>After LoadUserListPerDay ", Log.LogType.Info);
            foreach (var client in Settings.Instance.ClientDic_Temp)
            {
                client.Value.NetworkState = false;
                strDBFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\";
                strDBFilePath = strDBFilePath + client.Value.ClientIP + "\\";
                strDBFilePath += Constants.DbFileName;
                client.Value.ProcessList = new List<ListOfProcessByOrder>();
                List<string> strTempList = new List<string>();
                strTempList = Md5Crypto.ReadCryptoFile(strDBFilePath);

                strDBServerFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\";
                strDBServerFilePath = strDBServerFilePath + client.Value.ClientIP + "\\";
                strDBServerFilePath += Constants.DbServerFileName;
                List<string> strServerTempList = new List<string>();
                if (File.Exists(strDBServerFilePath))
                {
                    client.Value.ProcessServerList = new List<ListOfProcessByOrder>();

                    strServerTempList = Md5Crypto.ReadCryptoFile(strDBServerFilePath);
                }



                strAudioPath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\";
                strAudioPath = strAudioPath + client.Value.ClientIP + "\\";
                strAudioPath += Constants.AudioFileName;
                client.Value.AudioList = new List<ListOfAudio>();
                List<string> strAudioTempList = new List<string>();
                strAudioTempList = Md5Crypto.ReadCryptoFile(strAudioPath);

                strUrlFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\";
                strUrlFilePath = strUrlFilePath + client.Value.ClientIP + "\\";
                strUrlFilePath += Constants.urlFileName;
                client.Value.URLList = new List<ListOfUrl>();
                List<string> strURLTempList = new List<string>();
                strURLTempList = Md5Crypto.ReadCryptoFile(strUrlFilePath);

                List<string> strDownloadTempList = new List<string>();
                strDownloadPath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\" + Constants.DownloadFileName;

                Settings.Instance.DownloadList = Md5Crypto.ReadCryptoFile(strDownloadPath);

                strDownloadTempList = Md5Crypto.ReadCryptoFile(strDownloadPath);
                int nDownload = 0;
                foreach (var strDownload in strDownloadTempList)
                {
                    if (strDownload.Contains(client.Value.ClientIP))
                    {
                        nDownload++;
                    }
                }

                String[] spearator = { Constants.filePattern };

                foreach (var line in strTempList)
                {
                    try
                    {
                        String[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        ListOfProcessByOrder temp;
                        temp.ProcessName = strArray[0];
                        temp.ProcessWindow = strArray[1];
                        temp.ProcessPath = strArray[2];
                        temp.ProcessStartTime = DateTime.Parse(strArray[3]);
                        temp.ProcessEndTime = DateTime.Parse(strArray[4]);
                        try
                        {
                            temp.ProcessColor = strArray[5];
                        }
                        catch
                        {
                            temp.ProcessColor = Constants.Default;
                        }
                        client.Value.ProcessList.Add(temp);
                    }
                    catch
                    {

                    }
                }

                foreach (var line in strServerTempList)
                {
                    try
                    {
                        String[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        ListOfProcessByOrder temp;
                        temp.ProcessName = strArray[0];
                        temp.ProcessWindow = strArray[1];
                        temp.ProcessPath = strArray[2];
                        temp.ProcessStartTime = DateTime.Parse(strArray[3]);
                        temp.ProcessEndTime = DateTime.Parse(strArray[4]);
                        try
                        {
                            temp.ProcessColor = strArray[5];
                        }
                        catch
                        {
                            temp.ProcessColor = Constants.Default;
                        }
                        client.Value.ProcessServerList.Add(temp);
                    }
                    catch
                    {

                    }
                }

                foreach (var line in strAudioTempList)
                {
                    try
                    {
                        String[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        ListOfAudio temp;
                        temp.ProcessName = strArray[0];
                        temp.ProcessWindow = strArray[1];
                        temp.ProcessPath = strArray[2];
                        temp.ProcessStartTime = DateTime.Parse(strArray[3]);
                        temp.ProcessEndTime = DateTime.Parse(strArray[4]);
                        temp.FileName = strArray[5];
                        temp.FileSize = strArray[6];
                        client.Value.AudioList.Add(temp);
                    }
                    catch
                    {

                    }
                }


                foreach (var line in strURLTempList)
                {
                    //Log.Instance.DoLog(client.Value.ClientIP, Log.LogType.Info);
                    String[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    ListOfUrl temp;
                    temp.strWindow = strArray[0];
                    temp.strURL = strArray[1];
                    temp.URLStartTime = DateTime.Parse(strArray[2]);
                    temp.URLEndTime = DateTime.Parse(strArray[3]);
                    temp.BrowserType = (byte)Int32.Parse(strArray[4]);

                    client.Value.URLList.Add(temp);

                }
                //client.Value.ProcessList = testList;

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    pnlSample pnlMember = new pnlSample(client.Value, this.Width - 126);
                    pnlMember.m_DatePickerFlag = true;
                    pnlMember.fldDownload.Content = nDownload.ToString();
                    pnlPanels.Children.Add(pnlMember);
                    m_UserInfoList.Add(pnlMember);

                });

            }
            Log.Instance.DoLog("OnShowData ==========> END ", Log.LogType.Info);
            OnShowUrlPage();
            OnShowSettingPage();
            OnShowAudio();
        }
        private void OnShowForbidden(object sender, MouseEventArgs e)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenFileName;
            Forbidder.Items.Clear();
            if (!File.Exists(strFilePath))
            {
                return;
            }
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                Forbidder.Items.Add(new ForbiddenProcess { ID = nID, GName = line.Split(',')[1], PName = line.Split(',')[0] });

                nID++;
            }


        }

        private void OnShowAllowURL(object sender, MouseEventArgs e)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.AllowURLFileName;
            AllowURL.Items.Clear();
            if (!File.Exists(strFilePath))
            {
                return;
            }
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                AllowURL.Items.Add(new AllowURL { ID = nID, allowURL = line });

                nID++;
            }
        }

        private void OnShowForbiddenURL(object sender, MouseEventArgs e)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenURLFileName;
            ForbiddenURL.Items.Clear();
            if (!File.Exists(strFilePath))
            {
                return;
            }
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                ForbiddenURL.Items.Add(new ForbiddenURL { ID = nID, forbiddenURL = line });

                nID++;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtProcess.Text == "")
            {
                return;
            }
            if (txtProgram.Text == "")
            {
                return;
            }
            Settings.Instance.Forbiddenprocess_list.Add(txtProcess.Text, txtProgram.Text);

            ForbiddenProc.AddForbiddenList();

            txtProgram.Text = "";
            txtProcess.Text = "";
            DisplayForbiddenProcess();
        }

        private void btnAddURL_Click(object sender, RoutedEventArgs e)
        {
            if (txtAllowURL.Text == "")
            {
                return;
            }
            Settings.Instance.AllowURLList.Add(txtAllowURL.Text);

            AllowURLProc.AddAllowURLList();

            txtAllowURL.Text = "";
            DisplayAllowURLList();
        }

        private void btnAddFURL_Click(object sender, RoutedEventArgs e)
        {
            if (txtForbiddenURL.Text == "")
            {
                return;
            }
            Settings.Instance.ForbiddenURLList.Add(txtForbiddenURL.Text);

            ForbiddenURLProc.AddForbiddenURLList();

            txtForbiddenURL.Text = "";
            DisplayForbiddenURLList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Forbidder.SelectedItem == null)
            {
                return;
            }
            ConditionMsg confirmMessage = new ConditionMsg("Are you sure to delete the item?");
            if (ConditionMsgWindow.bDelete == false)
            {
                return;
            }
            var cellInfo = Forbidder.SelectedItems[0] as ForbiddenProcess;
            string PName = cellInfo.PName;

            ForbiddenProc.DeleteForbiddenList(PName);
            DisplayForbiddenProcess();
            ForbiddenProc.LoadForbiddenProcessList();
        }

        private void btnDeleteURL_Click(object sender, RoutedEventArgs e)
        {
            if (AllowURL.SelectedItem == null)
            {
                return;
            }
            ConditionMsg confirmMessage = new ConditionMsg("Are you sure to delete the item?");
            if (ConditionMsgWindow.bDelete == false)
            {
                return;
            }
            var cellInfo = AllowURL.SelectedItems[0] as AllowURL;
            string PName = cellInfo.allowURL;

            AllowURLProc.DeleteAllowURLList(PName);
            DisplayAllowURLList();
            AllowURLProc.LoadAllowURLList();
        }

        private void btnDeleteFURL_Click(object sender, RoutedEventArgs e)
        {
            if (ForbiddenURL.SelectedItem == null)
            {
                return;
            }
            ConditionMsg confirmMessage = new ConditionMsg("Are you sure to delete the item?");
            if (ConditionMsgWindow.bDelete == false)
            {
                return;
            }
            var cellInfo = ForbiddenURL.SelectedItems[0] as ForbiddenURL;
            string PName = cellInfo.forbiddenURL;

            ForbiddenURLProc.DeleteForbiddenURLList(PName);
            DisplayForbiddenURLList();
            ForbiddenURLProc.LoadForbiddenURLList();
        }

        private void DisplayForbiddenProcess()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenFileName;
            Forbidder.Items.Clear();
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                Forbidder.Items.Add(new ForbiddenProcess { ID = nID, GName = line.Split(',')[1], PName = line.Split(',')[0] });

                nID++;
            }

            //foreach ()
        }

        private void DisplayAllowURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.AllowURLFileName;
            AllowURL.Items.Clear();
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                AllowURL.Items.Add(new AllowURL { ID = nID, allowURL = line });

                nID++;
            }
        }

        private void DisplayForbiddenURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenURLFileName;
            ForbiddenURL.Items.Clear();
            strListTemp.Clear();
            strListTemp = Md5Crypto.ReadCryptoFile(strFilePath);
            int nID = 1;
            foreach (string line in strListTemp)
            {
                ForbiddenURL.Items.Add(new ForbiddenURL { ID = nID, forbiddenURL = line });

                nID++;
            }
        }

        private void imgBack_MouseLeave(object sender, MouseEventArgs e)
        {
            //  System.Drawing.Image img = ((System.Drawing.Image)sender);
            //    img.Height = img.ActualHeight * 1.1;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            //if (m_TimeCount % 60 == 0)
            //{
            //    ShowUserList();
            //}

            //if (m_TimeCount == 1200) m_TimeCount = 0;

            //m_TimeCount++;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Content = Constants.version;
            WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 0, UseAeroCaptionButtons = false });

            string strBase = Settings.Instance.RegValue.BaseDirectory;
            strBase = strBase.Substring(0, 1);
            DriveInfo dDrive = new DriveInfo(strBase);

            // When the drive is accessible..
            if (dDrive.IsReady)
            {
                if (dDrive.AvailableFreeSpace < 1073741824)
                    DiskSpace.Content = "There is less than 1 GB of free space on the disk " + strBase + ".";
                else
                    DiskSpace.Content = "";

            }
            //1073741824

            LoadUserList();
            ShowOsHistory();
            ShowServerSetting();
            LoadMessageList();
            LoadDownloadInfo();

           // Events.RaiseOnMainProc();

            //OsHistory.RowHeight = 26;
            //OsHistory.IsEnabled = false;
            //USB_Grid.IsEnabled = false;
            MemeberForbidder.IsEnabled = false;
        }


        public void LoadDownloadInfo()
        {
            Settings.Instance.DownloadList.Clear();
            string strPath = Settings.Instance.Directories.TodayDirectory + "\\" + Constants.DownloadFileName;
            Settings.Instance.DownloadList = Md5Crypto.ReadCryptoFile(strPath);
        }

        public void ShowUserList(ClientInfo client)
        {
            try
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    pnlSample pnlMember = new pnlSample(client, this.Width - 126);

                    //pnlMember.btnShowState.Click += new RoutedEventHandler(btnShowState_Click);
                    pnlPanels.Children.Add(pnlMember);


                    m_UserInfoList.Add(pnlMember);

                });
            }

            catch
            {

            }

        }

        public void ReplaceUserList(ClientInfo client)
        {


            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                foreach (var obj in m_UserInfoList)
                {
                    if (obj.fldIPAddr.Content.ToString() == client.ClientIP)
                    {
                        pnlPanels.Children.Remove(obj);
                        m_UserInfoList.Remove(obj);

                        break;
                    }
                }
                pnlSample pnlMember = new pnlSample(client, this.Width - 126);

                //pnlMember.btnShowState.Click += new RoutedEventHandler(btnShowState_Click);
                pnlPanels.Children.Add(pnlMember);


                m_UserInfoList.Add(pnlMember);

            });
        }

        public void ShowNewDay()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Thread.Sleep(100);
                pnlPanels.Children.Clear();
                m_UserInfoList.Clear();

                date_Picker.SelectedDate = DateTime.Now;

                Thread.Sleep(100);
            });
        }
        public void LoadMessageList()
        {
            try
            {

                string strFileName = Settings.Instance.Directories.WorkDirectory + @"\" + Constants.MessageFileName;
                if (!File.Exists(strFileName))
                    return;
                var messageList = Md5Crypto.ReadCryptoFile(strFileName);
                Settings.Instance.MessageList.Clear();
                foreach (string item in messageList)
                {
                    if (item == "")
                        continue;
                    string[] items = item.Split('*');
                    CustomMessage message = new CustomMessage()
                    {
                        index = items[0],
                        clientIP = items[1],
                        name = items[2],
                        message = items[3],
                        time = items[4],
                        type = items[5],
                        state = Convert.ToBoolean(items[6])
                    };
                    ShowMessageList(message);
                    Settings.Instance.MessageList.Add(message);
                    if (Settings.Instance.MessageList.Count == 50)
                    {
                        break;
                    }
                }
                LoadMessageHistory();
                LoadState();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void LoadMessageHistory()
        {
            foreach (var item in Settings.Instance.ClientDic.ToList())
            {
                var client = m_UserInfoList.Where(m => m.fldIPAddr.Content.ToString() == item.Key).FirstOrDefault();
                if (client == null)
                {
                    return;
                }
                var count = Settings.Instance.MessageList.Where(c => c.state == false).Where(m => m.clientIP == item.Key).ToList().Count;
                if (count > 0)
                {

                    client.message_body.Visibility = Visibility.Visible;
                    client.message_count.Content = count.ToString();
                }
                else
                {
                    client.message_body.Visibility = Visibility.Hidden;
                }
            }
        }
        public void ShowMessageList(CustomMessage message)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                pnlMessage pnlMember = new pnlMessage(message);
                pnlMessages.Children.Insert(0, pnlMember);
                if (Settings.Instance.MessageList.Count > 50)
                {
                    Settings.Instance.MessageList.RemoveRange(0, Settings.Instance.MessageList.Count - 50);
                }
                pnlMessages.Children.RemoveRange(50, Settings.Instance.MessageList.Count - 50);
                LoadMessageHistory();
            });
        }

        public void SaveMessage(string strType, string clientIP, string strMessage)
        {
            try
            {

                string index = Guid.NewGuid().ToString();
                string time = DateTime.Now.ToString();
                string name = Settings.Instance.ClientDic[clientIP].UserName;

                string strFileName = Settings.Instance.Directories.WorkDirectory + @"\" + Constants.MessageFileName;
                if (File.Exists(Settings.Instance.Directories.WorkDirectory + @"\MessageList.lib"))
                {
                    File.Delete(Settings.Instance.Directories.WorkDirectory + @"\MessageList.lib");
                }
                if (!File.Exists(strFileName))
                {
                    File.Create(strFileName);
                }
                string strData = index + "*" + clientIP + "*" + name + "*" + strMessage + "*" + time + "*" + strType + "*" + "false";
                Md5Crypto.WriteCryptoFileAppend(Settings.Instance.Directories.WorkDirectory, Constants.MessageFileName, strData);

                CustomMessage message = new CustomMessage()
                {
                    index = index,
                    clientIP = clientIP,
                    name = name,
                    message = strMessage,
                    time = time,
                    type = strType,
                    state = false
                };
                Settings.Instance.MessageList.Add(message);
                if (Settings.Instance.MessageList.Count > 50)
                {
                    Settings.Instance.MessageList.RemoveRange(0, Settings.Instance.MessageList.Count - 50);
                }
                ShowMessageList(message);
                LoadState();
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }
        }

        public void LoadState()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                int count = Settings.Instance.MessageList.Where(x => x.state == false).Count();

                if (count > 0)
                {
                    this.eliMessage.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.eliMessage.Fill = new SolidColorBrush(Colors.LightGray);
                }

            });
            LoadMessageHistory();
        }

        public void ShowServerSetting()
        {
            MessageListCounts.Text = Settings.Instance.RegValue.MessageListCount.ToString();
            DelDataDay.Text = Settings.Instance.RegValue.DelDataDay.ToString();
            WorkingFolder.Text = Settings.Instance.RegValue.BaseDirectory;
        }

        public void LoadUserList()
        {
            //try
            //{
            Settings.Instance.ClientDic.Clear();
            Settings.Instance.ClientDic_Temp.Clear();
            strListTemp.Clear();
            string strFileName = Settings.Instance.Directories.TodayDirectory + @"\" + Constants.UserFileName;
            if (!File.Exists(strFileName))
            {
                strFileName = Settings.Instance.Directories.TodayDirectory + "\\UserInformation.lib";
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
                if (user == "")
                {
                    continue;
                }

                string[] items = user.Split('*');
                ClientInfo client = new ClientInfo()
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
                    OSDate = items[11],
                };
                try
                {
                    client.ActiveDuration = Convert.ToInt32(items[12]);
                }
                catch
                {
                    client.ActiveDuration = Constants.ActiveDuration;
                }

                
                List<string> checkedList = new List<string>();

                strFileName = Settings.Instance.Directories.TodayDirectory + "\\" + items[0] + "\\" + Constants.InspectFileName;

                if (File.Exists(strFileName))
                {
                    checkedList = Md5Crypto.ReadCryptoFile(strFileName);
                }

                client.ReadCaptureCount = 0;

                foreach ( string element in checkedList)
                {
                    client.checkedMap.Add(element, true);
                    client.ReadCaptureCount++;
                }
                
                Settings.Instance.ClientDic.Add(items[0], client);

                string strDbFilePath = Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP;
                saveProcessInfo.LoadProcessInfo(strDbFilePath, client.ClientIP);
                saveProcessInfo.LoadClientProcessInfo(strDbFilePath, client.ClientIP);
                urlProc.LoadURLInfo(strDbFilePath, client.ClientIP);
                audioInfo.LoadAudioInfo(strDbFilePath, client.ClientIP);
                ShowUserList(client);
            }

            //}
            //catch (Exception ex)
            //{
            //    CustomEx.DoExecption(Constants.exResume, ex);
            //}
        }


        public void LoadUserListPerDay(string strDate)
        {
            Settings.Instance.ClientDic_Temp.Clear();
            strListTemp.Clear();
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
                try
                {
                    client1.ActiveDuration = Convert.ToInt32(items[12]);
                }
                catch
                {
                    client1.ActiveDuration = Constants.ActiveDuration;
                }


                List<string> checkedList = new List<string>();

                strFileName = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\" + Constants.InspectFileName;

                if (File.Exists(strFileName))
                {
                    checkedList = Md5Crypto.ReadCryptoFile(strFileName);
                }

                if (Directory.Exists(Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\Capture"))
                {
                    client1.TotalCaptureCount = Directory.GetFiles(Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + items[0] + "\\Capture", "*", SearchOption.TopDirectoryOnly).Length;
                }
                else
                {
                    client1.TotalCaptureCount = 0;
                }
                client1.ReadCaptureCount = 0;

                foreach (string element in checkedList)
                {
                    client1.checkedMap.Add(element, true);
                    client1.ReadCaptureCount++;
                }

                Settings.Instance.ClientDic_Temp.Add(items[0], client1);
                //ShowUserList(client);
                //client1.ReadCaptureCount = 1234567;
            }
        }
        public void SaveUserInformation(ClientInfo client)
        {
            try
            {
                strListTemp.Clear();
                string strPath = Settings.Instance.Directories.TodayDirectory;
                string strName = Constants.UserFileName;
                strListTemp = Md5Crypto.ReadCryptoFile(strPath + "\\" + strName);
                strListTemp.Add(client.ClientIP + "*" + client.UserName + "*" + client.Password + "*" + client.Company + "*" + client.SessionTime + "*" + client.CaptureTime + "*" + client.SlideWidth + "*" + client.SlideHeight + "*" + client.CaptureWidth + "*" + client.CaptureHeight + "*" + client.PCName + "*" + client.OSDate + "*" + client.ActiveDuration);

                Md5Crypto.WriteCryptoFile(strPath, strName, strListTemp);
                if (client.ProcessList.Count == 0)
                {
                    saveProcessInfo.LoadProcessInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                    saveProcessInfo.LoadClientProcessInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                    audioInfo.LoadAudioInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                    urlProc.LoadURLInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                }

            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
        }

        public void DeletedUserLoadInfo(ClientInfo client)
        {
            if (client.ProcessList.Count == 0)
            {
                saveProcessInfo.LoadProcessInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                saveProcessInfo.LoadClientProcessInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                audioInfo.LoadAudioInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
                urlProc.LoadURLInfo(Settings.Instance.Directories.TodayDirectory + "\\" + client.ClientIP, client.ClientIP);
            }
        }

        public void ReplaceUserInfomation()
        {
            try
            {
                strListTemp.Clear();
                string strPath = Settings.Instance.Directories.TodayDirectory;
                string strName = Constants.UserFileName;
                foreach (var client in Settings.Instance.ClientDic.ToList())
                {
                    strListTemp.Add(client.Value.ClientIP + "*" + client.Value.UserName + "*" + client.Value.Password + "*" + client.Value.Company + "*" + client.Value.SessionTime + "*" + client.Value.CaptureTime + "*" + client.Value.SlideWidth + "*" + client.Value.SlideHeight + "*" + client.Value.CaptureWidth + "*" + client.Value.CaptureHeight + "*" + client.Value.PCName + "*" + client.Value.OSDate + "*" + client.Value.ActiveDuration);
                }

                Md5Crypto.WriteCryptoFile(strPath, strName, strListTemp);
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
        }

        public void ShowUSBHistory()
        {


        }

        public void ShowOsHistory()
        {
            //this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            //{
            //    try
            //    {
            //        OsHistory.Items.Clear();
            //        int i = 1;
            //        foreach (var item in Settings.Instance.ClientDic.ToList())
            //        {

            //            OsHistory.Items.Add(new OsHistory { ID = i.ToString(),  IP = item.Value.ClientIP, User = item.Value.UserName, Name = item.Value.PCName, OsDate = item.Value.OSDate });
            //            i++;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        CustomEx.DoExecption(Constants.exResume, ex);
            //    }
            //});
        }

        private void URLCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (strClientIP == "")
            {
                return;
            }

            MemeberURL.Items.Clear();

            OnFilterURLPage();
        }

        private void URLFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (urlFilter.Text == "Filter by url name.")
            {
                urlFilter.Text = "";
            }
        }

        private void URLFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlFilter.Text))
                urlFilter.Text = "Filter by url name.";
        }

        private void OnKeyURLHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MemeberURL.Items.Clear();
                OnFilterURLPage();
            }
        }

        private void OnFilterURLPage()
        {
            ClientInfo item = new ClientInfo();

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                item = Settings.Instance.ClientDic[strClientIP];
            }
            else
            {
                item = Settings.Instance.ClientDic_Temp[strClientIP];
            }

            string ChromePath32 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            string ChromePath64 = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

            string FirefoxPath32 = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            string FirefoxPath64 = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
            Icon icon = null;

            int nID = 1;

            for (int i = item.URLList.ToList().Count - 1; i >= 0; i--)
            {
                try
                {

                    if (item.URLList.ToList()[i].BrowserType == 1)
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
                    else if (item.URLList.ToList()[i].BrowserType == 2)
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

                    if (urlFilter.Text.ToLower() == "filter by url name." || urlFilter.Text == "")
                    {
                        if (chURLFilter.IsChecked == false)
                        {
                            BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                             
                            string strState = "";
                            foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                            {
                                if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFURL.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower()))
                                {
                                    strState = "Danger";
                                    break;
                                }

                            }
                            MemeberURL.Items.Add(new pnlUrl_Client.MemeberURLList { ID = nID, Type = bitmapSource, Window = "  " + item.URLList.ToList()[i].strWindow, Url = "  " + item.URLList.ToList()[i].strURL, StartTime = item.URLList.ToList()[i].URLStartTime.ToString("HH-mm-ss"), EndTime = item.URLList.ToList()[i].URLEndTime.ToString("HH-mm-ss"), State = strState });

                            
                            nID++;
                            continue;
                        }
                    }

                    if (chURLFilter.IsChecked == true)
                    {
                        foreach (var strDangerURL in Settings.Instance.ForbiddenURLList)
                        {
                            if (item.URLList.ToList()[i].strURL.ToLower().Contains(strDangerURL.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower().Trim()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower().Trim()))
                            {
                                if (urlFilter.Text.ToLower() != "filter by url name." && urlFilter.Text != "")
                                {
                                    if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFilter.Text.ToLower()))
                                    {
                                        BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                                        string strState = "";
                                        foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                                        {
                                            if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFURL.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower()))
                                            {
                                                strState = "Danger";
                                                break;
                                            }

                                        }
                                        MemeberURL.Items.Add(new pnlUrl_Client.MemeberURLList { ID = nID, Type = bitmapSource, Window = "  " + item.URLList.ToList()[i].strWindow, Url = "  " + item.URLList.ToList()[i].strURL, StartTime = item.URLList.ToList()[i].URLStartTime.ToString("HH-mm-ss"), EndTime = item.URLList.ToList()[i].URLEndTime.ToString("HH-mm-ss"), State = strState });

                                        nID++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                                    string strState = "";
                                    foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                                    {
                                        if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFURL.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower()))
                                        {
                                            strState = "Danger";
                                            break;
                                        }

                                    }
                                    MemeberURL.Items.Add(new pnlUrl_Client.MemeberURLList { ID = nID, Type = bitmapSource, Window = "  " + item.URLList.ToList()[i].strWindow, Url = "  " + item.URLList.ToList()[i].strURL, StartTime = item.URLList.ToList()[i].URLStartTime.ToString("HH-mm-ss"), EndTime = item.URLList.ToList()[i].URLEndTime.ToString("HH-mm-ss"), State = strState });

                                    nID++;
                                    continue;
                                }
                            }
                        }
                    }
                    if (urlFilter.Text.ToLower() != "filter by url name." && urlFilter.Text != "")
                    {
                        if (chURLFilter.IsChecked == false)
                        {
                            if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFilter.Text.Trim().ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower().Trim()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower().Trim()))
                            {
                                BitmapSource bitmapSource = ConvertBitmap(icon.ToBitmap());
                                string strState = "";
                                foreach (var urlFURL in Settings.Instance.ForbiddenURLList.ToList())
                                {
                                    if (item.URLList.ToList()[i].strURL.ToLower().Contains(urlFURL.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.TranslateCom.ToLower()) && !item.URLList.ToList()[i].strURL.ToLower().Contains(Constants.Updating.ToLower()))
                                    {
                                        strState = "Danger";
                                        break;
                                    }

                                }
                                MemeberURL.Items.Add(new pnlUrl_Client.MemeberURLList { ID = nID, Type = bitmapSource, Window = "  " + item.URLList.ToList()[i].strWindow, Url = "  " + item.URLList.ToList()[i].strURL, StartTime = item.URLList.ToList()[i].URLStartTime.ToString("HH-mm-ss"), EndTime = item.URLList.ToList()[i].URLEndTime.ToString("HH-mm-ss"), State = strState });

                                nID++;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
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

        public class MemeberURLListTemp
        {
            public int ID { get; set; }
            public BitmapSource Type { get; set; }
            public string Window { get; set; }
            public string Url { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string State { get; set; }
        }

        private void ShowURLPage(object sender, MouseButtonEventArgs e)
        {
            OnShowUrlPage();
        }

        private void ShowDownloadPage(object sender, MouseButtonEventArgs e)
        {
            chVideo.IsChecked = false;
            chAudio.IsChecked = false;
            chImage.IsChecked = false;
            OnShowDownloadPage();
        }

        private void DownloadFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtFilter.Text == "Filter by download file name.")
            {
                txtFilter.Text = "";
            }
        }

        private void DownloadFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilter.Text))
                txtFilter.Text = "Filter by download file name.";
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                FilterByDownload();
            }
        }

        private void VideoCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (chVideo.IsChecked == true)
            {
                FilterByDownload();
            }
            else
            {
                if (txtFilter.Text.ToLower() == "filter by download file name." && chVideo.IsChecked == false && chAudio.IsChecked == false && chImage.IsChecked == false)
                {
                    OnShowDownloadPage();
                    return;
                }
                else
                {
                    FilterByDownload();
                }
            }
        }

        private void AudioCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (chAudio.IsChecked == true)
            {
                FilterByDownload();
            }
            else
            {
                if (txtFilter.Text.ToLower() == "filter by download file name." && chVideo.IsChecked == false && chAudio.IsChecked == false && chImage.IsChecked == false)
                {
                    OnShowDownloadPage();
                    return;
                }
                else
                {
                    FilterByDownload();
                }
            }
        }

        private void ImageCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (chImage.IsChecked == true)
            {
                FilterByDownload();
            }
            else
            {
                if (txtFilter.Text.ToLower() == "filter by download file name." && chVideo.IsChecked == false && chAudio.IsChecked == false && chImage.IsChecked == false)
                {
                    OnShowDownloadPage();
                    return;
                }
                else
                {
                    FilterByDownload();
                }
            }
        }

        private void FilterByDownload()
        {
            DownLoadPanel.Items.Clear();
            string strFilter = txtFilter.Text.ToLower();
            if (strFilter == "" && chVideo.IsChecked == false && chAudio.IsChecked == false && chImage.IsChecked == false)
            {
                OnShowDownloadPage();
                return;
            }
            List<string> strDownloadListTemp = new List<string>();
            strDownloadListTemp.Clear();
            DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
            string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
            string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + Constants.DownloadFileName;
            strDownloadListTemp = Md5Crypto.ReadCryptoFile(strPath);
            String[] spearator = { Constants.filePattern };
            int nID = 1;
            for (int i = strDownloadListTemp.Count - 1; i >= 0; i--)
            {
                string[] strArray = strDownloadListTemp[i].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                string strFileName = strArray[2].ToLower();
                if (chVideo.IsChecked == false && chAudio.IsChecked == false && chImage.IsChecked == false)
                {
                    if (strFilter != "" || strFilter != "filter by download file name.")
                    {
                        if (strFileName.Contains(strFilter))
                        {
                            DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                            nID++;
                            continue;
                        }
                    }
                }

                if (chVideo.IsChecked == true)
                {
                    bool bVideo = false;
                    for (int nVideo = 0; nVideo < Constants.videoFilterArray.Count(); nVideo++)
                    {
                        if (strFileName.Contains(Constants.videoFilterArray[nVideo]))
                        {
                            bVideo = true;
                            break;
                        }
                    }
                    if (bVideo == true)
                    {
                        if (strFilter == "" || strFilter == "filter by download file name.")
                        {
                            DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                            nID++;
                            continue;
                        }
                        else
                        {
                            if (strFileName.Contains(strFilter))
                            {
                                DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                                nID++;
                                continue;
                            }
                        }
                    }
                }
                if (chAudio.IsChecked == true)
                {
                    bool bAudio = false;
                    for (int nAudio = 0; nAudio < Constants.audioFilterArray.Count(); nAudio++)
                    {
                        if (strFileName.Contains(Constants.audioFilterArray[nAudio]))
                        {
                            bAudio = true;
                            break;
                        }
                    }
                    if (bAudio == true)
                    {
                        if (strFilter == "" || strFilter == "filter by download file name.")
                        {
                            DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                            nID++;
                            continue;
                        }
                        else
                        {
                            if (strFileName.Contains(strFilter))
                            {
                                DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                                nID++;
                                continue;
                            }
                        }
                    }
                }
                if (chImage.IsChecked == true)
                {
                    bool bImage = false;
                    for (int nImage = 0; nImage < Constants.imageFilterArray.Count(); nImage++)
                    {
                        if (strFileName.Contains(Constants.imageFilterArray[nImage]))
                        {
                            bImage = true;
                            break;
                        }
                    }
                    if (bImage == true)
                    {
                        if (strFilter == "" || strFilter == "filter by download file name.")
                        {
                            DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                            nID++;
                            continue;
                        }
                        else
                        {
                            if (strFileName.Contains(strFilter))
                            {
                                DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strFileName, Size = strArray[3] });
                                nID++;
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public void OnShowDownloadPage()
        {
            DownLoadPanel.Items.Clear();
            List<string> strDownloadListTemp = new List<string>();
            strDownloadListTemp.Clear();
            DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
            string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
            string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + Constants.DownloadFileName;
            strDownloadListTemp = Md5Crypto.ReadCryptoFile(strPath);
            String[] spearator = { Constants.filePattern };
            int nID = 1;
            for (int i = strDownloadListTemp.Count - 1; i >= 0; i--)
            {
                string[] strArray = strDownloadListTemp[i].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                DownLoadPanel.Items.Add(new Download { ID = nID, UserName = strArray[0], UserIP = strArray[1], From = strArray[4], To = strArray[5], FileName = "  " + strArray[2], Size = strArray[3] });
                nID++;
            }
            if (strDownloadListTemp.Count == 0)
            {
                DownLoadPanel.Items.Add(new Download { UserName = "", UserIP = "", From = "", To = "", FileName = "", Size = "" });
            }

        }

        public void OnShowUrlPage()
        {
            pnlURL_Client_List.Children.Clear();
            MemeberURL.Items.Clear();
            chURLFilter.IsChecked = false;
            urlFilter.Text = "Filter by url name.";
            if (date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                foreach (var client in Settings.Instance.ClientDic.ToList())
                {
                    pnlUrl_Client client_URL = new pnlUrl_Client();
                    if (client.Value.DangerURLCount == "0")
                    {
                        client_URL.message_body.Visibility = Visibility.Hidden;
                        client_URL.message_Dangercount.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        client_URL.message_body.Visibility = Visibility.Visible;
                        client_URL.message_Dangercount.Visibility = Visibility.Visible;
                        client_URL.message_Dangercount.Content = client.Value.DangerURLCount;
                    }

                    client_URL.lblClientName.Content = client.Value.UserName + " - " + client.Value.ClientIP;
                    client_URL.lblVisitURL.Content = "Visited URL : " + client.Value.URLList.Count;
                    client_URL.lblDownload.Content = "Download : " + client.Value.DownloadCount;
                    client_URL.lblDanger.Content = "Danger URL : " + client.Value.DangerURLCount;
                    client_URL.lblForbidden.Content = "Forbidden Process : " + client.Value.ForbiddenProcessCount;
                    //if (client.Value.NetworkState == false)
                    //{
                    //    client_URL.bConnect.Fill = new SolidColorBrush(Colors.LightGray);
                    //}
                    //else
                    //{
                    //    client_URL.bConnect.Fill = new SolidColorBrush(Colors.LightGreen);
                    //}

                    pnlURL_Client_List.Children.Add(client_URL);
                }
            }
            else
            {
                foreach (var client in Settings.Instance.ClientDic_Temp.ToList())
                {
                    pnlUrl_Client client_URL = new pnlUrl_Client();
                    if (client.Value.DangerURLCount == "0")
                    {
                        client_URL.message_body.Visibility = Visibility.Hidden;
                        client_URL.message_Dangercount.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        client_URL.message_body.Visibility = Visibility.Visible;
                        client_URL.message_Dangercount.Visibility = Visibility.Visible;
                        client_URL.message_Dangercount.Content = client.Value.DangerURLCount;
                    }

                    client_URL.lblClientName.Content = client.Value.UserName + " - " + client.Value.ClientIP;
                    client_URL.lblVisitURL.Content = "Visited URL : " + client.Value.URLList.Count;
                    client_URL.lblDownload.Content = "Download : " + client.Value.DownloadCount;
                    client_URL.lblDanger.Content = "Danger URL : " + client.Value.DangerURLCount;
                    client_URL.lblForbidden.Content = "Forbidden Process : " + client.Value.ForbiddenProcessCount;



                    //if (client.Value.NetworkState == false)
                    //{
                    //    client_URL.bConnect.Fill = new SolidColorBrush(Colors.LightGray);
                    //}
                    //else
                    //{
                    //    client_URL.bConnect.Fill = new SolidColorBrush(Colors.LightGreen);
                    //}

                    pnlURL_Client_List.Children.Add(client_URL);
                }
                // DateTime tempDate = (DateTime)date_Picker.SelectedDate;
                // string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
            }
        }


        public void ShowUserInfo(ClientInfo clientInfo)
        {
            pnlSample pnlMember = new pnlSample(clientInfo, this.Width - 189);


            pnlMember.btnShowState.Click += new RoutedEventHandler(btnShowState_Click);
            pnlPanels.Children.Add(pnlMember);
            m_UserInfoList.Add(pnlMember);
        }

        private void ShowSettingPage(object sender, MouseButtonEventArgs e)
        {
            OnShowSettingPage();
        }

        public void OnShowSettingPage()
        {
            pnlSetting_Client_List.Children.Clear();
            CaptureTime.Text = "";
            CaptureWidth.Text = "";
            CaptureHeight.Text = "";
            SlideWidth.Text = "";
            SlideHeight.Text = "";
            WorkingSession.Text = "";
            ActiveDuration.Text = "";
            lblUserIP.Content = "";
            lblUserName.Content = "";
            lblWorkTime.Content = "";
            lblVisitURL.Content = "";
            lblDownload.Content = "";
            lblDangerURL.Content = "";
            lblForbidden.Content = "";

            chScreenCapture.IsChecked = false;
            chAudioRecord.IsChecked = false;
            chDownloadFiles.IsChecked = false;
            chTaskList.IsChecked = false;
            chURL.IsChecked = false;
            chDownloadFiles.IsChecked = false;

            if (date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                foreach (var client in Settings.Instance.ClientDic.ToList())
                {
                    pnlSetting_Client client_Setting = new pnlSetting_Client();
                    client_Setting.lblClientName.Content = client.Value.UserName + " - " + client.Value.ClientIP;
                    client_Setting.lblName.Content = client.Value.PCName;
                    client_Setting.lblOS.Content = client.Value.OSDate;
                    client_Setting.lblWork.Content = client.Value.WorkTimeCount;
                    if (client.Value.NetworkState == false)
                    {
                        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGray);
                    }
                    else
                    {
                        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGreen);
                    }

                    pnlSetting_Client_List.Children.Add(client_Setting);
                }
            }
            else
            {
                foreach (var client in Settings.Instance.ClientDic_Temp.ToList())
                {
                    pnlSetting_Client client_Setting = new pnlSetting_Client();
                    client_Setting.lblClientName.Content = client.Value.UserName + " - " + client.Value.ClientIP;
                    client_Setting.lblName.Content = client.Value.PCName;
                    client_Setting.lblOS.Content = client.Value.OSDate;
                    client_Setting.lblWork.Content = client.Value.WorkTimeCount;
                    if (client.Value.NetworkState == false)
                    {
                        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGray);
                    }
                    else
                    {
                        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGreen);
                    }

                    pnlSetting_Client_List.Children.Add(client_Setting);
                }
                // DateTime tempDate = (DateTime)date_Picker.SelectedDate;
                // string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
            }
        }

        private void ShowForbiddenPage(object sender, MouseEventArgs e)
        {
            ForbiddenProc.LoadForbiddenProcessList();
            MemeberForbidder.Items.Clear();
            int nID = 1;
            bool bFlag = false;
            if (Windows.MainWindow.date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                foreach (var item in Settings.Instance.ClientDic.ToList())
                {
                    if (item.Value.ProcessList.Count == 0)
                    {
                        string strDbFilePath = Settings.Instance.Directories.TodayDirectory + "\\" + item.Value.ClientIP;
                        //SaveProcessInfo saveProcessInfo = new SaveProcessInfo();
                        saveProcessInfo.LoadProcessInfo(strDbFilePath, item.Value.ClientIP);
                        saveProcessInfo.LoadClientProcessInfo(strDbFilePath, item.Value.ClientIP);

                    }

                    foreach (var processInfo in item.Value.ProcessList.ToList())
                    {
                        foreach (var processName in Settings.Instance.Forbiddenprocess_list.ToList())
                        {
                            if (processInfo.ProcessName.Contains(processName.Key.Trim()))
                            {
                                bFlag = true;
                                MemeberForbidder.Items.Add(new MemeberForbidder { ID = nID, UserName = item.Value.UserName, UserIP = item.Value.ClientIP, From = processInfo.ProcessStartTime.ToString(), To = processInfo.ProcessEndTime.ToString(), ProgramName = "  " + processName.Value, ProcessName = "  " + processName.Key });
                                nID++;
                            }
                        }
                    }
                }
                if (bFlag == false)
                {
                    MemeberForbidder.Items.Add(new MemeberForbidder { UserName = "", UserIP = "", From = "", To = "", ProgramName = "", ProcessName = "" });
                    //MemeberForbidder.Items.Add(new MemeberForbidder {UserName = "", UserIP = "", From = "", To = "All members are didn't use the forbidden process.", ProgramName = "", ProcessName = "" });
                }
            }
            else
            {
                foreach (var item in Settings.Instance.ClientDic_Temp.ToList())
                {
                    if (item.Value.ProcessList.Count == 0)
                    {
                        DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
                        string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
                        string strDbFilePath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + item.Value.ClientIP;
                        //SaveProcessInfo saveProcessInfo = new SaveProcessInfo();
                        saveProcessInfo.LoadProcessInfo(strDbFilePath, item.Value.ClientIP);
                        saveProcessInfo.LoadClientProcessInfo(strDbFilePath, item.Value.ClientIP);

                    }

                    foreach (var processInfo in item.Value.ProcessList.ToList())
                    {
                        foreach (var processName in Settings.Instance.Forbiddenprocess_list.ToList())
                        {
                            if (processInfo.ProcessName.Contains(processName.Key.Trim()))
                            {
                                bFlag = true;
                                MemeberForbidder.Items.Add(new MemeberForbidder { ID = nID, UserName = item.Value.UserName, UserIP = item.Value.ClientIP, From = processInfo.ProcessStartTime.ToString(), To = processInfo.ProcessEndTime.ToString(), ProgramName = "  " + processName.Value, ProcessName = "  " + processName.Key });
                                nID++;
                            }
                        }
                    }
                }
                if (bFlag == false)
                {
                    MemeberForbidder.Items.Add(new MemeberForbidder { UserName = "", UserIP = "", From = "", To = "", ProgramName = "", ProcessName = "" });
                    //MemeberForbidder.Items.Add(new MemeberForbidder {UserName = "", UserIP = "", From = "", To = "All members are didn't use the forbidden process.", ProgramName = "", ProcessName = "" });
                }
            }


            //pnlForbidden_Client_List.Children.Clear();
            //foreach (var client in Settings.Instance.ClientDic)
            //{
            //    pnlSetting_Client client_Setting = new pnlSetting_Client();
            //    client_Setting.lblClientName.Content = client.Value.UserName + " - " + client.Value.ClientIP;
            //    client_Setting.lblName.Content = "Name : " + client.Value.UserName;
            //    client_Setting.lblCompany.Content = "Company : " + client.Value.Company;
            //    if (client.Value.NetworkState == false)
            //    {
            //        client_Setting.lblNetworkState.Content = "NetworkSate : DisConnected";
            //        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGray);
            //    }
            //    else
            //    {
            //        client_Setting.lblNetworkState.Content = "NetworkSate : Connected";
            //        client_Setting.bConnect.Fill = new SolidColorBrush(Colors.LightGreen);
            //    }
            //    client_Setting.mainWindow = this;
            //    pnlForbidden_Client_List.Children.Add(client_Setting);
            //}
        }

        private void WorkDirectory_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                WorkingFolder.Text = openFileDlg.SelectedPath;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (strClientIP == "")
            {
                return;
            }
            if (CaptureTime.Text == "")
            {
                return;
            }
            if (CaptureWidth.Text == "")
            {
                return;
            }
            if (CaptureHeight.Text == "")
            {
                return;
            }
            if (WorkingSession.Text == "")
            {
                return;
            }
            if (SlideWidth.Text == "")
            {
                return;
            }
            if (SlideHeight.Text == "")
            {
                return;
            }
            if (ActiveDuration.Text == "")
            {
                return;
            }
            Settings.Instance.ClientDic[strClientIP].CaptureTime = Int32.Parse(CaptureTime.Text);
            Settings.Instance.ClientDic[strClientIP].CaptureWidth = Int32.Parse(CaptureWidth.Text);
            Settings.Instance.ClientDic[strClientIP].CaptureHeight = Int32.Parse(CaptureHeight.Text);
            Settings.Instance.ClientDic[strClientIP].SessionTime = Int32.Parse(WorkingSession.Text);
            Settings.Instance.ClientDic[strClientIP].SlideWidth = Int32.Parse(SlideWidth.Text);
            Settings.Instance.ClientDic[strClientIP].SlideHeight = Int32.Parse(SlideHeight.Text);
            Settings.Instance.ClientDic[strClientIP].ActiveDuration = Int32.Parse(ActiveDuration.Text);

            ReplaceUserInfomation();

            string strMessage = WorkingSession.Text + ":" + SlideWidth.Text + ":" + SlideHeight.Text + ":" + CaptureTime.Text + ":" + CaptureWidth.Text + ":" + CaptureHeight.Text + ":" + ActiveDuration.Text;
            CommProc.Instance.SendaAnalysis(strClientIP, strMessage, Constants.Se_SetInfo);

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            string[] dirs = Directory.GetDirectories(Settings.Instance.RegValue.BaseDirectory);
            foreach (var dir in dirs)
            {
                string cDirectory = dir + "\\" + strClientIP;
                if (chScreenCapture.IsChecked.Value)
                {
                    try
                    {
                        if (Directory.Exists(cDirectory + "\\Capture"))
                        {
                            Directory.Delete(cDirectory + "\\Capture", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }

                    try
                    {
                        if (Directory.Exists(cDirectory + "\\Slide"))
                        {
                            Directory.Delete(cDirectory + "\\Slide", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }

                if (chAudioRecord.IsChecked.Value)
                {
                    try
                    {
                        if (Directory.Exists(cDirectory + "\\Audio"))
                        {
                            Directory.Delete(cDirectory + "\\Audio", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }

                    try
                    {
                        if (File.Exists(cDirectory + "\\" + Constants.AudioFileName))
                        {
                            File.Delete(cDirectory + "\\" + Constants.AudioFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }

                if (chTaskList.IsChecked.Value)
                {
                    try
                    {
                        if (File.Exists(cDirectory + "\\" + Constants.DbFileName))
                        {
                            File.Delete(cDirectory + "\\" + Constants.DbFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }

                if (chURL.IsChecked.Value)
                {
                    try
                    {
                        try
                        {
                            if (File.Exists(cDirectory + "\\" + Constants.urlFileName))
                            {
                                File.Delete(cDirectory + "\\" + Constants.DbFileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            CustomEx.DoExecption(Constants.exResume, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }

                if (chDownloadFiles.IsChecked.Value)
                {
                    try
                    {
                        if (File.Exists(cDirectory + "\\" + Constants.DownloadFileName))
                        {
                            File.Delete(cDirectory + "\\" + Constants.DownloadFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }
            }
        }

        private void btnServer_Set_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.RegValue.DelDataDay = Int32.Parse(DelDataDay.Text);
            Settings.Instance.RegValue.MessageListCount = Int32.Parse(MessageListCounts.Text);

            if (Settings.Instance.RegValue.BaseDirectory != WorkingFolder.Text)
            {
                string today = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                string todayDir = Path.Combine(Settings.Instance.Directories.WorkDirectory, today);

                //foreach (var item in Settings.Instance.SocketDic)
                //{
                //    try
                //    {
                //        item.Value.clientSocket.Dispose();
                //    }
                //    catch { }
                //}

                foreach (var item in Settings.Instance.ClientDic)
                {
                    try
                    {
                        FileHelper.SafeMoveFolder(todayDir + "\\" + item.Value.ClientIP, WorkingFolder.Text + "\\" + today + "\\" + item.Value.ClientIP, true);
                    }
                    catch { }
                }

                //FileHelper.SafeMoveFolder(todayDir, WorkingFolder.Text + "\\" + today, true);

                Settings.Instance.RegValue.BaseDirectory = WorkingFolder.Text;
            }

            Settings.Instance.RegValue.WriteValue();

            EnvironmentHelper.Restart();
        }

        private void btnPassword_Set_Click(object sender, RoutedEventArgs e)
        {
            ConditionMsg confirmMessage = new ConditionMsg("Are you sure to change the password?");
            if (ConditionMsgWindow.bDelete == false)
            {
                return;
            }
            if (Password.Password != ConfrimPassword.Password)
            {
                CustomMsg message = new CustomMsg("Please input the password correctly");
                return;
            }
            if (Password.Password == "")
            {
                CustomMsg message = new CustomMsg("Please input the password.");
                return;
            }

            Settings.Instance.RegValue.Password = Password.Password;
            //   CustomMsg success_message = new CustomMsg("Success.");

            Password.Password = "";
            ConfrimPassword.Password = "";
            Settings.Instance.RegValue.WriteValue();
        }

        private void btnShowState_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb;
            if (txtMessageListState.Text == "HIDE")
            {
                sb = Resources["SlideLeft"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "SHOW";
            }
            else
            {
                sb = Resources["SlideRight"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "HIDE";
            }
        }

        private void btnMessage_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb;
            if (txtMessageListState.Text == "HIDE")
            {
                sb = Resources["SlideLeft"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "SHOW";
            }
            else
            {
                sb = Resources["SlideRight"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "HIDE";
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb;
            if (txtMessageListState.Text == "HIDE")
            {
                sb = Resources["SlideLeft"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "SHOW";
            }
            else
            {
                sb = Resources["SlideRight"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "HIDE";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }
        public void SetTrayIcon()
        {
            Settings.Instance.ni.Icon = Properties.Resources.TaskControl;
            Settings.Instance.ni.Visible = true;
            Settings.Instance.ni.Text = "Task Control";
            Settings.Instance.ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

        }

        public void DeleteTryIcon()
        {
            Settings.Instance.ni.Dispose();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            this.Hide();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Disable close button
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = NativeImports.GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                NativeImports.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void PnlMessages_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadState();
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            bDatePicker = true;
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            bDatePicker = false;
        }

        private void OnSelectedDateChaged(object sender, SelectionChangedEventArgs e)
        {
            if (bDatePicker == true)
            {
                DateTime tempDate = (DateTime)date_Picker.SelectedDate;


                pnlPanels.Children.Clear();
                m_UserInfoList.Clear();
                if (tempDate.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    //m_UserInfoList.Clear();
                    foreach (var clientInfo in Settings.Instance.ClientDic)
                    {

                        ShowUserList(clientInfo.Value);
                    }
                }
                else
                {
                    //m_UserInfoList.Clear();
                    string strDate = string.Format("{0}-{1}-{2}", tempDate.Year, tempDate.Month, tempDate.Day);
                    OnShowData(strDate);
                }
                bDatePicker = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("LIST COUNT ========> {0}", m_UserInfoList.Count);
            foreach (var list in m_UserInfoList)
            {
                list.m_Width = this.Width - 126;
            }
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

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteTryIcon();
            EnvironmentHelper.Restart(true);
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Thread.Sleep(1000);
            Storyboard sb;
            if (txtMessageListState.Text == "HIDE")
            {
                sb = Resources["SlideLeft"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "SHOW";
            }
            else
            {
                sb = Resources["SlideRight"] as Storyboard;
                sb.Begin(grdMessageList);
                txtMessageListState.Text = "HIDE";
            }
        }

        private void CopyURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;

                //Get the ContextMenu to which the menuItem belongs
                var contextMenu = (ContextMenu)menuItem.Parent;

                //Find the placementTarget
                var item = (DataGrid)contextMenu.PlacementTarget;
                var urlItem = item.CurrentItem as pnlUrl_Client.MemeberURLList;

                if (urlItem == null) return;

                string strURL = urlItem.Url;

                Clipboard.SetText(strURL.Trim());
            }
            catch (Exception ex) { }

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

                var urlItem = item.CurrentItem as pnlUrl_Client.MemeberURLList;

                if (urlItem == null) return;

                string strURL = urlItem.Url;

                System.Diagnostics.Process.Start(strURL.Trim());
                  

                
            }
            catch(Exception ex) { }
        }

        private void MemeberURL_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (MemeberURL.Items.Count == 0)
                menu.Visibility = Visibility.Hidden;
            else
                menu.Visibility = Visibility.Visible;
        }


        public void ShowAlarm()
        {
            //BitmapImage logo = new BitmapImage();
            //logo.BeginInit();
            //logo.UriSource = new Uri("pack://application:,,,/Resource/alarm.png");
            //logo.EndInit();
            //fVersion = true;
            //alarm.Source = logo;
        }
        private void Alarm_Click(object sender, RoutedEventArgs e)
        {
            if (fVersion == false)
            {
                return;
            }
            else
            {
                patchProc.DownloadPatch();
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Audio audioInfo = ((Button)e.Source).DataContext as Audio;


            String IP = audioInfo.UserIP.ToString();

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                string strAudioFilePath = Settings.Instance.Directories.TodayDirectory + "\\" + audioInfo.UserIP.ToString() + "\\" + Constants.AudioFolder + "\\" + audioInfo.FileName;

                if (!File.Exists(strAudioFilePath)) return;

                Process.Start(strAudioFilePath);
            }
            else
            {
                DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
                string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
                string strAudioFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + strDate + "\\" + audioInfo.UserIP.ToString() + "\\" + Constants.AudioFolder + "\\" + audioInfo.FileName;

                if (!File.Exists(strAudioFilePath)) return;


                Process.Start(strAudioFilePath);
            }


        }
        private void ShowAudioPage(object sender, MouseButtonEventArgs e)
        {
            OnShowAudio();
        }

        private void OnShowAudio()
        {
            AudioInfo.Items.Clear();
            DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
            List<Audio> audioList = new List<Audio>();
            String[] spearator = { Constants.filePattern };

            int nID = 1;
            if (date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                string strDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                foreach (var item in Settings.Instance.ClientDic.ToList())
                {
                    //string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + item.Value.ClientIP + "\\" + Constants.AudioFileName;
                    //var strAudioListTemp = Md5Crypto.ReadCryptoFile(strPath);
                    //foreach (var strListTemp in strAudioListTemp)
                    //{
                    //    try
                    //    {
                    //        string[] strArray = strListTemp.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    //        DateTime date1 = DateTime.Parse(strArray[3]);
                    //        DateTime date2 = DateTime.Parse(strArray[4]);
                    //        System.TimeSpan diff = date2.Subtract(date1);
                    //        string duration = String.Format("{0}:{1}:{2}", diff.Hours.ToString("00"), diff.Minutes.ToString("00"), diff.Seconds.ToString("00"));
                    //        audioList.Add(new Audio { ID = nID, UserName = item.Value.UserName, UserIP = item.Value.ClientIP, ProcessWindow = strArray[1], From = DateTime.Parse(strArray[3]).ToString("HH:mm:ss"), Duration = duration, FileName = strArray[5], Size = strArray[6] });

                    //    }
                    //    catch
                    //    {

                    //    }
                    //}
                    foreach (var strArray in item.Value.AudioList.ToList())
                    {
                        try
                        {
                            DateTime date1 = strArray.ProcessStartTime;
                            DateTime date2 = strArray.ProcessEndTime;
                            System.TimeSpan diff = date2.Subtract(date1);
                            string duration = String.Format("{0}:{1}:{2}", diff.Hours.ToString("00"), diff.Minutes.ToString("00"), diff.Seconds.ToString("00"));

                            audioList.Add(new Audio { ID = nID, UserName = item.Value.UserName, UserIP = item.Value.ClientIP, ProcessWindow = strArray.ProcessWindow, From = strArray.ProcessStartTime.ToString("HH:mm:ss"), Duration = duration, FileName = strArray.FileName, Size = strArray.FileSize });
                            
                        }
                        catch
                        {

                        }
                    }
                }
                foreach (var item in audioList.OrderByDescending(x => x.From))
                {
                    item.ID = nID;
                    AudioInfo.Items.Add(item);
                    nID++;
                }

            }
            else
            {
                string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
                foreach (var item in Settings.Instance.ClientDic_Temp.ToList())
                {
                    string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + item.Value.ClientIP + "\\" + Constants.AudioFileName;
                    var strAudioListTemp = Md5Crypto.ReadCryptoFile(strPath);
                    foreach (var strListTemp in strAudioListTemp)
                    {
                        try
                        {
                            string[] strArray = strListTemp.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                            DateTime date1 = DateTime.Parse(strArray[3]);
                            DateTime date2 = DateTime.Parse(strArray[4]);
                            System.TimeSpan diff = date2.Subtract(date1);
                            string duration = String.Format("{0}:{1}:{2}", diff.Hours.ToString("00"), diff.Minutes.ToString("00"), diff.Seconds.ToString("00"));
                            audioList.Add(new Audio { ID = nID, UserName = item.Value.UserName, UserIP = item.Value.ClientIP, ProcessWindow = strArray[1], From = DateTime.Parse(strArray[3]).ToString("HH:mm:ss"), Duration = duration, FileName = strArray[5], Size = strArray[6] });

                        }
                        catch
                        {

                        }
                    }
                }
                foreach (var item in audioList.OrderByDescending(x => x.From))
                {
                    item.ID = nID;
                    AudioInfo.Items.Add(item);
                    nID++;
                }
            }
        }
    }


    public class OsHistory
    {
        public string ID { get; set; }
        public string User { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public string OsDate { get; set; }
    }

    public class ForbiddenProcess
    {
        public int ID { get; set; }
        public string PName { get; set; }
        public string GName { get; set; }

        public string ColorSet { get; set; }

        public ForbiddenProcess() { }

        public ForbiddenProcess(int id, string pName, string gName)
        {
            ID = id;
            PName = pName;
            GName = gName;

            if (ID % 2 == 0)
                ColorSet = "Red";
            else
                ColorSet = "Green";
        }
    }

    public class AllowURL
    {
        public int ID { get; set; }
        public string allowURL { get; set; }
    }

    public class ForbiddenURL
    {
        public int ID { get; set; }
        public string forbiddenURL { get; set; }
    }


    public class MemeberForbidder
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserIP { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ProcessName { get; set; }
        public string ProgramName { get; set; }
    }

    public class Download
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserIP { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FileName { get; set; }
        public string Size { get; set; }
    }

    public class Audio
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserIP { get; set; }
        public string From { get; set; }
        public string Duration { get; set; }
        public string ProcessWindow { get; set; }
        public string Size { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
    }
}
