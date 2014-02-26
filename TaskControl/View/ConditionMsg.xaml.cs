using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shell;

namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for ConditionMsg.xaml
    /// </summary>
    public partial class ConditionMsgWindow : Window
    {
        private bool _isAvailable = true;
        public static bool bDelete;
        public bool IsAvailable
        {
            get { return _isAvailable; }

            set
            {
                _isAvailable = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        //        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public ConditionMsgWindow()
        {
            InitializeComponent();
        }
        public ConditionMsgWindow(string Msg)
        {
            InitializeComponent();
            this.lbl_Msg.Content = Msg;
            bDelete = false;
        }
        public ConditionMsgWindow(string Msg, int CountFont)
        {
            InitializeComponent();
            this.Width = this.Width + (CountFont - 30) * 8;
            this.btn_Msg.Width += (CountFont - 30) * 8;
            this.lbl_Msg.Width += (CountFont - 30) * 8;
            this.lbl_Msg.Content = Msg;

        }
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            bDelete = false;
            Close();
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            bDelete = true;
            Close();
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            bDelete = false;
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 0, UseAeroCaptionButtons = false });
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Settings.Instance.RememberCredentials = RememberCheckBox.IsChecked ?? false;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            //Writing for set UI

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
