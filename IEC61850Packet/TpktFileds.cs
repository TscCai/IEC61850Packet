using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet
{
    public struct TpktFileds
    {
        public static readonly int VersionLength = 1;
        public static readonly int ReservedLength = 1;
        public static readonly int LengthLength = 2;
        public static readonly int HeaderLength = VersionLength+ReservedLength+LengthLength;    // Include Version, reserved, length
        public static readonly int MaxLength = 0x0404;

        public bool LeadWithSegment;
        public int LeadingSegmentLength;

    }
}
