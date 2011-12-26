using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// Interaction logic for RemoteWindow.xaml
    /// </summary>
    public partial class RemoteWindow : Window
    {
        private string clientIP;
        private StreamWriter eventSender;
        private Thread theThread;
        private int resolutionX;
        private int resolutionY;
        private bool bFlag;
        public RemoteWindow(string strIP)
        {
            InitializeComponent();
            clientIP = strIP;
            this.Title = Settings.Instance.ClientDic[strIP].UserName + ":" + strIP;
            bFlag = false;
            
        }
        public void ShowVideo(byte[] buffer)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = ms;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                myImage.Source = bitmapimage;
            }
            catch(Exception ex)
            {
                Utils.CustomEx.DoExecption(Constants.exRepair, ex);
            }
            
        }

        

        public bool SafeSendValue(string Text)
        {
            int Shift = 45;
            if (Text.Length == 1) Shift = 77;//Change the shift for a single key press just to confuse anyone looking
            if (Settings.Instance.ClientDic[clientIP].NetworkState == false) return false;
            try
            {
                if(bFlag == true)
                {
                    CommProc.Instance.SendaAnalysis(clientIP, Text, Constants.Se_VidCMD);
                }
                
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }
        public void SendCommand(string Cmd)
        {
            SafeSendValue("CMD " + Cmd.ToUpper());
        }

        public void sendBlackScreen()
        {
            SafeSendValue("BLACKSCREEN");
        }

        public void sendMatrixText(String text)
        {
            SafeSendValue("BSTEXT " + text);
        }

        public void SendScreenSize()
        {
            //SafeSendValue("SCREEN " + Settings.ScreenServerX + " " + Settings.ScreenServerY);
        }


        public Bitmap ResizeImage(Bitmap B, int Width, int Height)
        {//Make the image fit our screen if it was compressed down to speed the network up
            Bitmap BNew = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(BNew);
            float scaleFactorX = (float)Width / (float)B.Width;
            float scaleFactorY = (float)Height / (float)B.Height;
            G.ScaleTransform(scaleFactorX / 1.25f, scaleFactorY / 1.25f);
            G.DrawImage(B, 0, 0);
            return BNew;
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommProc.Instance.SendaAnalysis(clientIP, "End", Constants.Se_VidCapture);
        }

        private void MyImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e.LeftButton);

            if (e.LeftButton == MouseButtonState.Released)
                SafeSendValue("LUP");
            else if (e.RightButton == MouseButtonState.Released)
                SafeSendValue("RUP");
        }

        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                SafeSendValue("RDOWN");
            else if (e.RightButton == MouseButtonState.Released)
                SafeSendValue("LDOWN");
        }

        private void MyCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            try
            {//We are not full screen so scale the mouse X,Y to suite the current size
                double mouseX = e.GetPosition(MyCanvas).X;
                double mouseY = e.GetPosition(MyCanvas).Y;
                
                double correctX = (double)Screen.PrimaryScreen.Bounds.Width * ((double)mouseX / (MyCanvas.Width));
                double correctY = (double)Screen.PrimaryScreen.Bounds.Height * ((double)mouseY / (MyCanvas.Height));
                correctX = ((int)correctX);
                correctY = ((int)correctY);
                SafeSendValue("M" + correctX + " " + correctY);
            }
            catch (Exception)
            {

            }
        }

        private void MyCanvas_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                //String keysToSend = "KEY";
                //if(e.Key == Key.RightShift || e.Key == Key.F5 || e.Key == Key.F10)
                //{
                //    return;
                //}
                //if(e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                //{
                //    keysToSend += "Ctrl ";
                //}
                //keysToSend += e.Key.ToString().ToLower();
                //SafeSendValue(keysToSend);
            }
            catch (Exception)
            {

            }
        }

       

        

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (bFlag == true)
            {
                Console.WriteLine("true");
                bFlag = false;
            }
            else
            {
                bFlag = true;
                Console.WriteLine("false");
            }
        }
    }
}
