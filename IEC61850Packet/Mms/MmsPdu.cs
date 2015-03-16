using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public abstract class MmsPdu
    {
        public virtual ByteArraySegment Bytes { get; set; }
        protected virtual byte[] Identifier { get; set; }
    }
}
