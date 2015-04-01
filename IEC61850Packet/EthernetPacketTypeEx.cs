using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet
{
    public enum EthernetPacketTypeEx:ushort
    {
        Goose=0x88B8,
        Gse=0x88B9,
        Sv = 0x88BA,
        VLanTaggedFrame=0x8100  // same as PacketDotNet.EhternetPacketType.VLanTaggedFrame
    }
}
