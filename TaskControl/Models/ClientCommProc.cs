using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Models
{
    public class ClientCommProc
    {
        private static ClientCommProc _instance;
        public ScreenCaptures ScrCapture = new ScreenCaptures();
        public SaveProcessInfo PrcInfo = new SaveProcessInfo();
        public static ClientCommProc Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientCommProc();
                }
                return _instance;
            }
        }


        public void RecDataAnalysis(byte[] buf, int length, string ClientIP)
        {

            string prefix = Encoding.UTF8.GetString(buf, 0, 4);
            byte[] temp = new byte[length - 4];
            Array.Copy(buf, 4, temp, 0, length - 4);

            if (prefix == Constants.Re_ServerSlideData)
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
    }
}
