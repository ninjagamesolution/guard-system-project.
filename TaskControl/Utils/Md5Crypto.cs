using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Utils
{
    public static class Md5Crypto
    {
        public static void WriteCryptoFile(string dbFilePath, string dbFileName, List<string> strTempList)
        {
            if (!Directory.Exists(dbFilePath))
            {
                Directory.CreateDirectory(dbFilePath);
            }
            dbFilePath = Path.Combine(dbFilePath, dbFileName);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.Md5Key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        using (TextWriter tw = new StreamWriter(dbFilePath))
                        {
                            foreach (var list in strTempList)
                            {
                                if (list == "")
                                {
                                    continue;
                                }
                                try
                                {
                                    byte[] textBytes = UTF8Encoding.UTF8.GetBytes(list);
                                    byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                                    tw.WriteLine(Convert.ToBase64String(bytes, 0, bytes.Length));
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WriteCryptoFileAppend(string dbFilePath, string dbFileName, string strData)
        {
            try
            {
                if (!Directory.Exists(dbFilePath))
                {
                    Directory.CreateDirectory(dbFilePath);
                }
                if (!File.Exists(dbFilePath + "\\" + dbFileName))
                {
                    var dbFile = File.Create(dbFilePath + "\\" + dbFileName);
                    dbFile.Close();
                }
            }
            catch
            {

            }
            try
            {
                dbFilePath = Path.Combine(dbFilePath, dbFileName);
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.Md5Key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;

                        


                        using (var transform = tdes.CreateEncryptor())
                        {
                            try
                            {
                                using (StreamWriter sw = File.AppendText(dbFilePath))
                                {
                                    try
                                    {
                                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(strData);
                                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                                        sw.WriteLine(Convert.ToBase64String(bytes, 0, bytes.Length));
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("****** Error : " + ex.ToString());
                            }
                        }
                        
                    }
                }
            }
            catch
            {

            }
        }


        public static void WriteCryptoFileAppendList(string dbFilePath, string dbFileName, List<string> lstData)
        {
            try
            {
                if (!Directory.Exists(dbFilePath))
                {
                    Directory.CreateDirectory(dbFilePath);
                }
                if (!File.Exists(dbFilePath + "\\" + dbFileName))
                {
                    var dbFile = File.Create(dbFilePath + "\\" + dbFileName);
                    dbFile.Close();
                }
            }
            catch
            {

            }
            try
            {
                dbFilePath = Path.Combine(dbFilePath, dbFileName);
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.Md5Key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;




                        using (var transform = tdes.CreateEncryptor())
                        {
                            try
                            {
                                using (StreamWriter sw = File.AppendText(dbFilePath))
                                {
                                    try
                                    {
                                        foreach(string strData in lstData)
                                        {
                                            byte[] textBytes = UTF8Encoding.UTF8.GetBytes(strData);
                                            byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                                            sw.WriteLine(Convert.ToBase64String(bytes, 0, bytes.Length));
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("****** Error : " + ex.ToString());
                            }
                        }

                    }
                }
            }
            catch
            {

            }
        }


        public static void WriteFileReplace(string dbFilePath, string dbFileName, string strData)
        {
            try
            {
                if (!Directory.Exists(dbFilePath))
                {
                    Directory.CreateDirectory(dbFilePath);
                }
                if (!File.Exists(dbFilePath + "\\" + dbFileName))
                {
                    File.Create(dbFilePath + "\\" + dbFileName);
                }
            }
            catch
            {

            }
            try
            {
                dbFilePath = Path.Combine(dbFilePath, dbFileName);
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.Md5Key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;

                        using (var transform = tdes.CreateEncryptor())
                        {
                            using (TextWriter tw = new StreamWriter(dbFilePath))
                            {
                                try
                                {
                                    byte[] textBytes = UTF8Encoding.UTF8.GetBytes(strData);
                                    byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                                    tw.WriteLine(Convert.ToBase64String(bytes, 0, bytes.Length));
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }


        public static List<string> ReadCryptoFile(string dbFilePath)
        {
            List<string> strTempList = new List<string>();
            if (!File.Exists(dbFilePath))
            {
                //File.Create(dbFilePath);
                return strTempList;
            }

            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.Md5Key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;
                    string line = "";

                    using (var transform = tdes.CreateDecryptor())
                    {
                        using (var reader = new System.IO.StreamReader(dbFilePath))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line == "")
                                {
                                    continue;
                                }
                                try
                                {
                                    byte[] cipherBytes = Convert.FromBase64String(line);
                                    byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                                    strTempList.Add(UTF8Encoding.UTF8.GetString(bytes));
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
            return strTempList;
        }

        public static string OnImageFileNameChange(string strPath, string strFileName)
        {
            string strFileNameTemp = "";
            for (int i = 0; i < strFileName.Length; i++)
            {
                if ((int)strFileName[i] - '0' >= 0 && (int)strFileName[i] - '0' < 10)
                {
                    strFileNameTemp = strFileNameTemp.Insert(i, Constants.strArray[(int)strFileName[i] - '0']);
                }
                else
                {
                    strFileNameTemp = strFileNameTemp.Insert(i, strFileName[i].ToString());
                }
            }
            strPath = strPath + "\\" + strFileNameTemp;
            return strPath;
        }

        public static string OnGetRealName(string strFileName)
        {
            string strFileNameTemp = "";
            for (int i = 0; i < strFileName.Length; i++)
            {
                int nIndex = Constants.strArray.ToList().FindIndex(a => a.Contains(strFileName[i].ToString()));

                if (nIndex != -1)
                {
                    strFileNameTemp = strFileNameTemp.Insert(i, nIndex.ToString());
                }
                else if (strFileName[i].ToString() == ".")
                {
                    break;
                }
                else
                {
                    strFileNameTemp = strFileNameTemp.Insert(i, strFileName[i].ToString());
                }
            }
            return strFileNameTemp;
        }

        public static byte[] OnReadImgFile(string strFilePath)
        {
            if (!File.Exists(strFilePath))
            {
                return Encoding.ASCII.GetBytes("");
            }
            byte[] byteData = File.ReadAllBytes(strFilePath);
            try
            {
                if (strFilePath.Contains("psl"))
                {
                    byte[] temp = new byte[byteData.Length - 4];
                    Array.Copy(byteData, 4, temp, 0, byteData.Length - 4);
                    return temp;
                }
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exResume, ex);
            }
            return byteData;
        }

        //public static void WriteImageFile(string strPath, string strFileName, Bitmap imgTemp)
        //{
        //    string strFileNameTemp = "";
        //    for (int i = 0; i < strFileName.Length; i++)
        //    {
        //        int dd = (int)strFileName[i] - '0';
        //        if ((int)strFileName[i] - '0' >= 0 && (int)strFileName[i] - '0' < 10)
        //        {
        //            strFileNameTemp = strFileNameTemp.Insert(i, Constants.strArray[(int)strFileName[i] - '0']);
        //        }
        //        else
        //        {
        //            strFileNameTemp = strFileNameTemp.Insert(i, strFileName[i].ToString());
        //        }
        //    }
        //    strPath = strPath + "\\" + strFileNameTemp;
        //    ImageConverter converter = new ImageConverter();
        //    byte[] bytes = (byte[])converter.ConvertTo(imgTemp, typeof(byte[]));
        //    bytes[0] = 214;
        //    File.WriteAllBytes(strPath, bytes);
        //}

        //public static Dictionary<string, Image> ReadImageFile(string strPath)
        //{
        //    Dictionary<string, Image> imgList = new Dictionary<string, Image>();
        //    //strPath = "D:\\Work\\2021-1-6\\Slide";
        //    if (strPath.Substring(strPath.Length - 1) != "\\")
        //    {
        //        strPath += "\\";
        //    }

        //    string[] fileNames = Directory.GetFiles(strPath, "*.*").Select(Path.GetFileName).ToArray();
        //    try
        //    {
        //        foreach (var fileName in fileNames)
        //        {
        //            using (FileStream fsSource = new FileStream(strPath + fileName, FileMode.Open, FileAccess.Read))
        //            {
        //                // Read the source file into a byte array.
        //                byte[] bytes = new byte[fsSource.Length];
        //                int numBytesToRead = (int)fsSource.Length;
        //                int numBytesRead = 0;
        //                while (numBytesToRead > 0)
        //                {
        //                    // Read may return anything from 0 to numBytesToRead.
        //                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

        //                    // Break when the end of the file is reached.
        //                    if (n == 0)
        //                        break;

        //                    numBytesRead += n;
        //                    numBytesToRead -= n;
        //                }
        //                numBytesToRead = bytes.Length;
        //                if (bytes[0] == 214)
        //                {
        //                    bytes[0] = 137;
        //                }

        //                string strFileNameTemp = "";
        //                for (int i = 0; i < fileName.Length; i++)
        //                {
        //                    int nIndex = Constants.strArray.ToList().FindIndex(a => a.Contains(fileName[i].ToString()));

        //                    if (nIndex != -1)
        //                    {
        //                        strFileNameTemp = strFileNameTemp.Insert(i, nIndex.ToString());
        //                    }
        //                    else if (fileName[i].ToString() == ".")
        //                    {
        //                        strFileNameTemp += ".jpg";
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        strFileNameTemp = strFileNameTemp.Insert(i, fileName[i].ToString());
        //                    }
        //                }

        //                using (MemoryStream ms = new MemoryStream(bytes))
        //                {
        //                    imgList.Add(strFileNameTemp, Image.FromStream(ms));
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return imgList;
        //}
    }
}
