using Monitor.TaskControl.Globals;
using Monitor.TaskControl.myEvents;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shell;



namespace Monitor.TaskControl.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            WorkDirectory.Text = Constants.BaseDirectory;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 50 });
        }

        private void WorkDirectory_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                WorkDirectory.Text = openFileDlg.SelectedPath;
            }
        }

        private void WorkDirectory1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                WorkDirectory.Text = openFileDlg.SelectedPath;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            EnvironmentHelper.ShutDown();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void PasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (WorkDirectory.Text == "")
            {
                CustomMsg message = new CustomMsg("Please select your work directory.");
                return;
            }

            if (PasswordTextBox.Password == "")
            {
                CustomMsg message = new CustomMsg("Please input the password.");

                //MessageBox.Show("Please input the password.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Events.RaiseOnRegister(e);
            Events.RaiseOnPassword(e);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            EnvironmentHelper.ShutDown();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
