using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using Monitor.TaskControl.Models;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    public partial class ExplorerWindow : Window
    {
        public ExplorerWindow()
        {
            InitializeComponent();
        }

        List<string> fileList = new List<string>();
        int nWidth, nHeight;
        int nStart, nEnd, nCount, nIndex, nPrevStart, nPrevEnd, nOnceCount;
        public int nColumn = 0;
        public int nRow = 0;
        public int bCheck = 0;
        public int totalCount = 0;
        public bool bFlag = false;
        System.Timers.Timer timer;
        Stopwatch stopwatch = new Stopwatch();
        double scrollVerticalChange, currentHeight, scrollHeight;
        int deltaMicroSecond;
        double rateImage = 1;
        string strSelIP, strSelPath, strSelDate;
        bool isToday;
        
        public ExplorerWindow(string strPath, string strIP, string strUser, string strDate)
        {
            InitializeComponent();

            strSelIP = strIP;
            strSelPath = strPath;
            strSelDate = strDate;


            DateTime curDate = DateTime.Now;
            string strCurDate = curDate.Year.ToString() + "-" + curDate.Month.ToString() + "-" + curDate.Day.ToString();

            if (strCurDate == strDate)
            {
                isToday = true;
            } else
            {
                isToday = false;
            }

            nWidth = Constants.smallWidth;
            nHeight = Constants.smallHeight;
            this.Title = strUser + " : " + strIP + "        " + strDate;
            string[] supportedExtensions = new[] { ".psl" };
            var files = Directory.GetFiles(strPath, "*.*").Where(s => supportedExtensions.Contains(System.IO.Path.GetExtension(s).ToLower()));
            fileList.Clear();
            fileList = files.ToList();
            totalCount = fileList.Count;
            ImageList.Items.Clear();
            chSmall.IsChecked = true;

            nIndex = 0;
            deltaMicroSecond = 50;
            nPrevStart = nPrevEnd = -1;
            nOnceCount = 100;

            stopwatch.Start();

            timer = new System.Timers.Timer(deltaMicroSecond);
            timer.Elapsed += SetRangeTimerCallback;
            timer.Stop();

            foreach (var item in fileList)
            {
                try
                {
                    string strFileName = System.IO.Path.GetFileName(item);
                    string strRealFileName = Md5Crypto.OnGetRealName(strFileName).Replace("-", ":");
                    
                    StackPanel ImgStackPanel = new StackPanel();
                    ImgStackPanel.Orientation = Orientation.Vertical;
                    Image imgTemp = new Image();
                    Label lblFileName = new Label();
                    lblFileName.Content = strRealFileName;

                    lblFileName.HorizontalAlignment = HorizontalAlignment.Center;
                    lblFileName.VerticalAlignment = VerticalAlignment.Bottom;
                    lblFileName.Margin = new Thickness(0, 0, 0, 0);

                    imgTemp.Width = nWidth;
                    imgTemp.Height = nHeight - 20;
                    
                    imgTemp.Margin = new Thickness(0, 10, 0, 0);
                    
                    Border borderImg = SetBorderEffect();

                    borderImg.Child = imgTemp;
                    borderImg.Margin = new Thickness(0, nHeight - 10 - ((nHeight - 20) / rateImage), 0, 0);

                    //ImgStackPanel.Children.Add(imgTemp);
                    ImgStackPanel.Children.Add(borderImg);
                    ImgStackPanel.Children.Add(lblFileName);
                    ImgStackPanel.Width = nWidth;
                    ImgStackPanel.Height = nHeight + 10;

                    ImageList.Items.Add(ImgStackPanel);
                    //ImageList.Items.Add(_grid);
                }
                catch (Exception ex)
                {
                }
            }
            nCount = fileList.Count;
            nStart = 0;
            if (nCount > nOnceCount)
            {
                nEnd = nOnceCount;
            }
            else
            {
                nEnd = nCount;
            }

            Display(false);

            PreviewImageDisplay(nIndex);
        }
        
        private void EmptyDisplay()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = nPrevStart; i < nPrevEnd; i ++)
                {
                    if (nStart <= i && i < nEnd) continue;
                    try
                    {
                        //ImageList.Items.RemoveAt(i);
                        string strFileName = System.IO.Path.GetFileName(fileList[i]);
                        string strRealFileName = Md5Crypto.OnGetRealName(strFileName).Replace("-", ":");

                        StackPanel ImgStackPanel = new StackPanel();
                        ImgStackPanel.Orientation = Orientation.Vertical;
                        Image imgTemp = new Image();
                        Label lblFileName = new Label();
                        lblFileName.Content = strRealFileName;

                        lblFileName.HorizontalAlignment = HorizontalAlignment.Center;
                        lblFileName.VerticalAlignment = VerticalAlignment.Bottom;
                        lblFileName.Margin = new Thickness(0, 0, 0, 0);


                        imgTemp.Width = nWidth - 10;
                        imgTemp.Height = nHeight - 20;
                        //imgTemp.Margin = new Thickness(0, 10, 0, 0);
                
                        Border borderImg = SetBorderEffect();

                        borderImg.Child = imgTemp;
                        borderImg.Margin = new Thickness(0, nHeight - 10 - ((nHeight - 20) / rateImage), 0, 0);


                        //ImgStackPanel.Children.Add(imgTemp);
                        ImgStackPanel.Children.Add(borderImg);
                        ImgStackPanel.Children.Add(lblFileName);
                        ImgStackPanel.Width = nWidth;
                        ImgStackPanel.Height = nHeight + 10;

                        //ImageList.Items.Insert(i, ImgStackPanel);
                        ImageList.Items[i] = ImgStackPanel;
                        //ImageList.Items.Add(_grid);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("********ERROR : " + i.ToString() + " Line : " + ex.ToString());
                    }
                }

            }, System.Windows.Threading.DispatcherPriority.Normal);

        }

        Border SetBorderEffect()
        {
            DropShadowBitmapEffect myDropShadowEffect = new DropShadowBitmapEffect();
            Color myShadowColor = Colors.DarkGray;

            Border borderImg = new Border();
            borderImg.Width = nWidth - 10;
            borderImg.Height = (nWidth - 10) * (67.5 / rateImage / 120.0);//(nHeight - 20) / rateImage;
            borderImg.BorderThickness = new Thickness(0.5, 0.5, 2, 2);
            
            borderImg.BorderBrush = Brushes.DarkGray;

            return borderImg;
        }
        private void Display(bool updateGrid)
        {
            timer.Stop();

            if (updateGrid)
            {
                ListViewItem item = ImageList.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;

                double totalHeight = (item.ActualHeight + 20) * nRow - ImageList.ActualHeight;
                double pieceHeight = 0.0;

                if (totalHeight < 0) totalHeight = 0;

                int visibleStartRow = 0;
                int visibleEndRow = 100; // set the max value

                if (scrollHeight > 1e-6)
                {
                    visibleStartRow = (int)(1 + totalHeight * (currentHeight / scrollHeight) / (item.ActualHeight + 20) - 1e-6);
                    visibleEndRow = (int)((totalHeight * (currentHeight / scrollHeight) + ImageList.ActualHeight) / (item.ActualHeight + 20) + 1e-6 - 1);
                }
                //int currentPos = (int)currentHeight;
                //double percent = currentHeight / scrollHeight;

                if (visibleStartRow > 0)
                {
                    pieceHeight = ((item.ActualHeight + 20) * visibleStartRow) - (totalHeight * (currentHeight / scrollHeight));
                    if (pieceHeight  > (double)(item.ActualHeight + 20) / 3.0)
                    {
                        visibleStartRow--;
                    }
                }

                pieceHeight = (totalHeight * (currentHeight / scrollHeight) + ImageList.ActualHeight) - ((item.ActualHeight + 20) * (visibleEndRow + 1));
                if (pieceHeight > (double)(item.ActualHeight + 20) / 3.0)
                {
                    visibleEndRow++;
                }

                //nStart = (int)(percent * nCount) - 50;
                int nDelta = nColumn * 2;

                nStart = visibleStartRow * nColumn - nDelta;

                if (nStart < 0)
                {
                    nStart = 0;
                }

                //nEnd = nStart + nOnceCount;
                nEnd = visibleEndRow * nColumn + nDelta;

                if (nEnd > nCount)
                {
                    nEnd = nCount;
                }

                int nLast = (visibleEndRow + 1) * nColumn;

                if (nLast > nCount)
                {
                    nLast = nCount;
                }

                List<string> listFile = new List<string>();
                for (int i = visibleStartRow * nColumn; i < nLast; i++)
                {
                    string strFileName = System.IO.Path.GetFileName(fileList[i]);

                    if (isToday)
                    {
                        if (Settings.Instance.ClientDic[strSelIP].checkedMap.ContainsKey(strFileName)) continue;

                        Settings.Instance.ClientDic[strSelIP].checkedMap.Add(strFileName, true);
                        Settings.Instance.ClientDic[strSelIP].ReadCaptureCount++;

                    }
                    else
                    {
                        if (Settings.Instance.ClientDic_Temp[strSelIP].checkedMap.ContainsKey(strFileName)) continue;

                        Settings.Instance.ClientDic_Temp[strSelIP].checkedMap.Add(strFileName, true);
                        Settings.Instance.ClientDic_Temp[strSelIP].ReadCaptureCount++;

                    }

                    listFile.Add(strFileName);

                }


                string path = Settings.Instance.Directories.WorkDirectory + "\\" + strSelDate + "\\" + strSelIP + "\\Capture";

                if (Directory.Exists(path))
                {
                    if (isToday)
                    {
                        Settings.Instance.ClientDic[strSelIP].TotalCaptureCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
                    }
                    else
                    {
                        Settings.Instance.ClientDic_Temp[strSelIP].TotalCaptureCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
                    }

                }
                else
                {
                    if (isToday)
                    {
                        Settings.Instance.ClientDic[strSelIP].TotalCaptureCount = 0;
                    }
                    else
                    {
                        Settings.Instance.ClientDic_Temp[strSelIP].TotalCaptureCount = 0;
                    }
                }

                String suffix = "\\Capture";
                Md5Crypto.WriteCryptoFileAppendList(strSelPath.Substring(0, strSelPath.Length - suffix.Length), Constants.InspectFileName, listFile);

                if (!Settings.Instance.CheckedDates.Contains(strSelDate))
                {
                    Settings.Instance.CheckedDates.Add(strSelDate);
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
                        string strPath = Settings.Instance.Directories.WorkDirectory + "\\" + strSelDate + "\\" + entry.Key + "\\Capture";

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
                //        string strSend = strSelDate + "*" + percent + "%" + "*" + checkedCnt + "/" + totCnt;
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
            if (bFlag)
            {
                Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < nCount; i ++)
                    {
                        StackPanel ImgStackPanel = (StackPanel)ImageList.Items[i];
                        ImgStackPanel.Width = nWidth;
                        ImgStackPanel.Height = nHeight + 10;

                        Border border = (Border)ImgStackPanel.Children[0];
                        border.Width = nWidth - 10;
                        border.Height = (nWidth - 10) * (67.5 / rateImage / 120.0);//(nHeight - 20) / rateImage;

                        border.Margin = new Thickness(0, nHeight - 10 - ((nHeight - 20) / rateImage), 0, 0);

                        Image image = (Image)(border.Child);
                        image.Width = nWidth - 10;
                        image.Height = nHeight - 20;

                    }
                }, System.Windows.Threading.DispatcherPriority.Normal);

                bFlag = false;


                ListViewItem itemDown = ImageList.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
                itemDown.Focus();
                
                return;
            }
            
            EmptyDisplay();

            Dispatcher.Invoke(() =>
            {

                for (int i = nStart; i < nEnd; i ++)
                {
                    if (nPrevStart <= i && i < nPrevEnd) continue;

                    try
                    {
                        string strFileName = System.IO.Path.GetFileName(fileList[i]);
                        string strRealFileName = Md5Crypto.OnGetRealName(strFileName).Replace("-", ":");
                        byte[] temp = Md5Crypto.OnReadImgFile(fileList[i]);

                        StackPanel ImgStackPanel = new StackPanel();
                        ImgStackPanel.Orientation = Orientation.Vertical;
                        Image imgTemp = new Image();
                        Label lblFileName = new Label();
                        lblFileName.Content = strRealFileName;
                        
                        lblFileName.HorizontalAlignment = HorizontalAlignment.Center;
                        lblFileName.VerticalAlignment = VerticalAlignment.Bottom;
                        lblFileName.Margin = new Thickness(0, 0, 0, 0);

                        BitmapImage imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        MemoryStream ms = new MemoryStream(temp);
                        imageSource.StreamSource = ms;
                        imageSource.EndInit();

                        rateImage = imageSource.Width / Constants.CaptureWidth;

                        imgTemp.Source = imageSource;
                        imgTemp.Width = nWidth - 10;
                        imgTemp.Height = nHeight - 20;
                        
                        Border borderImg = SetBorderEffect();
                        borderImg.Child = imgTemp;

                        borderImg.Margin = new Thickness(0, nHeight - 10 - ((nHeight - 20) / rateImage), 0, 0);
                        
                        ImgStackPanel.Children.Add(borderImg);
                        ImgStackPanel.Children.Add(lblFileName);
                        ImgStackPanel.Width = nWidth;
                        ImgStackPanel.Height = nHeight + 10;
                        
                        ImageList.Items[i] = ImgStackPanel;
                        //ImageList.Items.Add(_grid);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }, System.Windows.Threading.DispatcherPriority.Normal);

            GC.Collect();

            nPrevStart = nStart;
            nPrevEnd = nEnd;
        }

        private void Inspect()
        {
            //MainProc.Instance.Inspect();
        }
        public void UpdateColumnAndRowCounts()
        {
            double width = ImageList.ActualWidth;
            nColumn = (int)(width) / (nWidth + 30);
            nRow = (int)nCount / nColumn;
            if (nCount % nColumn != 0) nRow++;
        }
        
        private void SmallCheckBox_Click(object sender, RoutedEventArgs e)
        {
            chMedium.IsChecked = false;
            chLarge.IsChecked = false;

            if (bCheck == 0)
            {
                bFlag = false;
            } else
            {
                bFlag = true;
            }
            chSmall.IsChecked = true;
            
            nWidth = Constants.smallWidth;
            nHeight = Constants.smallHeight;
            bCheck = 0;
            Display(false);
        }

        private void MediumCheckBox_Click(object sender, RoutedEventArgs e)
        {
            chSmall.IsChecked = false;
            chLarge.IsChecked = false;

            if (bCheck == 1)
            {
                bFlag = false;
            }
            else
            {
                bFlag = true;
            }
            chMedium.IsChecked = true;

            nWidth = Constants.mediumWidth;
            nHeight = Constants.mediumHeight;
            bCheck = 1;
            bFlag = true;
            Display(false);
        }

        private void LargeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            chSmall.IsChecked = false;
            chMedium.IsChecked = false;

            if (bCheck == 2)
            {
                bFlag = false;
            }
            else
            {
                bFlag = true;
            }
            chLarge.IsChecked = true;

            nWidth = Constants.largeWidth;
            nHeight = Constants.largeHeight;
            bCheck = 2;
            Display(false);
        }

        
        private void SetRangeTimerCallback(Object source, ElapsedEventArgs e)
        {
            UpdateColumnAndRowCounts();
            
            Display(true);
        }

        private void listview_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
            stopwatch.Stop();
            long elapsedTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            stopwatch.Start();

            if (elapsedTime < deltaMicroSecond)
            {
                timer.Stop();
                //return;
            }

            Decorator border = VisualTreeHelper.GetChild(ImageList, 0) as Decorator;
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
           
            if (scrollViewer != null)
            {
                
                scrollVerticalChange = e.VerticalChange;
                currentHeight = e.VerticalOffset;
                scrollHeight = scrollViewer.ScrollableHeight;

                timer.Start();

            }
            
        }

        public void SetFocus()
        {
            ImageList.SelectedIndex = nIndex;
            PreviewImageDisplay(nIndex);
            ListViewItem item = ImageList.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
            item.Focus();

        }
        public void SetEnd()
        {
            nEnd = nStart + 100;
            if(nEnd > nCount)
            {
                nEnd = nCount;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            timer.Start();
        }


        
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
           
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateColumnAndRowCounts();

            switch (e.Key)
            {
                case Key.Left:

                    nIndex--;
                    if (nIndex < 0)
                    {
                        nIndex = 0;
                    }
                    
                    break;

                case Key.Right:

                    nIndex++;

                    if (nIndex >= nCount)
                    {
                        nIndex = nCount - 1;
                    }
                    
                    break;

                case Key.Up:
                    nIndex -= nColumn;

                    if (nIndex < 0)
                    {
                        nIndex += nColumn;
                    }
                    
                    break;

                case Key.Down:
                    nIndex += nColumn;

                    if (nIndex >= nCount)
                    {
                        nIndex -= nColumn;
                    }

                    break;
            }

            Console.WriteLine(nIndex);
            PreviewImageDisplay(nIndex);
            ListViewItem item = ImageList.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
            item.Focus();
            e.Handled = true;
            
        }

        
        private Point myMousePlacementPoint;
        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void OnListViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void OnListViewMouseMove(object sender, MouseEventArgs e)
        {
            

        }

       

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void PreviewImageDisplay(int nIndex)
        {
            if (nIndex == -1)
            {
                return;
            }
            try
            {
                byte[] temp = Md5Crypto.OnReadImgFile(fileList[nIndex]);
            
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                MemoryStream ms = new MemoryStream(temp);
                imageSource.StreamSource = ms;
                imageSource.EndInit();
                currentImage.Source = imageSource;
            }
            catch (Exception ex)
            {

            }

            if (nIndex == fileList.Count - 1)
            {
                forwardImage.Source = null;
            }
            else
            {
                try
                {
                    byte[] temp = Md5Crypto.OnReadImgFile(fileList[nIndex + 1]);
                
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    MemoryStream ms1 = new MemoryStream(temp);
                    imageSource.StreamSource = ms1;
                    imageSource.EndInit();
                    forwardImage.Source = imageSource;
                }
                catch (Exception ex)
                {

                }
            }

            if (nIndex == 0)
            {
                backImage.Source = null;
            }
            else
            {
                try
                {
                    byte[] temp = Md5Crypto.OnReadImgFile(fileList[nIndex - 1]);
                
                    BitmapImage imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    MemoryStream ms1 = new MemoryStream(temp);
                    imageSource.StreamSource = ms1;
                    imageSource.EndInit();
                    backImage.Source = imageSource;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            nIndex = ImageList.SelectedIndex;
            PreviewImageDisplay(nIndex);
            ListViewItem item = ImageList.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
            item.Focus();
        }

        private void ListView_MouseDoubleDown(object sender, MouseButtonEventArgs e)
        {
            nIndex = ImageList.SelectedIndex;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ImageModal window = new ImageModal();

                window.OnImageShow(fileList[nIndex]);
                window.ShowDialog();
            });
            ListViewItem item = ImageList.ItemContainerGenerator.ContainerFromIndex(nIndex) as ListViewItem;
            item.Focus();
        }

        

    }
}
