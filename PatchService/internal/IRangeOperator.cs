using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using PatchService.Utils;

namespace PatchService.Internals
{
    internal interface IRangeOperator : ICollection<IPAddress>
    {
        bool Contains(IPAddressRange range);
    }
}
