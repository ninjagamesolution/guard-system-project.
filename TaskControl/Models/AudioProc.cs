using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Monitor.TaskControl.Models
{
    public class AudioProc
    {
        public void LoadAudioInfo(string strPath, string clientIP)
        {
            strPath = strPath + "\\" + Constants.AudioFileName;
            List<string> strTempList = new List<string>();
            strTempList = Md5Crypto.ReadCryptoFile(strPath);
            if (strTempList.Count == 0)
            {
                return;
            }
            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            String[] spearator = { Constants.filePattern };
            foreach (var line in strTempList)
            {
                try
                {
                    if (line == "")
                    {
                        continue;
                    }
                    string[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    item.LOA.ProcessName = strArray[0];
                    item.LOA.ProcessWindow = strArray[1];
                    item.LOA.ProcessPath = strArray[2];
                    item.LOA.ProcessStartTime = DateTime.Parse(strArray[3]);
                    item.LOA.ProcessEndTime = DateTime.Parse(strArray[4]);
                    item.LOA.FileName = strArray[5];
                    item.LOA.FileSize = strArray[6];
                    item.AudioList.Add(item.LOA);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }
            
        }

        public void RecvAudioInfo(byte[] buf, string clientIP)
        {
            string strProName = ""; string strProWindow = ""; string strProPath = ""; DateTime startTemp = new DateTime(); DateTime endTemp = new DateTime();
            string strFileName = ""; string strFileSize = "";

            string strBuf = System.Text.Encoding.UTF8.GetString(buf);
            String[] spearator = { Constants.filePattern };
            String[] strlist = strBuf.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

            if (strlist[0] == strBuf)
            {
                return;
            }

            DateTime strProTime = DateTime.Now;

            foreach (string strTemp in strlist)
            {
                if (strTemp.Substring(0, 1) == "N")
                {
                    strProName = strTemp.Substring(1);
                }
                else if (strTemp.Substring(0, 1) == "W")
                {
                    strProWindow = strTemp.Substring(1);
                }
                else if (strTemp.Substring(0, 1) == "P")
                {
                    strProPath = strTemp.Substring(1);
                }
                else if (strTemp.Substring(0, 1) == "S")
                {
                    startTemp = DateTime.Parse(strTemp.Substring(1));
                }
                else if (strTemp.Substring(0, 1) == "E")
                {
                    endTemp = DateTime.Parse(strTemp.Substring(1));
                }
                else if (strTemp.Substring(0, 1) == "F")
                {
                    strFileName = strTemp.Substring(1);
                }
                else if (strTemp.Substring(0, 1) == "Z")
                {
                    strFileSize = strTemp.Substring(1);
                }
            }

            ClientInfo item = Settings.Instance.ClientDic[clientIP];

            item.LOA.ProcessName = strProName;
            item.LOA.ProcessWindow = strProWindow;
            item.LOA.ProcessPath = strProPath;
            System.TimeSpan diff = endTemp.Subtract(startTemp);
            item.LOA.ProcessStartTime = strProTime - diff;
            item.LOA.ProcessEndTime = strProTime;
            item.LOA.FileName = strFileName;
            item.LOA.FileSize = strFileSize;
            item.AudioList.Add(item.LOA);

            string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
            string strData = strProName + Constants.filePattern + strProWindow + Constants.filePattern + strProPath + Constants.filePattern + item.AudioList[item.AudioList.Count - 1].ProcessStartTime.ToString() + Constants.filePattern + strProTime.ToString() + Constants.filePattern + strFileName + Constants.filePattern + strFileSize;
            Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.AudioFileName, strData);

            //if (item.AudioList.Count == 0)
            //{
            //    item.LOA.ProcessName = strProName;
            //    item.LOA.ProcessWindow = strProWindow;
            //    item.LOA.ProcessPath = strProPath;
            //    item.LOA.ProcessStartTime = strProTime;
            //    item.LOA.ProcessEndTime = strProTime;
            //    item.AudioList.Add(item.LOA);
            //}
            //else
            //{
            //    if ((int)strProTime.Subtract(item.AudioList[item.AudioList.Count - 1].ProcessEndTime).TotalSeconds < (int)Constants.nAudioSession + 2)
            //    {
            //        string strProcessNameTemp = item.AudioList[item.AudioList.Count - 1].ProcessName;
            //        string strProcessWindowTemp = item.AudioList[item.AudioList.Count - 1].ProcessWindow;
            //        string strProcessPathTemp = item.AudioList[item.AudioList.Count - 1].ProcessPath;
            //        DateTime startTemp = item.AudioList[item.AudioList.Count - 1].ProcessStartTime;
            //        item.AudioList.RemoveAt(item.AudioList.Count - 1);
            //        item.LOA.ProcessName = strProcessNameTemp;
            //        item.LOA.ProcessWindow = strProcessWindowTemp;
            //        item.LOA.ProcessPath = strProcessPathTemp;
            //        item.LOA.ProcessStartTime = startTemp;
            //        item.LOA.ProcessEndTime = strProTime;
            //        item.AudioList.Add(item.LOA);
            //    }
            //    else
            //    {
            //        string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
            //        string strData = item.AudioList[item.AudioList.Count - 1].ProcessName + Constants.filePattern + item.AudioList[item.AudioList.Count - 2].ProcessWindow + Constants.filePattern + item.AudioList[item.AudioList.Count - 2].ProcessPath + Constants.filePattern + item.AudioList[item.AudioList.Count - 2].ProcessStartTime.ToString() + Constants.filePattern + item.AudioList[item.AudioList.Count - 2].ProcessEndTime.ToString();
            //        Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.AudioFileName, strData);

            //        item.LOA.ProcessName = strProName;
            //        item.LOA.ProcessWindow = strProWindow;
            //        item.LOA.ProcessPath = strProPath;
            //        item.LOA.ProcessStartTime = strProTime;
            //        item.LOA.ProcessEndTime = strProTime;
            //        item.AudioList.Add(item.LOA);
            //    }
            //}
        }
    }
}
