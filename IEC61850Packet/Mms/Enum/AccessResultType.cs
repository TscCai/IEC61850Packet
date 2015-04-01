using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Mms.Enum
{
    public enum AccessResultType:byte
    {
        Failiure = 0x80,
        Success
    }
}
