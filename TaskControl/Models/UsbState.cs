using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.TaskControl.Models
{
    public class UsbState
    {
        private string workPath { get; set; }
        public UsbState()
        {

        }
        public void SaveUsbHistory(byte[] buff)
        {
            //string strData = Encoding.UTF8.GetString(buff);
            //USBModel model = JsonConvert.DeserializeObject<USBModel>(strData);
            //workPath = Settings.Instance.Directories.WorkDirectory + @"\UsbHistory.dat";
            //List<USBModel> UsbList;
            //if (File.Exists(workPath))
            //{
            //    UsbList = JsonConvert.DeserializeObject<List<USBModel>>(File.ReadAllText(workPath));
            //    File.Delete(workPath);
            //}
            //else
            //{
            //    UsbList = new List<USBModel>();
            //}

            //UsbList.Add(model);
            //using (StreamWriter fs = File.CreateText(workPath))
            //{
            //    fs.Write(UsbList.ToString());
            //}

        }

        public List<USBModel> LoadUsbHistory()
        {
            workPath = Settings.Instance.Directories.WorkDirectory + @"\UsbHistory.dat";
            //List<USBModel> UsbList;// = new List<USBModel>;//JsonConvert.DeserializeObject<List<USBModel>>(File.ReadAllText(workPath));
            return null;
        }
    }
}
