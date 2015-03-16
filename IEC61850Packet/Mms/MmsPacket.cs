using PacketDotNet;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public class MmsPacket : ApplicationPacket
    {
        public string PduType { get; set; }
        public MmsPdu Pdu { get; set; }

        

        public MmsPacket(ByteArraySegment bas, Packet parent)
        {
            this.ParentPacket = parent;

        }
    }
}
