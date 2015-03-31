using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet
{
    public struct CotpFileds
    {
        
        public static readonly int LengthLength = 1;
        public static readonly int PduTypeLength = 1;
        public static readonly int PduNumberLength = 1;
        public static readonly int HeaderLength = LengthLength + PduTypeLength + PduNumberLength;

    }
}
