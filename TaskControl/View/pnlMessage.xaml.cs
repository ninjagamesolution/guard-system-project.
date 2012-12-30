using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;
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

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for pnlMessage.xaml
    /// </summary>
    public partial class pnlMessage
    {
        private CustomMessage message;
        
        public pnlMessage(CustomMessage arriveMessage)
        {
            InitializeComponent();
            message = arriveMessage;
            
            
            if (message.state == true)
            {
                this.message_body.Background = new SolidColorBrush(Color.FromRgb(242, 246, 249));
            }
            string type = message.type;
            lbl_IP.Content = message.clientIP;
            lbl_time.Content = message.time;
            if (type == Constants.Re_MsgForbidden)
            {
                try
                {
                    lbl_Name.Content = message.name;
                    lbl_type.Content = "This is the Forbidden Message";
                    lbl_header.Background = new SolidColorBrush(Color.FromRgb(157, 50, 5));
                    lbl_Message.Content = "Process Name:" + message.message;
                }
                catch
                {

                }
                
            }
            
            else if (type == Constants.Re_MsgDownLoading)
            {
                try
                {
                    lbl_Name.Content = message.name;
                    lbl_type.Content = "This user is downloading file now.";
                    lbl_Message.Content = "File Name : " + message.message;
                    lbl_header.Background = new SolidColorBrush(Color.FromRgb(37, 160, 218));
                }
                catch
                {

                }
                
            }else if(type == Constants.Re_MsgDownload)
            {
                try
                {
                    lbl_Name.Content = message.name;
                    lbl_type.Content = "This user has just downloaded file.";
                    lbl_Message.Content = "File Name : " + message.message;
                    lbl_header.Background = new SolidColorBrush(Color.FromRgb(37, 160, 218));
                }
                catch
                {

                }
            }else if(type == Constants.Re_MsgDanger)
            {
                try
                {
                    lbl_Name.Content = message.name;
                    lbl_type.Content = "This user has just used danger URL.";
                    lbl_Message.Content = "Danger URL : " + message.message;
                    lbl_header.Background = new SolidColorBrush(Color.FromRgb(168, 0, 0));
                }
                catch
                {

                }
            }
            else if (type == Constants.Re_MsgAudio)
            {
                try
                {
                    lbl_Name.Content = message.name;
                    lbl_type.Content = "This user listening music data.";
                    lbl_Message.Content = "Process Name : " + message.message;
                    lbl_header.Background = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                }
                catch
                {

                }
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                
                this.message_body.Background = new SolidColorBrush(Color.FromRgb(242,246,249));
                Settings.Instance.MessageList.Where(x => x.index == message.index).FirstOrDefault(c => c.state = true);
                string strFileName = Settings.Instance.Directories.WorkDirectory + @"\" + Constants.MessageFileName;
                if (!File.Exists(strFileName))
                    File.Create(strFileName);

                var list_temp = new List<string>();
                foreach (var item in Settings.Instance.MessageList)
                {
                    string strData = item.index + "*" + item.clientIP + "*" + item.name + "*" + item.message + "*" + item.time + "*" + item.type + "*" + item.state;
                    list_temp.Add(strData);
                }
                Md5Crypto.WriteCryptoFile(Settings.Instance.Directories.WorkDirectory, strFileName, list_temp);
                Windows.MainWindow.LoadState();
                
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }
            
        }
    }
}
