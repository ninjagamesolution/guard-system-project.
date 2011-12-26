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
    public class URLProc
    {
        public void LoadURLInfo(string strPath, string clientIP)
        {
            strPath = strPath + "\\" + Constants.urlFileName;
            List<string> strTempList = new List<string>();
            strTempList = Md5Crypto.ReadCryptoFile(strPath);
            if (strTempList.Count == 0)
            {
                return;
            }

            ClientInfo item = new ClientInfo();
            try
            {
                if (Windows.MainWindow.date_Picker.SelectedDate.Value.ToLongDateString() == DateTime.Now.ToLongDateString())
                {
                    item = Settings.Instance.ClientDic[clientIP];
                }
                else
                {
                    item = Settings.Instance.ClientDic_Temp[clientIP];
                }
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }

            String[] spearator = { Constants.filePattern };
            
            foreach (var line in strTempList)
            {
                try
                {
                    string[] strArray = line.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    item.LOU.strWindow = strArray[0];
                    item.LOU.strURL = strArray[1];
                    item.LOU.URLStartTime = DateTime.Parse(strArray[2]);
                    item.LOU.URLEndTime = DateTime.Parse(strArray[3]);
                    item.LOU.BrowserType = (byte)Int32.Parse(strArray[4]);
                    item.URLList.Add(item.LOU);
                }
                catch (Exception ex)
                {
                    CustomEx.DoExecption(Constants.exResume, ex);
                }
            }
        }

        public void RecvURLInfo(byte[] buf, string clientIP)
        {
            string strWindow = ""; string strUrl = ""; byte BrowserType;

            string strBuf = System.Text.Encoding.UTF8.GetString(buf);
            String[] spearator = { Constants.filePattern };
            String[] strlist = strBuf.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            if (strlist[0] == strBuf)
            {
                return;
            }

            DateTime CurTime = DateTime.Now;
            strWindow = strlist[0];
            strUrl = strlist[1];
            BrowserType = (byte)Int32.Parse(strlist[3]);
            

            ClientInfo item = Settings.Instance.ClientDic[clientIP];
            if (item.URLList.Count < 1)
            {
                string strDbFilePath = Settings.Instance.Directories.TodayDirectory + "\\" + clientIP;
                LoadURLInfo(strDbFilePath, clientIP);
            }

            if (item.bFirstUrl == true)
            {
                item.LOU.strWindow = strWindow;
                item.LOU.strURL = strUrl;
                item.LOU.BrowserType = BrowserType;
                item.LOU.URLStartTime = CurTime;
                item.LOU.URLEndTime = CurTime.AddSeconds(Constants.urlSessionTime);
                item.URLList.Add(item.LOU);

                foreach (string dangerURL in Settings.Instance.ForbiddenURLList.ToList())
                {
                    if (strUrl.ToLower().Contains(dangerURL.ToLower()) && !strUrl.ToLower().Contains(Constants.TranslateCom.ToLower()) && !strUrl.ToLower().Contains(Constants.Updating.ToLower()))
                    {
                        CommProc.Instance.SendaAnalysis(clientIP, strUrl, Constants.Se_MsgDanger);
                        string strClientData = clientIP + ":" + Settings.Instance.ClientDic[clientIP].UserName + " turn on the danger url:" + strUrl;
                        try
                        {
                            //Windows.AlarmWindow.Show(strClientData);
                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                            () =>
                            {
                                var notify = new NotificationWindow();
                                notify.ShowNotification(strUrl, clientIP, Settings.Instance.ClientDic[clientIP].UserName, Constants.Re_MsgDanger);

                            }));
                            Windows.MainWindow.SaveMessage(Constants.Re_MsgDanger, clientIP, strUrl);
                        }
                        catch (Exception ex)
                        {
                            CustomEx.DoExecption(Constants.exResume, ex);
                        }
                    }
                }
            }
            else
            {
                if (strUrl != item.URLList[item.URLList.Count - 1].strURL)
                {
                    item.LOU.strWindow = strWindow;
                    item.LOU.strURL = strUrl;
                    item.LOU.BrowserType = BrowserType;
                    item.LOU.URLStartTime = CurTime;
                    item.LOU.URLEndTime = CurTime.AddSeconds(Constants.urlSessionTime);
                    item.URLList.Add(item.LOU);

                    foreach (string dangerURL in Settings.Instance.ForbiddenURLList.ToList())
                    {
                        if (strUrl.Contains(dangerURL))
                        {
                            CommProc.Instance.SendaAnalysis(clientIP, strUrl, Constants.Se_MsgDanger);
                            string strClientData = clientIP + ":" + Settings.Instance.ClientDic[clientIP].UserName + " turn on the danger url:" + strUrl;
                            try
                            {
                                //Windows.AlarmWindow.Show(strClientData);
                                App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                                () =>
                                {
                                    var notify = new NotificationWindow();
                                    notify.ShowNotification(strUrl, clientIP, Settings.Instance.ClientDic[clientIP].UserName, Constants.Re_MsgDanger);

                                }));
                                Windows.MainWindow.SaveMessage(Constants.Re_MsgDanger, clientIP, strUrl);
                            }
                            catch (Exception ex)
                            {
                                CustomEx.DoExecption(Constants.exResume, ex);
                            }
                        }
                    }

                    if (item.URLList.Count > 2)
                    {
                        string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                        string strData = item.URLList[item.URLList.Count - 2].strWindow + Constants.filePattern + item.URLList[item.URLList.Count - 2].strURL + Constants.filePattern + item.URLList[item.URLList.Count - 2].URLStartTime.ToString() + Constants.filePattern + item.URLList[item.URLList.Count - 2].URLEndTime.ToString() + Constants.filePattern + item.URLList[item.URLList.Count - 2].BrowserType.ToString();
                        Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.urlFileName, strData);
                    }
                }
                else
                {
                    if (CurTime.Subtract(item.URLList[item.URLList.Count - 1].URLEndTime).TotalSeconds < Constants.urlActiveTime + 4)
                    {
                        DateTime startTemp = item.URLList[item.URLList.Count - 1].URLStartTime;
                        item.URLList.RemoveAt(item.URLList.Count - 1);

                        item.LOU.strWindow = strWindow;
                        item.LOU.strURL = strUrl;
                        item.LOU.BrowserType = BrowserType;
                        item.LOU.URLStartTime = startTemp;
                        item.LOU.URLEndTime = CurTime.AddSeconds(Constants.urlSessionTime);
                        item.URLList.Add(item.LOU);
                    }
                    else
                    {
                        item.LOU.strWindow = strWindow;
                        item.LOU.strURL = strUrl;
                        item.LOU.BrowserType = BrowserType;
                        item.LOU.URLStartTime = CurTime;
                        item.LOU.URLEndTime = CurTime.AddSeconds(Constants.urlSessionTime);
                        item.URLList.Add(item.LOU);

                        if (item.URLList.Count > 2)
                        {
                            string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + item.ClientIP;
                            string strData = item.URLList[item.URLList.Count - 2].strWindow + Constants.filePattern + item.URLList[item.URLList.Count - 2].strURL + Constants.filePattern + item.URLList[item.URLList.Count - 2].URLStartTime.ToString() + Constants.filePattern + item.URLList[item.URLList.Count - 2].URLEndTime.ToString() + Constants.filePattern + item.URLList[item.URLList.Count - 2].BrowserType.ToString();
                            Md5Crypto.WriteCryptoFileAppend(strDbPath, Constants.urlFileName, strData);
                        }
                    }
                }
            }
            item.bFirstUrl = false;   
        }

        //public void SaveURLInfo(string strClientIP)
        //{
        //    ClientInfo item = Settings.Instance.ClientDic[strClientIP];
        //    if (item.ClientIP == strClientIP)
        //    {
        //        string strDbPath = Settings.Instance.Directories.TodayDirectory + "\\" + strClientIP;
        //        List<string> strTempList = new List<string>();
        //        foreach (ListOfUrl list in item.URLList.ToList())
        //        {
        //            strTempList.Add(list.strWindow + Constants.filePattern + list.strURL + Constants.filePattern + list.URLStartTime.ToString() + Constants.filePattern + list.URLEndTime.ToString() + Constants.filePattern + list.BrowserType.ToString());
        //        }
        //        Md5Crypto.WriteCryptoFile(strDbPath, Constants.urlFileName, strTempList);
        //    }
        //}
    }
}
