using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Monitor.TaskControl.myEvents
{
    public static class Events
    {


        public delegate void OnStartUpDelegate(StartupEventArgs e);
        public static event OnStartUpDelegate OnStartUp;

        public delegate void OnExitDelegate(ExitEventArgs e);
        public static event OnExitDelegate OnExit;

        public delegate void OnRegisterDelegate(EventArgs e);
        public static event OnRegisterDelegate OnRegister;

        public delegate void OnConnectionFinishedDelegate(EventArgs args);
        public static event OnConnectionFinishedDelegate OnConnectedFinished;

        public delegate void OnMainProcDelegate();
        public static event OnMainProcDelegate OnMainProc;

        public delegate void OnRecvDataDelegate(byte[] buff, int len, string ClientIP);
        public static event OnRecvDataDelegate OnReceiveData;

        public delegate void OnPasswordDelgate(EventArgs e);
        public static event OnPasswordDelgate OnPassword;

        public delegate void OnUSBDelegate();
        public static event OnUSBDelegate OnUSBData;


        static Events()
        {
            EventHandlers.Initialize();
        }

        public static void RaiseOnReveiveData(byte[] buff, int len, string ClientIP)
        {
            if (OnReceiveData != null)
            {
                OnReceiveData(buff, len, ClientIP);
            }
        }

        public static void RaiseOnUSBData()
        {
            if (OnUSBData != null)
                OnUSBData();
        }

        public static void RaiseOnMainProc()
        {
            if (OnMainProc != null)
                OnMainProc();
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
        public static void RaiseOnRegister(EventArgs args)
        {
            if (OnRegister != null)
            {
                OnRegister(args);
            }
        }

        public static void RaiseOnConnectedFinished(EventArgs args)
        {
            if (OnConnectedFinished != null)
            {
                OnConnectedFinished(args);
            }
        }

        public static void RaiseOnPassword(EventArgs e)
        {
            if (OnPassword != null)
            {
                OnPassword(e);
            }
        }

    }
}
