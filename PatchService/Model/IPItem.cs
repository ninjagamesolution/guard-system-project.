using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PatchService.Model
{
    public class IPItem : INotifyPropertyChanged
    {
        private bool _IsChecked = false;
        public bool IsChecked { get { return _IsChecked; } set { _IsChecked = value; OnChanged("IsChecked"); } }
        public string IPAddress { get; set; }
        public string Status { get; set; }
        public string During { get; set; }
        public string IconImage { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
