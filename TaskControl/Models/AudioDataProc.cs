using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Monitor.TaskControl.Models
{
    public class AudioDataProc
    {
        public void RecvAudioDataInfo(byte[] buf, string clientIP)
        {
            string strFileName = Encoding.UTF8.GetString(buf, 0, 30);
            strFileName = strFileName.Replace("\0", "");
            byte[] temp = new byte[buf.Length - 30];
            Array.Copy(buf, 30, temp, 0, buf.Length - 30);
            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            string strDataPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP + "\\Audio";
            if (!Directory.Exists(strDataPath))
            {
                Directory.CreateDirectory(strDataPath);
            }

            strDataPath = strDataPath + "\\" + strFileName;

            try
            {
                App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                () =>
                {
                    File.WriteAllBytes(strDataPath, temp);

                }));
            }
            catch (Exception ex)
            {

            }
            
        }
    }
}
