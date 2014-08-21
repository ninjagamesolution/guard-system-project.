using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;
using Monitor.TaskControl.Globals;
using Monitor.TaskControl.Utils;

//using DlibDotNet;
//using DlibDotNet.Extensions;
//using Dlib = DlibDotNet.Dlib;
using System.Runtime.InteropServices;
using Monitor.TaskControl.Logger;
using System.Threading;
//using Alturos.Yolo;
using System.Collections.Generic;

namespace Monitor.TaskControl.Models
{
    public class ScreenCaptures
    {
        public ScreenCaptures()
        {

        }
        public void SaveSlideImage(byte[] buffer, string ClientIP)
        {
            try
            {
                string strToday = Settings.Instance.Directories.TodayDirectory;
                if (!Directory.Exists(strToday))
                {
                    return;
                }
                string strPath = Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                strPath += @"\Slide";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                //MemoryStream ms = new MemoryStream(buffer);
                //Bitmap bitmap = new Bitmap(ms);
                DateTime localTime = DateTime.Now;
                string strFile = localTime.ToString("HH-mm-ss").Replace(":", "-") + "." + Constants.strImgExtension;
                
                string strFileName = Md5Crypto.OnImageFileNameChange(strPath, strFile);
                File.WriteAllBytes(strFileName, buffer);
            }
            catch(Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }
        }

        public void SaveCaputreImage(byte[] buffer, string ClientIP)
        {
            try
            {
                string strToday = Settings.Instance.Directories.TodayDirectory;
                if (!Directory.Exists(strToday))
                {
                    return;
                }
                string strPath = Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                strPath += @"\Capture";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                //MemoryStream ms = new MemoryStream(buffer);
                //Bitmap bitmap = new Bitmap(ms);
                DateTime localTime = DateTime.Now;
                string strFile = localTime.ToString("HH-mm-ss").Replace(":", "-") +  "." +Constants.strImgExtension;
                //string strFile = strPath + @"\" + localTime.Hour.ToString() + "-" + localTime.Minute.ToString() + "-" + localTime.Second.ToString() + ".jpg";
                //Md5Crypto.WriteImageFile(strPath, strFile, bitmap);
                
                
                
                string strFileName =  Md5Crypto.OnImageFileNameChange(strPath, strFile);
                File.WriteAllBytes(strFileName, buffer);

                
                
   
            }
            catch (Exception ex)
            {
                //File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture.jpg");
                //File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture_.jpg");

                CustomEx.DoExecption(Constants.exRepair, ex);
                Console.WriteLine("Exception ======> {0}", ex);
            }

        }

        //public void SaveHumanImage(byte[] buffer, string ClientIP)
        //{
        //    try
        //    {
        //        string strToday = Settings.Instance.Directories.TodayDirectory;
        //        if (!Directory.Exists(strToday))
        //        {
        //            return;
        //        }
        //        string strPath = Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP;
        //        if (!Directory.Exists(strPath))
        //            Directory.CreateDirectory(strPath);
        //        strPath += @"\Capture";
        //        if (!Directory.Exists(strPath))
        //            Directory.CreateDirectory(strPath);
        //        //MemoryStream ms = new MemoryStream(buffer);
        //        //Bitmap bitmap = new Bitmap(ms);
        //        DateTime localTime = DateTime.Now;
        //        string strFile = localTime.ToString("HH-mm-ss").Replace(":", "-") + "." + Constants.strImgExtension;
        //        //string strFile = strPath + @"\" + localTime.Hour.ToString() + "-" + localTime.Minute.ToString() + "-" + localTime.Second.ToString() + ".jpg";
        //        //Md5Crypto.WriteImageFile(strPath, strFile, bitmap);



        //        string strFileName = Md5Crypto.OnImageFileNameChange(strPath, strFile);
        //        File.WriteAllBytes(strFileName, buffer);

        //        byte[] sendBuffer = new byte[buffer.Length - 4];
        //        Array.Copy(buffer, 4, sendBuffer, 0, buffer.Length - 4);
        //        string screenFilePath = Path.Combine(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP, "human.lib");

        //        if (!File.Exists(screenFilePath))
        //            File.Create(screenFilePath);
        //        using (StreamWriter tw = File.AppendText(screenFilePath))
        //        {
        //            tw.WriteLine(strFileName);
        //            tw.Close();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture.jpg");
        //        File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture_.jpg");

        //        CustomEx.DoExecption(Constants.exRepair, ex);
        //        Console.WriteLine("Exception ======> {0}", ex);
        //    }

        //}

        public void ResizeImage(string fileName, string resizefile, int width, int height)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(fileName))
            {
                Log.Instance.DoLog("-------- Dlib.GetFrontalFaceDetector() BitMap Resize ====> ----", Log.LogType.Info);
                //Thread.Sleep(1000);
                new Bitmap(image, width, height).Save(resizefile);
            }
        }

        public void SaveSlideImage(byte[] buffer, string ClientIP, string fileName)
        {
            try
            {
                string strToday = Settings.Instance.Directories.TodayDirectory;
                if (!Directory.Exists(strToday))
                {
                    return;
                }
                string strPath = Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                strPath += @"\Slide";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                //MemoryStream ms = new MemoryStream(buffer);
                //Bitmap bitmap = new Bitmap(ms);
                //DateTime localTime = DateTime.Now;
                //string strFile = localTime.ToString("HH-mm-ss").Replace(":", "-") + "." + Constants.strImgExtension;

                string strFileName = Md5Crypto.OnImageFileNameChange(strPath, fileName + "." + Constants.strImgExtension);
                File.WriteAllBytes(strFileName, buffer);
            }
            catch (Exception ex)
            {
                CustomEx.DoExecption(Constants.exRepair, ex);
            }
        }

        public void SaveCaputreImage(byte[] buffer, string ClientIP, string fileName)
        {
            try
            {
                string strToday = Settings.Instance.Directories.TodayDirectory;
                if (!Directory.Exists(strToday))
                {
                    return;
                }
                string strPath = Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                strPath += @"\Capture";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                //MemoryStream ms = new MemoryStream(buffer);
                //Bitmap bitmap = new Bitmap(ms);
                //DateTime localTime = DateTime.Now;
                //string strFile = localTime.ToString("HH-mm-ss").Replace(":", "-") + "." + Constants.strImgExtension;
                //string strFile = strPath + @"\" + localTime.Hour.ToString() + "-" + localTime.Minute.ToString() + "-" + localTime.Second.ToString() + ".jpg";
                //Md5Crypto.WriteImageFile(strPath, strFile, bitmap);



                string strFileName = Md5Crypto.OnImageFileNameChange(strPath, fileName + "." + Constants.strImgExtension);
                File.WriteAllBytes(strFileName, buffer);




            }
            catch (Exception ex)
            {
                //File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture.jpg");
                //File.Delete(Settings.Instance.Directories.TodayDirectory + @"\" + ClientIP + @"\Capture" + "\\capture_.jpg");

                CustomEx.DoExecption(Constants.exRepair, ex);
                Console.WriteLine("Exception ======> {0}", ex);
            }

        }

    }
}
