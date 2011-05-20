using Monitor.TaskControl.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows;
using Monitor.TaskControl.Logger;
using Monitor.TaskControl.View;
using Monitor.TaskControl.Models;
using System.Net.Sockets;
using Monitor.TaskControl.Communication;

namespace Monitor.TaskControl.myEvents
{
    class EventHandlers
    {
        static EventHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            Events.OnStartUp += OnStartUp;
            Events.OnExit += OnExit;
            Events.OnRegister += OnRegister;
            Events.OnReceiveData += OnReceiveData;
            Events.OnPassword += OnPassword;
            Events.OnMainProc += OnMainProc;

        }

        public static void Initialize()
        {
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = (Exception)args.ExceptionObject;

            if (args.IsTerminating)
            {
                //                Settings.Save();
            }

            //            Log.Instance.DoLog(string.Format("Unhandled Exception.\r\nException: {0}\r\n", exception), Log.LogType.Error);

        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //foreach (var file in Directory.GetFiles(Settings.Instance.Directories.DependenciesDirectory))
            //{
            //    if (Path.GetFileNameWithoutExtension(file) == (new AssemblyName(args.Name).Name))
            //    {
            //        return Assembly.LoadFrom(file);
            //    }
            //}

            return null;
        }

        private static void OnStartUp(StartupEventArgs startupEventArgs)
        {
            Log.Instance.DoLog("-------- OnStartup ----", Log.LogType.Info);
            //Settings.Instance.Directories.CreateDirectories();
            if (!Settings.Instance.RegValue.ExistsGetValue())
            {

                Windows.LoginWindow = new LoginWindow();
                Windows.LoginWindow.Show();
                //Settings.Instance.Directories.CreateDirectories();
                //Communications.StartUpSocket();
            }
            else
            {
                Settings.Instance.Directories.CreateDirectories();
                Windows.MainWindow = new MainWindow();
                Windows.MainWindow.Show();
                Communications.StartUpSocket();
            }
        }

        private static void OnReceiveData(byte[] buff, int len, string ClientIP)
        {
            CommProc.Instance.RecDataAnalysis(buff, len, ClientIP);
        }


        private static void OnPassword(EventArgs exitEventArgs)
        {
            Settings.Instance.RegValue.Password = Windows.LoginWindow.PasswordTextBox.Password;
            Settings.Instance.RegValue.WriteValue();
            //Settings.Instance.RegValue.BaseDirectory = Constants.BaseDirectory;
            Settings.Instance.RegValue.WriteValue();
            Settings.Instance.Directories.CreateDirectories();
            if (Windows.LoginWindow != null)
            {
                Windows.LoginWindow.Hide();
            }

            Windows.MainWindow = new MainWindow();
            Windows.MainWindow.Show();
        }

        private static void OnExit(ExitEventArgs exitEventArgs)
        {
            Log.Instance.DoLog("TaskView is exiting.");
            Windows.MainWindow.DeleteTryIcon();
            //// Cleanup

            //           Settings.Save();

            Log.Instance.DoLog("TaskView has exited.\r\n");
        }

        private static void OnRegister(EventArgs exitEventArgs)
        {
            Settings.Instance.RegValue.BaseDirectory = Windows.LoginWindow.WorkDirectory.Text;
            Settings.Instance.RegValue.WriteValue();
            Settings.Instance.Directories.CreateDirectories();
            Communications.StartUpSocket();
            //Communications.StartUpSocket();
            //Settings.Instance.RegValue.ServerIP = Windows.LoginWindow.ServerIP.Text;
            //Settings.Instance.RegValue.UserName = Windows.LoginWindow.UserName.Text;
            //Settings.Instance.RegValue.Company = Windows.LoginWindow.Company.Text;
        }

        private static void OnMainProc()
        {

            MainProc.Instance.AllProcessStart();
        }
    }
}
