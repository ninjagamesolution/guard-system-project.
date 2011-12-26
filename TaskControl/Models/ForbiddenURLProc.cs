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
    internal static class ForbiddenURLProc
    {
        private static List<string> strForbiddenURLList = new List<string>();

        public static void checkForbiddenURLFile()
        {
            try
            {
                string strFilePath = Settings.Instance.RegValue.BaseDirectory;
                Settings.Instance.ForbiddenURLList.Clear();
                if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenURLFileName))
                {
                    strForbiddenURLList.Clear();
                    for (int i = 0; i < Constants.dangerURLArray.Count(); i++)
                    {
                        strForbiddenURLList.Add(Constants.dangerURLArray[i]);
                    }
                    Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenURLFileName, strForbiddenURLList);
                }
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
        }

        public static void LoadForbiddenURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenURLFileName;
            Settings.Instance.ForbiddenURLList.Clear();
            strForbiddenURLList.Clear();
            strForbiddenURLList = Md5Crypto.ReadCryptoFile(strFilePath);
            foreach (string line in strForbiddenURLList)
            {
                Settings.Instance.ForbiddenURLList.Add(line);
            }
        }

        public static void AddForbiddenURLList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenURLFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.ForbiddenURLFileName);
                myFile.Close();
            }

            strForbiddenURLList.Clear();
            foreach (var item in Settings.Instance.ForbiddenURLList)
            {
                strForbiddenURLList.Add(item);
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenURLFileName, strForbiddenURLList);
        }

        public static void DeleteForbiddenURLList(string PName)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenURLFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.ForbiddenURLFileName);
                myFile.Close();

            }

            strForbiddenURLList.Clear();
            foreach (var item in Settings.Instance.ForbiddenURLList)
            {
                if (item != PName)
                {
                    strForbiddenURLList.Add(item);
                }
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenURLFileName, strForbiddenURLList);
        }
    }
}
