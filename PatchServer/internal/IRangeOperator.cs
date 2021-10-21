using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Monitor.PatchServer.Utils;

namespace Monitor.PatchServer.Internals
{
    internal interface IRangeOperator : ICollection<IPAddress>
    {
        bool Contains(IPAddressRange range);
    }
}
