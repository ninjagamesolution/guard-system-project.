using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for pnlUrl_Client.xaml
    /// </summary>
    public partial class pnlUrl_Client
    {
        public pnlUrl_Client()
        {
            InitializeComponent();
            this.Height = 35;
        }

        string strClientIP = "";

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

        private void lblClientName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (object child in Windows.MainWindow.pnlURL_Client_List.Children)
            {
                pnlUrl_Client m_pstackpanel = (pnlUrl_Client)child;
                if (m_pstackpanel != this)
                {
                    m_pstackpanel.Height = 35;
                }

            }
            Windows.MainWindow.chURLFilter.IsChecked = false;
            Windows.MainWindow.urlFilter.Text = "Filter by url name.";
            Windows.MainWindow.MemeberURL.Items.Clear();
            if (this.Height == 35)
            {
                
                this.Height = 150;
            }
            else
            {
                this.Height = 35;
                return;
            }

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
            

            Windows.MainWindow.strClientIP = strClientIP;
            Windows.MainWindow.MemeberURL.Items.Clear();
            if (item.URLList.Count == 0)
            {
                URLProc _urlProc = new URLProc();

                _urlProc.LoadURLInfo(strDbFilePath, strClientIP);
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
                    Windows.MainWindow.MemeberURL.Items.Add(new MemeberURLList { ID = nID, Type = bitmapSource, Window = "  " + item.URLList.ToList()[i].strWindow, Url = "  " + item.URLList.ToList()[i].strURL, StartTime = item.URLList.ToList()[i].URLStartTime.ToString("HH-mm-ss"), EndTime = item.URLList.ToList()[i].URLEndTime.ToString("HH-mm-ss"), State = strState });

                    nID++;
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }
            
        }
        
        public class MemeberURLList
        {
            public int ID { get; set; }
            public BitmapSource Type { get; set; }
            public string Window { get; set; }
            public string Url { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string State { get; set; }
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
