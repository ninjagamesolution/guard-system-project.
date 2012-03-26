using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitor.TaskControl.Logger;
using Monitor.TaskControl.Globals;

namespace Monitor.TaskControl.Utils
{
    internal static class CustomEx
    {
        public static void DoExecption(Byte Type, Exception ex)
        {
            if (Type == Constants.exExit)
            {
                Log.Instance.DoLog(string.Format("Will program restart because of Unhandled Exception.\r\nException: {0}\r\n", ex.Message), Log.LogType.Error);
                EnvironmentHelper.Restart(true);
            }
            else if (Type == Constants.exRepair)
            {
                Log.Instance.DoLog(string.Format("Will Repair because of Unhandled Exception.\r\nException: {0}\r\n", ex.Message), Log.LogType.Error);
            }
            else if (Type == Constants.exResume)
            {
                Log.Instance.DoLog(string.Format("Unhandled Exception.\r\nException: {0}\r\n", ex.Message), Log.LogType.Error);
            }
        }
    }
}
