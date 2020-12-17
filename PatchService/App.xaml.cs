using PatchService.myEvent;
using PatchService.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PatchService
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
            _mutex = new Mutex(true, "PatchService", out _createdNew);
            //
            if (!_createdNew)  // already runnig
            {
                EnvironmentHelper.ShutDown();
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
