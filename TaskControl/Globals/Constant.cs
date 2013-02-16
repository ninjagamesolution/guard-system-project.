using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Globals
{
    enum Browser { Chrome = 1, Firefox, Edge, IE };
    internal static class Constants
    {

        internal static readonly string version = "TaskView Control 1.7.8b";
        internal static readonly string current_version = "1.7.8b";

        internal static readonly string LoaderMainLogFileName = "TaskControl.Log.txt";
        internal static readonly string InspectServer = "192.168.109.250";
        internal static readonly string RestProcess = "RestProcess";
        internal static readonly string DisConnect = "DisConnect";
        internal static readonly string Unknown = "Unknown";
        internal static readonly string TotalWTime = "Work Time : ";
        internal static readonly string TotalDTime = "Disconnection Time : ";
        internal static readonly string TotalRTime = "Rest Time : ";
        internal static readonly string SlideImage = "Slide Image : ";
        internal static readonly string ScreenShot = "ScreenShot : ";
        internal static readonly string Session_Time = "Session Time : ";
        internal static readonly string Capture_Time = "Capture Time : ";
        internal static readonly string Active_Time = "Active Time : ";
        internal static readonly string Work = "Work ";
        internal static readonly string Rest = "Rest ";
        internal static readonly string TranslateCom = "translate.";
        internal static readonly string Updating = "updating";

        internal static readonly string Default = "Default";

        //Filter
        internal static readonly string[] videoFilterArray = { ".mp4", ".mpg", ".mpeg", ".mov", ".avi", ".flv", ".mkv", ".wmv", ".webm" };
        internal static readonly string[] audioFilterArray = { ".mp3", ".wav", ".aac", ".wma" };
        internal static readonly string[] imageFilterArray = { ".png", ".jpg", ".svg", ".bmp", ".jpeg", ".gif", ".psd", ".tiff", ".eps", ".tif" };

        //Image Explorer
        internal static readonly int smallWidth = 130;
        internal static readonly int smallHeight = 90;
        internal static readonly int mediumWidth = 200;
        internal static readonly int mediumHeight = 140;
        internal static readonly int largeWidth = 350;
        internal static readonly int largeHeight = 240;

        //Allow URL
        internal static readonly string[] allowURLArray = { "https://www.freelancer.com", "https://www.upwork.com", "https://www.fiverr.com", "https://www.workana.com", "https://www.guru.com", "https://www.peopleperhour.com", "https://web.skype.com", "https://web.slack.com", "https://translate.google.com" };

        //Danger URL
        internal static readonly string[] dangerURLArray = { "youtube", "facebook", "social", "twitter", "linkedin", "sex", "adult", "dating", "vpn", "vps", "proxy", "korea", ".ko" };

        //Forbidden Process
        internal static readonly string[] forbiddenProcessArray = { "iexplore.exe,Internet Explorer", "opera.exe,Opera", "msedge.exe,Microsoft Edge", "browser.exe,Yandex", "Ferdi.exe,Ferdi", "Franz.exe,Franz", "vlc.exe,VLC Player", "QQPlayer.exe,QQPlayer", "Bluestacks.exe,Bluestacks", "Nox.exe,Nox", "CCleaner64.exe,CCleaner", "WasherSvc.exe,Washer" };

        //Hiden processes
        internal static readonly string HideProcess_IDLE = "Idle.exe";
        internal static readonly string HideProcess_LockApp = "LockApp.exe";
        internal static readonly string HideProcess_APH = "ApplicationFrameHost.exe";

        //URL Hooking
        internal static readonly int urlSessionTime = 3;
        internal static readonly int urlActiveTime = 10;


        //File name
        internal static readonly string RestInspectFileName = "RestInspect.lib";
        internal static readonly string DbServerFileName = "Contents_Server.lib";
        internal static readonly string DbFileName = "Contents.lib";
        internal static readonly string DbFileNameTemp = "Contents_A.lib";
        internal static readonly string USBFileName = "USBHistory.lib";
        internal static readonly string OSFileName = "OSHistory.lib";
        internal static readonly string UserFileName = "MonoUI.pak";
        internal static readonly string MessageFileName = "Redist.pak";
        internal static readonly string urlFileName = "Browser.dat";
        internal static readonly string ForbiddenFileName = "Disable.lib";
        internal static readonly string AllowURLFileName = "Allow.lib";
        internal static readonly string ForbiddenURLFileName = "DisableURL.lib";
        internal static readonly string DownloadFileName = "D3DCompiler_47.dll";
        internal static readonly string AudioFileName = "Auo.lib";
        internal static readonly string InspectFileName = "Inspect.lib";

        internal static readonly string[] strArray = { "A", "C", "E", "G", "I", "K", "M", "Q", "R", "T" };
        internal static readonly string strImgExtension = "psl";
        internal static readonly string AudioFolder = "Audio";

        //File Pattern
        internal static readonly string filePattern = "*!!*";

        //Audio Session time
        internal static readonly int nAudioSession = 3;
        internal static readonly string AudioExtension = ".wma";

        // Eviroment Path
        internal static readonly string SetupPath = "C:\\Program Files (x86)\\RM\\Server";
        internal static readonly string RegPath = "Ryonbong\\Server";
        internal static readonly string BaseDirectory = "D:\\CaptureData";
        internal static readonly string Version = "1.0";


        internal static readonly int AutoPatchPort = 19995;
        internal static readonly string AutoPatchServerIP = "192.168.109.250";

        // Exeption
        internal static readonly Byte exExit = 1;
        internal static readonly Byte exRepair = 2;
        internal static readonly Byte exResume = 3;

        // Server base setting
        internal static readonly int SessionTime = 10;
        internal static readonly int ActiveDuration = 5 * 60;
        internal static readonly int CaptureTime = 60;

        internal static readonly int SlideWidth = 355;
        internal static readonly int SlideHeight = 200;

        internal static readonly int CaptureWidth = 800;
        internal static readonly int CaptureHeight = 450;

        internal static readonly int MessageListCount = 30;
        internal static readonly int DelDataDay = 30;

        //Security
        internal static readonly string InitPassword = "youarefool";
        internal static readonly string Md5Key = "A!9HHhi%XjjYY4YP2@Nob009X";

        //Network protocols
        internal static readonly int Port = 9999;
        internal static readonly int Port1 = 20000;
        internal static readonly int ServerPort = 20039;


        internal static readonly string Se_ClientInfo = "RS21";
        internal static readonly string Se_Forbidden = "RS22";
        internal static readonly string Se_Confirm = "RS23";
        internal static readonly string Se_SetInfo = "RS24";
        internal static readonly string Se_Password = "RS25";
        internal static readonly string Se_End = "RS26";
        internal static readonly string Se_AutoVersion = "RS27";
        internal static readonly string Se_FileEnd = "RS28";

        internal static readonly string Se_MsgAudio = "RS31";
        internal static readonly string Se_MsgUSB = "RS32";
        internal static readonly string Se_MsgDownload = "RS33";
        internal static readonly string Se_MsgForbidden = "RS34";
        internal static readonly string Se_MsgDownLoading = "RS35";
        internal static readonly string Se_MsgDanger = "RS36";

        internal static readonly string Se_DataSlide = "RS11";
        internal static readonly string Se_DataCapture = "RS12";
        internal static readonly string Se_DataAudio = "RS13";
        internal static readonly string Se_DataProcess = "RS14";
        internal static readonly string Se_DataUSB = "RS15";
        internal static readonly string Se_DataURL = "RS16";
        internal static readonly string Se_DataDownload = "RS17";
        internal static readonly string Se_DataForbidden = "RS18";
        internal static readonly string Se_DataHuman = "RS19";

        internal static readonly string Se_VidCapture = "RS41";
        internal static readonly string Se_VidAudio = "RS42";
        internal static readonly string Se_VidCMD = "RS43";
        internal static readonly string Se_AudioData = "RS44";

        /// <summary>
        /// ////////////////////////////        /////////////////
        /// </summary>

        internal static readonly string Re_ClientInfo = "RR21";
        internal static readonly string Re_Forbidden = "RR22";
        internal static readonly string Re_Confirm = "RR23";
        internal static readonly string Re_SetInfo = "RR25";
        internal static readonly string Re_Password = "RR24";
        internal static readonly string Re_End = "RR26";

        internal static readonly string Re_MsgAudio = "RR31";
        internal static readonly string Re_MsgUSB = "RR32";
        internal static readonly string Re_MsgDownload = "RR33";
        internal static readonly string Re_MsgForbidden = "RR34";
        internal static readonly string Re_MsgDownLoading = "RR35";
        internal static readonly string Re_MsgDanger = "RR36";

        internal static readonly string Re_DataSlide = "RR11";
        internal static readonly string Re_DataCapture = "RR12";
        internal static readonly string Re_DataAudio = "RR13";
        internal static readonly string Re_DataProcess = "RR14";
        internal static readonly string Re_DataUSB = "RR15";
        internal static readonly string Re_DataURL = "RR16";
        internal static readonly string Re_DataDownload = "RR17";
        internal static readonly string Re_DataForbidden = "RR18";
        internal static readonly string Re_DataHuman = "RR19";


        internal static readonly string Re_VidCapture = "RR41";
        internal static readonly string Re_VidAudio = "RR42";
        internal static readonly string Re_VidCMD = "RR43";
        internal static readonly string Re_AudioData = "RR44";

        internal static readonly string Re_ServerProcessData = "RR51";
        internal static readonly string Re_ServerSlideData = "RR52";
        internal static readonly string Re_ServerCaptureData = "RR53";

        internal static readonly string Re_ServerTime = "RR00";
    }
}
