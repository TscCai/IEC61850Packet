using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet
{
    public enum ReasonCodeType:ushort
    {
        Reserved=0x0280,
        DataChange=0x0240,
        QualityChange=0x0220,
        Update = 0x0210,
        Integrity=0x0208,
        Call=0x0204

    }
}
