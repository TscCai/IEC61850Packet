using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet
{
    public struct TpktFileds
    {
        public static readonly int TpktHeaderLength = 4 ;    // Include Version, reserved, length

        public static readonly int TpktHeaderVersionLength = 1;
        public static readonly int TpktHeaderReservedLength = 1;

        public static readonly int TpktLengthLength = 2;

        public bool LeadWithSegment;
        public int LeadingSegmentLength;

    }
}
