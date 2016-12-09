using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Monitor.PatchServer.myEvents
{
    public static class Events
    {
        public delegate void OnStartUpDelegate(StartupEventArgs e);
        public static event OnStartUpDelegate OnStartUp;

        public delegate void OnExitDelegate(ExitEventArgs e);
        public static event OnExitDelegate OnExit;
        static Events()
        {
            EventHandlers.Initialize();
        }

        public static void RaiseOnStartUp(StartupEventArgs e)
        {
            if (OnStartUp != null)
            {
                OnStartUp(e);
            }
        }
        public static void RaiseOnExit(ExitEventArgs e)
        {
            if (OnExit != null)
            {
                OnExit(e);
            }
        }
    }
}
