using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Models
{
    internal static class AllowURLProc
    {
        private static List<string> strAllowURLList = new List<string>();
        public static void checkAllowURLFile()
        {
            try
            {
                string strFilePath = Settings.Instance.RegValue.BaseDirectory;
                Settings.Instance.AllowURLList.Clear();
                if (!File.Exists(strFilePath + "\\" + Constants.AllowURLFileName))
                {
                    strAllowURLList.Clear();
                    for (int i = 0; i < Constants.allowURLArray.Count(); i++)
                    {
                        strAllowURLList.Add(Constants.allowURLArray[i]);
                    }
                    Md5Crypto.WriteCryptoFile(strFilePath, Constants.AllowURLFileName, strAllowURLList);
                }
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
        }

        public static void LoadAllowURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.AllowURLFileName;
            Settings.Instance.AllowURLList.Clear();
            strAllowURLList.Clear();
            strAllowURLList = Md5Crypto.ReadCryptoFile(strFilePath);
            foreach (string line in strAllowURLList)
            {
                Settings.Instance.AllowURLList.Add(line);
            }
        }

        public static void AddAllowURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.AllowURLFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.AllowURLFileName);
                myFile.Close();
            }

            strAllowURLList.Clear();
            foreach (var item in Settings.Instance.AllowURLList)
            {
                strAllowURLList.Add(item);
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.AllowURLFileName, strAllowURLList);
        }

        public static void DeleteAllowURLList(string PName)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.AllowURLFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.AllowURLFileName);
                myFile.Close();

            }

            strAllowURLList.Clear();
            foreach (var item in Settings.Instance.AllowURLList)
            {
                if (item != PName)
                {
                    strAllowURLList.Add(item);
                }
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.AllowURLFileName, strAllowURLList);
        }
    }
}
