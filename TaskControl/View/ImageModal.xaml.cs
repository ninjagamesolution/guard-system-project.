using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for ImageModal.xaml
    /// </summary>
    public partial class ImageModal : Window
    {
        public ImageModal()
        {
            InitializeComponent();
        }

        public void OnImageShow(string filePath)
        {
            try
            {
                String[] spearator = { "\\" };
                String[] strArray = filePath.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                string strRealFileName = Md5Crypto.OnGetRealName(strArray[strArray.Count() - 1]).Replace("-", ":");
            
                this.Title = strRealFileName;
                byte[] temp = Md5Crypto.OnReadImgFile(filePath);
            
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                MemoryStream ms1 = new MemoryStream(temp);
                imageSource.StreamSource = ms1;
                imageSource.EndInit();
                imgModal.Source = imageSource;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
