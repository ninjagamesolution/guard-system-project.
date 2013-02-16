using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;

namespace Monitor.TaskControl.Models
{
    internal static class ForbiddenProc
    {
        private static List<string> strForbiddenList = new List<string>();
        public static void checkForbiddenFile()
        {
            try
            {
                string strFilePath = Settings.Instance.RegValue.BaseDirectory;
                Settings.Instance.Forbiddenprocess_list.Clear();
                if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenFileName))
                {
                    strForbiddenList.Clear();
                    for (int i = 0; i < Constants.forbiddenProcessArray.Count(); i++)
                    {
                        strForbiddenList.Add(Constants.forbiddenProcessArray[i]);
                    }
                    Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenFileName, strForbiddenList);
                }
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
        }

        public static void LoadForbiddenProcessList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory + "\\" + Constants.ForbiddenFileName;
            Settings.Instance.Forbiddenprocess_list.Clear();
            strForbiddenList.Clear();
            strForbiddenList = Md5Crypto.ReadCryptoFile(strFilePath);
            foreach (string line in strForbiddenList)
            {
                Settings.Instance.Forbiddenprocess_list.Add(line.Split(',')[0], line.Split(',')[1]);
            }
        }

        public static void AddForbiddenList()
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.ForbiddenFileName);
                myFile.Close();
            }

            strForbiddenList.Clear();
            foreach (var item in Settings.Instance.Forbiddenprocess_list)
            {
                strForbiddenList.Add(item.Key + "," + item.Value);
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenFileName, strForbiddenList);
        }

        public static void DeleteForbiddenList(string PName)
        {
            string strFilePath = Settings.Instance.RegValue.BaseDirectory;
            if (!File.Exists(strFilePath + "\\" + Constants.ForbiddenFileName))
            {
                var myFile = File.Create(strFilePath + "\\" + Constants.ForbiddenFileName);
                myFile.Close();

            }

            strForbiddenList.Clear();
            foreach (var item in Settings.Instance.Forbiddenprocess_list)
            {
                if (item.Key != PName)
                {
                    strForbiddenList.Add(item.Key + "," + item.Value);
                }
            }
            Md5Crypto.WriteCryptoFile(strFilePath, Constants.ForbiddenFileName, strForbiddenList);
        }
    }
}
