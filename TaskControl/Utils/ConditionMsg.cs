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
    public class ConditionMsg
    {
        ConditionMsgWindow newMsg;
        public ConditionMsg()
        {

        }

        public ConditionMsg(string Msg)
        {
            if (Msg.Length > 30)
            {
                newMsg = new ConditionMsgWindow(Msg, Msg.Length);
            }
            else
            {
                newMsg = new ConditionMsgWindow(Msg);
            }

            newMsg.ShowDialog();

        }

    }
}
