using PatchService.Globals;
using PatchService.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PatchService.myEvent
{
    public static class EventHandler
    {
        static EventHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            Events.OnStartUp += OnStartUp;
            Events.OnExit += OnExit;

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
            Windows.MainWindow = new MainWindow();
            Windows.MainWindow.Show();
        }

        private static void OnExit(ExitEventArgs exitEventArgs)
        {
            //Windows.MainWindow.DeleteTryIcon();
            //// Cleanup

            //           Settings.Save();

            //Log.Instance.DoLog("TaskView has exited.\r\n");
        }

    }
}
