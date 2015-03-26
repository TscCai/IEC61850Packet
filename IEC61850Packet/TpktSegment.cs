using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet.Utils;

namespace IEC61850Packet
{
    public class TpktSegment
    {
        public ByteArraySegment Segment { get; set; }
        public bool StartWithHeader { get; set; }

        public TpktSegment(ByteArraySegment seg, bool isHeader)
        {
            Segment = seg;
            StartWithHeader = isHeader;
        }
        public TpktSegment(byte[] seg, bool isHeader) : this(new ByteArraySegment(seg), isHeader) { }
        public TpktSegment(ByteArraySegment seg)
        {
            Segment = seg;
        }
        public TpktSegment(byte[] seg)
            : this(new ByteArraySegment(seg))
        {

        }
    }
}
