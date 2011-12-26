using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using System.Collections;
using System.Windows.Media.Animation;
using Monitor.TaskControl.Utils;
using Monitor.TaskControl.Globals;
using System.Threading;
using Monitor.TaskControl.Models;

namespace Monitor.TaskControl.View
{
    // <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SlideWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        int ctr = -1;
        string directoryPath = "D:\\Work\\2020-12-23\\Capture";
        //string directoryPath = "C:\\Users\\1\\Downloads\\WPFSlideShow\\Images";
        string[] fileEntries;
        public string IP { get; set; }
        public string user { get; set; }
        public string selDate { get; set; }
        public bool bFlag = false;
        public int step = 1;
        List<string> strHumanImages = new List<string>();
        bool isToday;
        
        public SlideWindow(string path, string strIP, string strUser, string strDate)
        {

            InitializeComponent();
            IP = strIP;
            user = strUser;
            selDate = strDate;
            Panel.SetZIndex(play, 4);
            Panel.SetZIndex(stop, 3);

            DateTime curDate = DateTime.Now;
            string strCurDate = string.Format("{0}-{1}-{2}", curDate.Year, curDate.Month, curDate.Day);

            if (selDate == strCurDate)
            {
                isToday = true;
            } else
            {
                isToday = false;
            }


            this.Title = strUser + " : " + strIP + " : " + strDate;
            directoryPath = path;
            if (Directory.Exists(directoryPath))
            {
                // This path is a directory
                fileEntries = Directory.GetFiles(directoryPath);
            }

            if (fileEntries.Length == 0)
            {

                return;
            }

            
            scrollBar.Maximum = fileEntries.Length  ;
            scrollBarSpeed.Value = 5;
            PlaySlideShow(0);


            if (timer == null) return;
            step = (int)scrollBarStep.Value;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Tick += new EventHandler(timer_Tick);

        }

        void start()
        {

        }
        void timer_Tick(object sender, EventArgs e)
        {
            ctr = ctr + step;
            if (ctr >= fileEntries.Length)
            {
                ctr = 0;
            }
            PlaySlideShow(ctr);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctr = 0;
            PlaySlideShow(ctr);
        }
        private void PlaySlideShow(int ctr)
        {
            try
            {
                if (ctr < 0 || ctr >= fileEntries.Length) return;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                //string filename = ((ctr < 10) ? "images/Plane0" + ctr + ".jpeg" : "images/Plane" + ctr + ".jpeg");
                string filename = fileEntries[ctr];

                byte[] temp = Md5Crypto.OnReadImgFile(filename);
                MemoryStream ms = new MemoryStream(temp);
                image.StreamSource = ms;
                string showName = filename.Remove(0, directoryPath.Count() + 1);
                string strFileName = Md5Crypto.OnGetRealName(showName);
                this.Title = user + " : " + IP + " : " + selDate + ":" + strFileName;
                image.EndInit();

                myImage.Source = image;
                myImage.Stretch = Stretch.Uniform;
                scrollBar.Value = ctr;

                string key = System.IO.Path.GetFileName(filename);
                if (isToday)
                {
                    if (!Settings.Instance.ClientDic[IP].checkedMap.ContainsKey(key))
                    {
                        Settings.Instance.ClientDic[IP].checkedMap.Add(key, true);
                        Settings.Instance.ClientDic[IP].ReadCaptureCount++;

                        String suffix = "\\Capture";

                        Md5Crypto.WriteCryptoFileAppend(directoryPath.Substring(0, directoryPath.Length - suffix.Length), Constants.InspectFileName, key);
                    }
                } else
                {
                    if (!Settings.Instance.ClientDic_Temp[IP].checkedMap.ContainsKey(key))
                    {
                        Settings.Instance.ClientDic_Temp[IP].checkedMap.Add(key, true);
                        Settings.Instance.ClientDic_Temp[IP].ReadCaptureCount++;

                        String suffix = "\\Capture";

                        Md5Crypto.WriteCryptoFileAppend(directoryPath.Substring(0, directoryPath.Length - suffix.Length), Constants.InspectFileName, key);
                    }
                }

                if (!Settings.Instance.CheckedDates.Contains(selDate))
                {
                    Settings.Instance.CheckedDates.Add(selDate);
                    while (true)
                    {
                        try
                        {
                            using (TextWriter tw = new StreamWriter(Globals.Constants.BaseDirectory + "\\" + Globals.Constants.RestInspectFileName))
                            {
                                foreach (string element in Settings.Instance.CheckedDates)
                                {
                                    tw.WriteLine(element);
                                }
                            }
                            break;
                        }
                        catch
                        {

                        }
                    }
                }
                

                int percent = 0, totCnt = 0, checkedCnt = 0;

                Dictionary<string, ClientInfo> clientTemp = new Dictionary<string, ClientInfo>();

                if (isToday)
                {
                    clientTemp = Settings.Instance.ClientDic;
                }
                else
                {
                    clientTemp = Settings.Instance.ClientDic_Temp;
                }

                foreach (KeyValuePair<string, ClientInfo> entry in clientTemp)
                {
                    totCnt++;

                    if (isToday)
                    {
                        string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + selDate + "\\" + entry.Key + "\\Capture";

                        if (Directory.Exists(strPath))
                        {
                            entry.Value.TotalCaptureCount = Directory.GetFiles(strPath, "*", SearchOption.TopDirectoryOnly).Length;
                        }
                        else
                        {
                            entry.Value.TotalCaptureCount = 0;
                        }
                    }

                    if (entry.Value.TotalCaptureCount == 0 || entry.Value.ReadCaptureCount == 0)
                    {
                        continue;
                    }

                    if (entry.Value.TotalCaptureCount == 0 || entry.Value.ReadCaptureCount == 0)
                    {
                        continue;
                    }

                    //percent += entry.Value.ReadCaptureCount * 100 / entry.Value.TotalCaptureCount;
                    //checkedCnt++;
                }

                // percent /= totCnt;

                //while (true)
                //{
                //    if (Settings.Instance.IsSending) continue;

                //    Settings.Instance.IsSending = true;

                //    try
                //    {
                //        string strSend = selDate + "*" + percent + "%" + "*" + checkedCnt + "/" + totCnt;
                //        byte[] sendData = Encoding.UTF8.GetBytes(strSend);
                //        Settings.Instance.InspectSocket.Send(sendData, sendData.Length, System.Net.Sockets.SocketFlags.None);
                //    }
                //    catch
                //    {
                //        Settings.Instance.InspectSocket.Dispose();
                //        Thread InspectThread = new Thread(new ThreadStart(Inspect));
                //        InspectThread.Start();
                //    }
                //    Settings.Instance.IsSending = false;
                //    break;
                //}
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }

        }

        private void Inspect()
        {
            //MainProc.Instance.Inspect();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer == null) return;
            if (fileEntries == null || fileEntries.Length <= 0) return;
            ctr = (int)scrollBar.Value;
            PlaySlideShow(ctr);
        }

        private void ScrollBarSpeed_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer == null) return;

            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1 / scrollBarSpeed.Value * 2000));
        }


        private void Last_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ctr = fileEntries.Length - 1;
            PlaySlideShow(ctr);
        }

        private void Next_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ctr = ctr + step;
            if (ctr >= fileEntries.Length)
            {
                ctr = 0;
            }
            PlaySlideShow(ctr);
        }

        private void Play_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //humanCheck.IsEnabled = false;
            next.IsEnabled = false;
            first.IsEnabled = false;
            last.IsEnabled = false;
            previous.IsEnabled = false;
            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1 / scrollBarSpeed.Value * 2000));
            timer.IsEnabled = true;
            Panel.SetZIndex(play, 3);
            Panel.SetZIndex(stop, 4);
        }

        private void Stop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //humanCheck.IsEnabled = true;
            next.IsEnabled = true;
            first.IsEnabled = true;
            last.IsEnabled = true;
            previous.IsEnabled = true;
            timer.IsEnabled = false;
            Panel.SetZIndex(play, 4);
            Panel.SetZIndex(stop, 3);
        }

        private void Previous_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ctr = ctr - step;
            if (ctr < 0)
            {
                ctr = fileEntries.Length - 1;
            }
            PlaySlideShow(ctr);
        }

        private void First_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ctr = 0;
            PlaySlideShow(ctr);
        }

        private void MyImage_MouseEnter(object sender, MouseEventArgs e)
        {
            var point = Mouse.GetPosition(Application.Current.MainWindow);
            Storyboard sb;
            if (point.Y > 500)
            {
                sb = Resources["SlideLeft"] as Storyboard;
                sb.Begin(SlideControl);

            }
            else
            {
                sb = Resources["SlideRight"] as Storyboard;
                sb.Begin(SlideControl);

            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var point = Mouse.GetPosition(Application.Current.MainWindow);
            Storyboard sb;
            if (point.Y > 400)
            {

                if (bFlag == true)
                {

                    sb = Resources["SlideTop"] as Storyboard;
                    sb.Begin(SlideControl);
                }
                bFlag = false;

            }
            else
            {
                if (bFlag == false)
                {
                    sb = Resources["SlideBottom"] as Storyboard;
                    sb.Begin(SlideControl);
                }
                bFlag = true;

            }
        }

        private void ScrollBarStep_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer == null) return;
            if (fileEntries == null || fileEntries.Length <= 0) return;
            step = (int)scrollBarStep.Value;

        }

        private void GetHumanDetectionImage()
        {
            try
            {
                string strPath = directoryPath.Replace(@"\Capture", "");

                string strFilePath = System.IO.Path.Combine(strPath, "human.lib");
                if (System.IO.File.Exists(strFilePath))
                {
                    System.IO.File.Create(strFilePath);
                }

                fileEntries = System.IO.File.ReadAllLines(strFilePath);
            }
            catch
            {
                CustomMsg cw = new CustomMsg("There is no image including human.");
                //humanCheck.IsChecked = false;
                fileEntries = Directory.GetFiles(directoryPath);
                scrollBar.Maximum = fileEntries.Length - 1;
                scrollBar.Value = 0;
                return;
            }
           
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (humanCheck.IsChecked == true)
            //    {
            //        GetHumanDetectionImage();

            //        if (fileEntries.Length == 0)
            //        {
            //            CustomMsg cw = new CustomMsg("There is no image including human.");
            //            humanCheck.IsChecked = false;
            //            fileEntries = Directory.GetFiles(directoryPath);
            //            scrollBar.Maximum = fileEntries.Length - 1;
            //            scrollBar.Value = 0;
            //            return;
            //        }
            //        else
            //        {
            //            scrollBar.Maximum = fileEntries.Length - 1;
            //            scrollBar.Value = 0;
            //        }
            //        //fileEntries = strHumanImages.ToArray();
            //    }
            //    else
            //    {
            //        if (Directory.Exists(directoryPath))
            //        {
            //            // This path is a directory
            //            fileEntries = Directory.GetFiles(directoryPath);
            //            scrollBar.Maximum = fileEntries.Length - 1;
            //            scrollBar.Value = 0;
            //        }
            //    }
            //}
            //catch
            //{

            //}
            

        }
    }
}
