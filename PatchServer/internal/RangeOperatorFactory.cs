using Monitor.PatchServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.PatchServer.Internals
{
    internal static class RangeOperatorFactory
    {
        public static IRangeOperator Create(IPAddressRange range)
        {
            return range.Begin.AddressFamily == AddressFamily.InterNetwork ?
                new IPv4RangeOperator(range) :
                new IPv6RangeOperator(range) as IRangeOperator;
        }
    }
}
