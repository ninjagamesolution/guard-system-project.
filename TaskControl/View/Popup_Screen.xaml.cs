using System;
using System.Collections.Generic;
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
    /// Interaction logic for Popup_Screen.xaml
    /// </summary>
    public partial class Popup_Screen
    {
        public Popup_Screen()
        {
            InitializeComponent();
        }

        public void SetScale(double width, double height)
        {
            this.Width = width;
            this.Height = height + 70;
        }
    }
}
