using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow()
        : base()
        {
            this.InitializeComponent();
            this.Closed += this.NotificationWindowClosed;
        }

        private void NotificationWindowClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ShowNotification(string Message, string clientIP, string clientName, string strType)
        {
            this.Topmost = true;
            base.Show();

            //this.Owner = System.Windows.Application.Current.MainWindow;
            if(strType == Constants.Re_MsgForbidden)
            {
                lbl_message.Content = "This user is  running Forbbiden Program";
                lbl_information.Content = "Forbbiden Program: " + Message;
            }
            else if(strType == Constants.Re_MsgDownLoading)
            {
                lbl_message.Content = "This user is downloading file.";
                lbl_information.Content = "File Name : " + Message;
            }
            else if(strType == Constants.Re_MsgDownload)
            {
                lbl_message.Content = "This user have downloaded file.";
                lbl_information.Content = "File Name : " + Message;

            }
            else if(strType == Constants.Re_ClientInfo)
            {
                lbl_message.Content = "This user have connected.";
                lbl_information.Content = "User State : " + Message;
            }
            else if(strType == Constants.Re_Confirm)
            {
                lbl_message.Content = "This user have disconnected.";
                lbl_information.Content = "User State : " + Message;
            }else if(strType == Constants.Re_MsgDanger)
            {
                lbl_message.Content = "This user use the Danger URL.";
                lbl_information.Content = "Danger URL:" + Message;
            }
            this.ClientIP.Content = clientIP;
            this.ClientName.Content = clientName;
            this.alarm_date.Content = DateTime.Now.ToString();
            this.Closed += this.NotificationWindowClosed;
            var workingArea = Screen.PrimaryScreen.WorkingArea;

            this.Left = workingArea.Right - this.ActualWidth;
            double top = workingArea.Bottom - this.ActualHeight;

            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                string windowName = window.GetType().Name;

                if (windowName.Equals("NotificationWindow") && window != this)
                {
                    // Adjust any windows that were above this one to drop down
                    if (window.Top < this.Top)
                    {
                        window.Top = window.Top + this.ActualHeight;
                    }
                }
            }

            this.Top = top;
        }
        private void ImageMouseUp(object sender,  System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void DoubleAnimationCompleted(object sender, EventArgs e)
        {
            if (!this.IsMouseOver)
            {
                //Settings.Instance.ni.Dispose();
                this.Close();
            }
           
        }


        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Settings.Instance.ni.Dispose();
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Settings.Instance.ni.Dispose();
            this.Close();
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Settings.Instance.ni.Dispose();
            Thread.Sleep(500);
            this.Close();
        }
    }
}
