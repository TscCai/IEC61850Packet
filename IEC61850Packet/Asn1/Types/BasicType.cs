using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;

namespace IEC61850Packet.Asn1.Types
{
    public abstract class BasicType
    {
        public virtual ByteArraySegment Bytes { get; set; }

        protected byte[] Identifier { get; set; }


    }
}
