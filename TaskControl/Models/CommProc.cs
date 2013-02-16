using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Monitor.TaskControl.Communication;
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Models;
using Monitor.TaskControl.View;

namespace Monitor.TaskControl.Models
{
    class CommProc
    {
        private static CommProc _instance;
        public ScreenCaptures ScrCapture = new ScreenCaptures();
        public SaveProcessInfo PrcInfo = new SaveProcessInfo();
        public DownloadProc dwlProc = new DownloadProc();
        public URLProc URLInfo = new URLProc();
        public AudioProc AudioInfo = new AudioProc();
        public AudioDataProc AudioDataInfo = new AudioDataProc();
        
        public static CommProc Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommProc();
                }
                return _instance;
            }
        }

        public void RecDataAnalysis(byte[] buf, int length, string ClientIP)
        {

            string prefix = Encoding.UTF8.GetString(buf, 0, 4);
            byte[] temp = new byte[length - 4];
            Array.Copy(buf, 4, temp, 0, length - 4);
            if (prefix == Constants.Re_ClientInfo)
            {
                string strClientData = Encoding.UTF8.GetString(temp);
                bool bFlag = false;
                string[] clientData = strClientData.Split(';');
                foreach (var item in Settings.Instance.ClientDic.AsParallel())
                {
                    if (item.Key == ClientIP)
                    {
                        bFlag = true;
                    }
                }

                if (bFlag == false)
                {
                    ClientInfo client = new ClientInfo()
                    {
                        ClientIP = ClientIP,
                        NetworkState = true,
                        UserName = clientData[0],
                        Password = clientData[1],
                        Company = clientData[2],
                        SessionTime = Convert.ToInt32(clientData[3]),
                        CaptureTime = Convert.ToInt32(clientData[4]),
                        SlideWidth = Convert.ToInt32(clientData[5]),
                        SlideHeight = Convert.ToInt32(clientData[6]),
                        CaptureWidth = Convert.ToInt32(clientData[7]),
                        CaptureHeight = Convert.ToInt32(clientData[8]),
                        PCName = clientData[9],
                        OSDate = clientData[10]
                    };
                    try
                    {
                        client.ActiveDuration = Convert.ToInt32(clientData[13]);
                    }
                    catch
                    {
                        client.ActiveDuration = Constants.ActiveDuration;
                    }



                    Windows.MainWindow.SaveUserInformation(client);
                    Settings.Instance.ClientDic.Add(ClientIP, client);
                    Windows.MainWindow.DeletedUserLoadInfo(client);
                    Windows.MainWindow.ShowUserList(client);
                    //try
                    //{
                    //    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                    //    () =>
                    //    {
                    //        new NotificationWindow().Show("Connect", ClientIP, clientData[0], Constants.Re_ClientInfo);
                    //        Windows.MainWindow.SaveMessage(Constants.Re_ClientInfo, ClientIP, "Connect");
                    //    }));
                    //}
                    //catch
                    //{

                    //    //Windows.MainWindow.ShowOsHistory();
                    //}
                }
                else
                {
                    Settings.Instance.ClientDic[ClientIP].UserName = clientData[0];
                    Settings.Instance.ClientDic[ClientIP].Company = clientData[2];
                    Settings.Instance.ClientDic[ClientIP].OSDate = clientData[10];
                    Settings.Instance.ClientDic[ClientIP].NetworkState = true;
                    
                    //Windows.MainWindow.ShowOsHistory();
                }
                
            }
            else if(prefix == Constants.Re_SetInfo)
            {
                string strClientData = Encoding.UTF8.GetString(temp);
                string[] clientData = strClientData.Split(':');
                Settings.Instance.ClientDic[ClientIP].UserName = clientData[0];
                Settings.Instance.ClientDic[ClientIP].Company = clientData[2];
                Windows.MainWindow.ReplaceUserInfomation();
                Windows.MainWindow.ReplaceUserList(Settings.Instance.ClientDic[ClientIP]);
            }
            else if (prefix == Constants.Re_Forbidden)
            {

            }
            else if(prefix == Constants.Re_End)
            {
                //Settings.Instance.SocketDic[]
                Settings.Instance.ClientDic[ClientIP].NetworkState = false;
                

            }

            else if (prefix == Constants.Re_MsgAudio)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgAudio, ClientIP, message);
            }
            else if (prefix == Constants.Re_MsgUSB)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgUSB, ClientIP, message);
            }
            else if (prefix == Constants.Re_MsgDownload)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgDownload, ClientIP, message);
            }
            else if (prefix == Constants.Re_MsgDownLoading)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgDownLoading, ClientIP, message);
            }
            else if (prefix == Constants.Re_MsgForbidden)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgForbidden, ClientIP, message);
            }
            else if (prefix == Constants.Re_MsgAudio)
            {
                string message = Encoding.UTF8.GetString(temp);
                Windows.MainWindow.SaveMessage(Constants.Re_MsgAudio, ClientIP, message);
            }
            else if (prefix == Constants.Re_DataSlide)
            {
                byte[] buffer = new byte[length];
                Array.Copy(buf, 0, buffer, 0, length);
                ScrCapture.SaveSlideImage(buffer, ClientIP);
            }
            else if (prefix == Constants.Re_DataCapture)
            {
                byte[] buffer = new byte[length];
                Array.Copy(buf, 0, buffer, 0, length);
                ScrCapture.SaveCaputreImage(buffer, ClientIP);
            }
            //else if(prefix == Constants.Re_DataHuman)
            //{
            //    byte[] buffer = new byte[length];
            //    Array.Copy(buf, 0, buffer, 0, length);
            //    ScrCapture.SaveHumanImage(buffer, ClientIP);
            //}
            else if (prefix == Constants.Re_DataProcess)
            {
                PrcInfo.RecvProcessInfo(temp, ClientIP);
                //PrcInfo.SaveSlideProcess(ClientIP);
            }
            else if (prefix == Constants.Re_DataAudio)
            {
                AudioInfo.RecvAudioInfo(temp, ClientIP);
            }
            else if (prefix == Constants.Re_DataUSB)
            {

            }
            else if (prefix == Constants.Re_DataURL)
            {
                URLInfo.RecvURLInfo(temp, ClientIP);
                //URLInfo.SaveURLInfo(ClientIP);
            }
            else if (prefix == Constants.Re_DataDownload)
            {
                dwlProc.RecvDownloadInfo(temp, ClientIP);
            }
            else if (prefix == Constants.Re_DataForbidden)
            {

            }
            else if (prefix == Constants.Re_VidCapture)
            {
                try
                {
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                    () =>
                    {
                        Windows.CaptureWindow.ShowVideo(temp);

                    }));
                }
                catch(Exception ex)
                {

                }
                
                
            }
            else if (prefix == Constants.Re_VidAudio)
            {

            }
            else if (prefix == Constants.Re_AudioData)
            {
                AudioDataInfo.RecvAudioDataInfo(temp, ClientIP);
            }
            else if (prefix == Constants.Re_ServerSlideData)
            {
                string fileName = Encoding.UTF8.GetString(temp, 0, 8);
                byte[] filetemp = new byte[temp.Length - 8];
                Array.Copy(temp, 8, filetemp, 0, temp.Length - 8);

                ScrCapture.SaveSlideImage(filetemp, ClientIP, fileName);

                Thread.Sleep(2000);
            }
            else if (prefix == Constants.Re_ServerCaptureData)
            {
                string fileName = Encoding.UTF8.GetString(temp, 0, 8);
                byte[] filetemp = new byte[temp.Length - 8];
                Array.Copy(temp, 8, filetemp, 0, temp.Length - 8);

                ScrCapture.SaveCaputreImage(filetemp, ClientIP, fileName);

                Thread.Sleep(2000);

            }
            else if (prefix == Constants.Re_ServerProcessData)
            {
                byte[] buffer = new byte[temp.Length];
                Array.Copy(temp, 0, buffer, 0, temp.Length);
                PrcInfo.SaveProcessFile(buffer, ClientIP);
                Thread.Sleep(1000);
            }

        }
        public void SendaAnalysis(string ClientIP, string message, string Type)
        {
            foreach(var item in Settings.Instance.SocketDic.ToList())
            {
                try
                {
                    if (item.Value.IPAddress == ClientIP)
                    {
                        string strMessage = Type + message;
                        item.Value.Send(strMessage);
                    }
                }
                catch(Exception ex)
                {
                    handleClient client = Settings.Instance.SocketDic[item.Key];
                    string client_ip = Communications.GetIPAddressOfClient(item.Key);
                    Settings.Instance.SocketDic.Remove(item.Key);
                    Settings.Instance.ClientDic[client_ip].NetworkState = false;
                    client.Close();
                }
                
            }
        }
    }
}