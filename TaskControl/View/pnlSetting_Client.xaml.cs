using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.Utils;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for pnlSetting_Client.xaml
    /// </summary>
    public partial class pnlSetting_Client
    {
        public pnlSetting_Client()
        {
            InitializeComponent();
            this.Height = 35;
        }
        
        string strClientIP = "";
        private void lblClientName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //StackPanel m_pstackpanel = (StackPanel)this.Parent;
            foreach (object child in Windows.MainWindow.pnlSetting_Client_List.Children)
            {
                pnlSetting_Client m_pstackpanel = (pnlSetting_Client)child;
                if (m_pstackpanel != this)
                {
                    m_pstackpanel.Height = 35;
                }
                
            }
            if (this.Height == 35)
            {
                this.Height = 130;
            }
            else
            {
                this.Height = 35;

                Windows.MainWindow.CaptureTime.Text = "";
                Windows.MainWindow.CaptureWidth.Text = "";
                Windows.MainWindow.CaptureHeight.Text = "";
                Windows.MainWindow.SlideWidth.Text = "";
                Windows.MainWindow.SlideHeight.Text = "";
                Windows.MainWindow.WorkingSession.Text = "";
                Windows.MainWindow.ActiveDuration.Text = "";

                Windows.MainWindow.lblUserIP.Content = "";
                Windows.MainWindow.lblUserName.Content = "";

                Windows.MainWindow.lblWorkTime.Content = "";
                Windows.MainWindow.lblVisitURL.Content = "";
                Windows.MainWindow.lblDownload.Content = "";
                Windows.MainWindow.lblDangerURL.Content = "";
                Windows.MainWindow.lblForbidden.Content = "";

                return;
            }

            Windows.MainWindow.chScreenCapture.IsChecked = false;
            Windows.MainWindow.chAudioRecord.IsChecked = false;
            Windows.MainWindow.chDownloadFiles.IsChecked = false;
            Windows.MainWindow.chTaskList.IsChecked = false;
            Windows.MainWindow.chURL.IsChecked = false;
            Windows.MainWindow.chDownloadFiles.IsChecked = false;

            strClientIP = lblClientName.Content.ToString().Split(' ')[lblClientName.Content.ToString().Split(' ').Length - 1];
            ClientInfo item = new ClientInfo();
            string strDbFilePath = "";

            if (Windows.MainWindow.date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
            {
                item = Settings.Instance.ClientDic[strClientIP];
                strDbFilePath = Settings.Instance.Directories.TodayDirectory + "\\" + strClientIP;
                
            }
            else
            {
                DateTime tempDate = (DateTime)Windows.MainWindow.date_Picker.SelectedDate;
                string strDate = tempDate.Year.ToString() + "-" + tempDate.Month.ToString() + "-" + tempDate.Day.ToString();
                strDbFilePath = Settings.Instance.Directories.WorkDirectory + "\\" + strDate + "\\" + strClientIP;
                item = Settings.Instance.ClientDic_Temp[strClientIP];
            }

            Windows.MainWindow.CaptureTime.Text = item.CaptureTime.ToString();
            Windows.MainWindow.CaptureWidth.Text = item.CaptureWidth.ToString();
            Windows.MainWindow.CaptureHeight.Text = item.CaptureHeight.ToString();
            Windows.MainWindow.SlideWidth.Text = item.SlideWidth.ToString();
            Windows.MainWindow.SlideHeight.Text = item.SlideHeight.ToString();
            Windows.MainWindow.WorkingSession.Text = item.SessionTime.ToString();
            Windows.MainWindow.ActiveDuration.Text = item.ActiveDuration.ToString();
            
            Windows.MainWindow.strClientIP = strClientIP;

            Windows.MainWindow.lblUserIP.Content = item.ClientIP;
            Windows.MainWindow.lblUserName.Content = item.UserName;

            Windows.MainWindow.lblWorkTime.Content = item.WorkTimeCount;
            Windows.MainWindow.lblVisitURL.Content = item.URLList.Count;
            Windows.MainWindow.lblDownload.Content = item.DownloadCount;
            Windows.MainWindow.lblDangerURL.Content = item.DangerURLCount;
            Windows.MainWindow.lblForbidden.Content = item.ForbiddenProcessCount;
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ConditionMsg message = new ConditionMsg("Are you sure to delete this member?");
            if (ConditionMsgWindow.bDelete == false)
            {
                return;
            }
            Settings.Instance.ClientDic.Remove(strClientIP);
            Settings.Instance.ClientDic_Temp.Remove(strClientIP);
            Windows.MainWindow.CaptureTime.Text = "";
            Windows.MainWindow.CaptureWidth.Text = "";
            Windows.MainWindow.CaptureHeight.Text = "";
            Windows.MainWindow.SlideWidth.Text = "";
            Windows.MainWindow.SlideHeight.Text = "";
            Windows.MainWindow.WorkingSession.Text = "";
            Windows.MainWindow.ActiveDuration.Text = "";
            Windows.MainWindow.pnlSetting_Client_List.Children.Clear();
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

                Windows.MainWindow.pnlSetting_Client_List.Children.Add(client_Setting);
            }

            foreach (var obj in Windows.MainWindow.m_UserInfoList)
            {
                if (obj.fldIPAddr.Content.ToString() == strClientIP)
                {
                    Windows.MainWindow.pnlPanels.Children.Remove(obj);
                    Windows.MainWindow.m_UserInfoList.Remove(obj);
                    
                    break;
                }
            }

            //Windows.MainWindow.ShowOsHistory();

            Windows.MainWindow.ReplaceUserInfomation();
            
        }

        void stackpanel_MouseLeave(object sender, MouseEventArgs e)
        {
            setting_client.Background.Opacity = 1;
        }

        void stackpanel_MouseMove(object sender, MouseEventArgs e)
        {
            setting_client.Background.Opacity = 0.7;
        }

    }
}
