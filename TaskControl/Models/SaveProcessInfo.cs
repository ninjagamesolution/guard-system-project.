using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Logger;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.Utils;
using Monitor.TaskControl.View;

namespace Monitor.TaskControl.Models
{
    public class SaveProcessInfo
    {
        private static object _syncLock;

        public SaveProcessInfo()
        {
            _syncLock = new object();
        }

        public void LoadProcessInfo(string strPath, string clientIP)
        {
            string dbOldPath = "";
            dbOldPath = strPath + "\\" + Constants.DbFileNameTemp;
            strPath = strPath + "\\" + Constants.DbFileName;
            List<string> strTempList = new List<string>();
            strTempList = Md5Crypto.ReadCryptoFile(strPath);
            if (strTempList.Count == 0)
            {
                return;
            }
            //if (!File.Exists(strPath))
            //{
            //    return;
            //}
            //string line = "";
            //int nCount = 1;
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
                    item.LOPBO.ProcessName = strArray[0];
                    item.LOPBO.ProcessWindow = strArray[1];
                    item.LOPBO.ProcessPath = strArray[2];
                    item.LOPBO.ProcessStartTime = DateTime.Parse(strArray[3]);
                    item.LOPBO.ProcessEndTime = DateTime.Parse(strArray[4]);
                    try
                    {
                        item.LOPBO.ProcessColor = strArray[5];
                    }
                    catch
                    {
                        item.LOPBO.ProcessColor = Constants.Default;
                    }
                    item.ProcessList.Add(item.LOPBO);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }

            var strAddData = Md5Crypto.ReadCryptoFile(dbOldPath);
            if (strAddData.Count == 0)
            {
                return;
            }

            foreach (var line in strAddData)
            {
                try
                {
                    if (line == "")
                    {
                        continue;
                    }
                    string[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    item.LOPBO.ProcessName = strArray[0];
                    item.LOPBO.ProcessWindow = strArray[1];
                    item.LOPBO.ProcessPath = strArray[2];
                    item.LOPBO.ProcessStartTime = DateTime.Parse(strArray[3]);
                    item.LOPBO.ProcessEndTime = DateTime.Parse(strArray[4]);
                    try
                    {
                        item.LOPBO.ProcessColor = strArray[5];
                    }
                    catch
                    {
                        item.LOPBO.ProcessColor = Constants.Default;
                    }
                    item.ProcessList.Add(item.LOPBO);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }
        }

        public void RecvProcessInfo(byte[] buf, string clientIP)
        {
            string strProName = ""; string strProWindow = ""; string strProPath = ""; string strProColor = "";

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
                else if (strTemp.Substring(0, 1) == "T")
                {
                    strProColor = strTemp.Substring(1);
                }
            }

            foreach (var forbiddenProcess in Settings.Instance.Forbiddenprocess_list.ToList())
            {
                if (strProName.Contains(forbiddenProcess.Key.Trim()))
                {
                    CommProc.Instance.SendaAnalysis(clientIP, forbiddenProcess.Value.ToString(), Constants.Se_MsgForbidden);
                    string strClientData = clientIP + ":" + Settings.Instance.ClientDic[clientIP].UserName + " turn on the Forbidden Process:" + forbiddenProcess.Value;
                    try
                    {
                        //Windows.AlarmWindow.Show(strClientData);
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        {
                            var notify = new NotificationWindow();
                            notify.ShowNotification(forbiddenProcess.Value.ToString(), clientIP, Settings.Instance.ClientDic[clientIP].UserName, Constants.Re_MsgForbidden);
                            
                        }));
                        Windows.MainWindow.SaveMessage(Constants.Re_MsgForbidden, clientIP, forbiddenProcess.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        CustomEx.DoExecption(Constants.exResume, ex);
                    }
                }
            }

            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            
            if (item.bFirst == true)
            {
                if (item.ProcessList.Count != 0)
                {
                    DateTime endTemp = item.ProcessList[item.ProcessList.Count - 1].ProcessEndTime;

                    item.LOPBO.ProcessName = Constants.DisConnect;
                    item.LOPBO.ProcessWindow = Constants.Unknown;
                    item.LOPBO.ProcessPath = Constants.Unknown;
                    item.LOPBO.ProcessStartTime = endTemp;
                    item.LOPBO.ProcessEndTime = strProTime;
                    item.LOPBO.ProcessColor = "";
                    item.ProcessList.Add(item.LOPBO);

                    item.LOPBO.ProcessName = strProName;
                    item.LOPBO.ProcessWindow = strProWindow;
                    item.LOPBO.ProcessPath = strProPath;
                    item.LOPBO.ProcessStartTime = strProTime;
                    item.LOPBO.ProcessEndTime = strProTime;
                    item.LOPBO.ProcessColor = strProColor;
                    item.ProcessList.Add(item.LOPBO);

                    if (item.ProcessList.Count > 1)
                    {
                        string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                        string strData = Constants.DisConnect + Constants.filePattern + Constants.Unknown + Constants.filePattern + Constants.Unknown + Constants.filePattern + endTemp.ToString() + Constants.filePattern + strProTime.ToString() + Constants.filePattern + "";
                        Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.DbFileName, strData);
                    }
                }
                else
                {
                    item.LOPBO.ProcessName = strProName;
                    item.LOPBO.ProcessWindow = strProWindow;
                    item.LOPBO.ProcessPath = strProPath;
                    item.LOPBO.ProcessStartTime = strProTime;
                    item.LOPBO.ProcessEndTime = strProTime;
                    item.LOPBO.ProcessColor = strProColor;
                    item.ProcessList.Add(item.LOPBO);
                }
                //new file
                string strDbPathTemp = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                string strDataTemp = strProName + Constants.filePattern + strProWindow + Constants.filePattern + strProPath + Constants.filePattern + strProTime.ToString() + Constants.filePattern + strProTime.ToString() + Constants.filePattern + strProColor;
                Md5Crypto.WriteFileReplace(strDbPathTemp, Constants.DbFileNameTemp, strDataTemp);
            }
            else
            {
                if ((int)strProTime.Subtract(item.ProcessList[item.ProcessList.Count - 1].ProcessEndTime).TotalSeconds > (int)item.ActiveDuration)
                {
                    string name = item.ProcessList[item.ProcessList.Count - 1].ProcessName;
                    DateTime startTemp = item.ProcessList[item.ProcessList.Count - 1].ProcessStartTime;
                    DateTime endTemp = item.ProcessList[item.ProcessList.Count - 1].ProcessEndTime;

                    if (Constants.DisConnect != name)
                    {
                        item.LOPBO.ProcessName = Constants.DisConnect;
                        item.LOPBO.ProcessWindow = Constants.Unknown;
                        item.LOPBO.ProcessPath = Constants.Unknown;
                        item.LOPBO.ProcessStartTime = endTemp;
                        item.LOPBO.ProcessEndTime = strProTime;
                        item.LOPBO.ProcessColor = "";
                        item.ProcessList.Add(item.LOPBO);
                        //
                        if (item.ProcessList.Count > 1)
                        {
                            string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                            string strData = item.ProcessList[item.ProcessList.Count - 2].ProcessName + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessWindow + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessPath + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessStartTime.ToString() + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessEndTime.ToString() + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessColor;
                            Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.DbFileName, strData);
                        }
                    }
                    else
                    {
                        item.ProcessList.RemoveAt(item.ProcessList.Count - 1);
                        item.LOPBO.ProcessName = Constants.DisConnect;
                        item.LOPBO.ProcessWindow = Constants.Unknown;
                        item.LOPBO.ProcessPath = Constants.Unknown;
                        item.LOPBO.ProcessStartTime = startTemp;
                        item.LOPBO.ProcessEndTime = strProTime;
                        item.LOPBO.ProcessColor = "";
                        item.ProcessList.Add(item.LOPBO);
                    }
                    strProName = Constants.DisConnect;
                    strProPath = Constants.Unknown;
                    strProWindow = Constants.Unknown;
                    strProColor = "";
                }
                else
                {
                    string name = item.ProcessList[item.ProcessList.Count - 1].ProcessName;
                    string window = item.ProcessList[item.ProcessList.Count - 1].ProcessWindow;
                    string path = item.ProcessList[item.ProcessList.Count - 1].ProcessPath;
                    string color = item.ProcessList[item.ProcessList.Count - 1].ProcessColor;
                    DateTime startTemp = item.ProcessList[item.ProcessList.Count - 1].ProcessStartTime;

                    item.ProcessList.RemoveAt(item.ProcessList.Count - 1);

                    item.LOPBO.ProcessName = name;
                    item.LOPBO.ProcessWindow = window;
                    item.LOPBO.ProcessPath = path;
                    item.LOPBO.ProcessStartTime = startTemp;
                    item.LOPBO.ProcessEndTime = strProTime;
                    item.LOPBO.ProcessColor = color;
                    item.ProcessList.Add(item.LOPBO);

                    if (name != strProName)
                    {
                        item.LOPBO.ProcessName = strProName;
                        item.LOPBO.ProcessWindow = strProWindow;
                        item.LOPBO.ProcessPath = strProPath;
                        item.LOPBO.ProcessStartTime = strProTime;
                        item.LOPBO.ProcessEndTime = strProTime;
                        item.LOPBO.ProcessColor = strProColor;
                        item.ProcessList.Add(item.LOPBO);

                        if (item.ProcessList.Count > 1)
                        {
                            string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                            string strData = item.ProcessList[item.ProcessList.Count - 2].ProcessName + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessWindow + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessPath + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessStartTime.ToString() + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessEndTime.ToString() + Constants.filePattern + item.ProcessList[item.ProcessList.Count - 2].ProcessColor;
                            Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.DbFileName, strData);
                        }
                    }

                    //new file
                    string strDbPathTemp = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                    string strDataTemp = strProName + Constants.filePattern + strProWindow + Constants.filePattern + strProPath + Constants.filePattern + strProTime.ToString() + Constants.filePattern + strProTime.ToString() + Constants.filePattern + strProColor;
                    Md5Crypto.WriteFileReplace(strDbPathTemp, Constants.DbFileNameTemp, strDataTemp);
                }
            }
            item.bFirst = false;
        }

        public void SaveSlideProcess(string strClientIP)
        {
            ClientInfo item = Settings.Instance.ClientDic[strClientIP];
            if (item.ClientIP == strClientIP)
            {
                string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + strClientIP;
                //if (!Directory.Exists(strDbPath))
                //{
                //    Directory.CreateDirectory(strDbPath);
                //}
                //strDbPath = strDbPath + "\\" + Constants.DbFileName;
                List<string> strTempList = new List<string>();
                foreach (ListOfProcessByOrder list in item.ProcessList.ToList())
                {
                    strTempList.Add(list.ProcessName + Constants.filePattern + list.ProcessWindow + Constants.filePattern + list.ProcessPath + Constants.filePattern + list.ProcessStartTime.ToString() + Constants.filePattern + list.ProcessEndTime.ToString());
                }
                Md5Crypto.WriteCryptoFile(strDbPath, Constants.DbFileName, strTempList);
                
            }
        }

        public void SaveProcessFile(byte[] buf, string clientIP)
        {
            try
            {
                string strToday = Settings.Instance.Directories.TodayDirectory;
                if (!Directory.Exists(strToday))
                {
                    return;
                }
                string strPath = Path.Combine(Settings.Instance.Directories.TodayDirectory, clientIP);
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                using (var stream = new FileStream(Path.Combine(strPath, Constants.DbServerFileName), FileMode.Append))
                {
                    stream.Write(buf, 0, buf.Length);
                }

                List<string> ProcessTempList = new List<string>();
                ProcessTempList = Md5Crypto.ReadCryptoFile(Path.Combine(strPath, Constants.DbServerFileName));
                String[] spearator = { Constants.filePattern };

                foreach (var line in ProcessTempList)
                {
                    try
                    {
                        String[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        ListOfProcessByOrder temp;
                        temp.ProcessName = strArray[0];
                        temp.ProcessWindow = strArray[1];
                        temp.ProcessPath = strArray[2];
                        temp.ProcessStartTime = DateTime.Parse(strArray[3]);
                        temp.ProcessEndTime = DateTime.Parse(strArray[4]);
                        try
                        {
                            temp.ProcessColor = strArray[5];
                        }
                        catch
                        {
                            temp.ProcessColor = Constants.Default;
                        }
                        Settings.Instance.ClientDic[clientIP].ProcessServerList.Add(temp);
                    }
                    catch
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }
        }

        public void LoadClientProcessInfo(string strPath, string clientIP)
        {
            //string dbOldPath = "";
            //dbOldPath = strPath + "\\" + Constants.DbFileNameTemp;
            strPath = strPath + "\\" + Constants.DbServerFileName;

            if (!File.Exists(strPath)) return;

            List<string> strTempList = new List<string>();
            strTempList = Md5Crypto.ReadCryptoFile(strPath);
            if (strTempList.Count == 0)
            {
                return;
            }
            //if (!File.Exists(strPath))
            //{
            //    return;
            //}
            //string line = "";
            //int nCount = 1;
            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            item.ProcessServerList.Clear();

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
                    item.LOPBO.ProcessName = strArray[0];
                    item.LOPBO.ProcessWindow = strArray[1];
                    item.LOPBO.ProcessPath = strArray[2];
                    item.LOPBO.ProcessStartTime = DateTime.Parse(strArray[3]);
                    item.LOPBO.ProcessEndTime = DateTime.Parse(strArray[4]);
                    try
                    {
                        item.LOPBO.ProcessColor = strArray[5];
                    }
                    catch
                    {
                        item.LOPBO.ProcessColor = Constants.Default;
                    }
                    item.ProcessServerList.Add(item.LOPBO);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }



        }
    }
}
