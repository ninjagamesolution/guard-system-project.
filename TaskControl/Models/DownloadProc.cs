using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;
using Monitor.TaskControl.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Monitor.TaskControl.Models
{
    public class DownloadProc
    {
        public void RecvDownloadInfo(byte[] buf, string clientIP)
        {
            
            string strBuf = System.Text.Encoding.UTF8.GetString(buf);
            
            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            strBuf = item.UserName + Constants.filePattern + clientIP + Constants.filePattern + strBuf;
            Settings.Instance.DownloadList.Add(strBuf);
            string strPath = Settings.Instance.Directories.TodayDirectory;
            Md5Crypto.WriteCryptoFileAppend(strPath, Constants.DownloadFileName, strBuf);

            String[] spearator = { Constants.filePattern };
            string[] strArray = strBuf.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            string strData = clientIP + " : " + Settings.Instance.ClientDic[clientIP].UserName + ": " + "downloaded file-> " + strArray[2];
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                () =>
                {
                    var notify = new NotificationWindow();
                    notify.ShowNotification(strArray[2],clientIP, Settings.Instance.ClientDic[clientIP].UserName,Constants.Re_MsgDownload);

                }));
            //Settings.Instance.DownloadList.Add(strBuf);
            //SaveDownloadInfo();
        }

        //public void SaveDownloadInfo()
        //{
        //    string strPath = Settings.Instance.Directories.TodayDirectory;
        //    Md5Crypto.WriteCryptoFile(strPath, Constants.DownloadFileName, Settings.Instance.DownloadList);
        //}
    }
}
