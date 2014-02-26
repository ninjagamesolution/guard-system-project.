using Monitor.TaskControl.myEvents;
using Monitor.TaskControl.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Monitor.TaskControl.Logger;
using System.IO;
using Monitor.TaskControl.Globals;
using Microsoft.Win32;

namespace Monitor.TaskControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;
        private bool _createdNew;

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.Sleep(300);
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            //  Double Runnig check 
            _mutex = new Mutex(true, "Monitor.TaskControl", out _createdNew);
            //
            if (!_createdNew)  // already runnig
            {
                EnvironmentHelper.ShutDown();
            }
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                var rk = key.OpenSubKey(Constants.RegPath);
                string BaseDirectory = (string)rk.GetValue("BaseDirectory");
                if (BaseDirectory == "D:\\Work")
                {
                    string[] fileEntries = Directory.GetFiles("D:\\Work\\", "Contents.lib", SearchOption.AllDirectories);
                    if (fileEntries.Count() > 0)
                    {
                        Directory.Move("D:\\Work", Constants.BaseDirectory);

                        key = key.CreateSubKey(Constants.RegPath);
                        key.SetValue("BaseDirectory", Constants.BaseDirectory);
                        key.Close();
                    }
                }
            }
            catch
            {

            }
            Events.RaiseOnStartUp(e); //
            base.OnStartup(e);
        }

        private void OnProcessExit(object sender, EventArgs eventArgs)
        {
            if (_mutex != null && _createdNew)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
