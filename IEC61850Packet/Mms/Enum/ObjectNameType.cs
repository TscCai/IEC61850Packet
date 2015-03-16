using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Mms.Enum
{
    public enum ObjectNameType:byte
    {
        Vmd_Specific=0x80,
        Domain_Specific=0x81,
        AA_Specific=0x82
    }
}
