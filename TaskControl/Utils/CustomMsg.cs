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
using Monitor.TaskControl.View;

namespace Monitor.TaskControl.Utils
{
    public class CustomMsg
    {
        CustomMsgWindow newMsg;
        public CustomMsg()
        {

        }

        public CustomMsg(string Msg)
        {
            if (Msg.Length > 28)
            {
                newMsg = new CustomMsgWindow(Msg, Msg.Length);
            }
            else
            {
                newMsg = new CustomMsgWindow(Msg);
            }

            newMsg.ShowDialog();

        }

    }
}
