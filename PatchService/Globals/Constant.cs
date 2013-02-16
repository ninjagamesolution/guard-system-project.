using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchService.Globals
{
    enum Browser { Chrome = 1, Firefox, Edge, IE };
    internal static class Constants
    {
        internal static readonly string TitleVersion = "Patch Server 1.7.8b";  

        internal static readonly string USBFileName = "USBHistory.lib";
        internal static readonly string FileCaptureName = "FileHistory.lib";

        internal static readonly string IPListFile = "IPList.lib";
        internal static readonly string InspectDirectory = "Inspect";

        //Network protocols
        internal static readonly int Port = 9999;



        internal static readonly int ServerFilePort = 19993;
        internal static readonly int ClientFilePort = 19992;
        internal static readonly int UsbPort = 19994;
        internal static readonly int ArpInfoPort = 19990;
        internal static readonly string ArpTablePath = "ArpTable.txt";
        internal static readonly string MysqlConString = "server=localhost;user=root;database=net_traf;port=3306;password=";
        internal static readonly int ArpDifferMin = 1440;
        
        internal static readonly int ServerAutoPort = 19995;
        internal static readonly int ClientAutoPort = 19996;
        internal static readonly int ServerZeroPort = 9997;
        internal static readonly int ClientZeroPort = 9998;
        internal static readonly int InspectPort = 20039;

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
