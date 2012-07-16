using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.PatchServer.Globals
{
    enum Browser { Chrome = 1, Firefox, Edge, IE };
    internal static class Constants
    {

        internal static readonly string USBFileName = "USBHistory.lib";
        internal static readonly string FileCaptureName = "FileHistory.lib";

        //Network protocols
        internal static readonly int Port = 9999;
        internal static readonly int ClientUsbPort = 9994;
        internal static readonly int ServerUsbPort = 9993;
        internal static readonly int ClientFilePort = 9992;
        internal static readonly int ServerFilePort = 10000;
        internal static readonly int ClientAlarmPort = 9991;
        internal static readonly int ServerAlarmPort = 9000;
        internal static readonly int ServerAutoPort = 9995;
        internal static readonly int ClientAutoPort = 9996;
        internal static readonly int ClientZeroPort = 9998;
        internal static readonly int ServerZeroPort = 9997;


        internal static readonly string Se_AutoVersion = "RS27";
        internal static readonly string Se_FileEnd = "RS28";
        internal static readonly string Se_AutoFileInfo = "RS23";
        internal static readonly string Se_AutoFileEnd = "RS24";
        internal static readonly string Se_Version = "RS25";


        internal static readonly string Re_UsbDetect = "RR21";
        internal static readonly string Re_UsbEnd = "RR22";
        internal static readonly string Re_FileAlready = "RR23";
        internal static readonly string Re_FileName = "RR24";
        internal static readonly string Re_FileEnd = "RR25";
        internal static readonly string Re_End = "RR26";
        internal static readonly string Re_DrectoryName = "RR27";
        internal static readonly string Re_VersionSame = "same";
        internal static readonly string Re_VersionOther = "other";
    }
}
