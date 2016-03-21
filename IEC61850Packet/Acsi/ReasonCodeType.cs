using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Acsi
{
    public enum ReasonCodeType:uint
    {
        Reserved=0x84020280,
        DataChange=0x84020240,
        QualityChange = 0x84020220,
        Update = 0x84020210,
        Integrity = 0x84020208,
        Call = 0x84020204

    }
}
