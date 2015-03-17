using IEC61850Packet.Asn1;
using PacketDotNet;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Mms
{
    public class MmsPacket : ApplicationPacket
    {
        //public MmsPduType PduType { get; set; }
        public MmsPdu Pdu { get; set; }

        public MmsPacket(ByteArraySegment bas, Packet parent)
        {
            this.ParentPacket = parent;
            
            // Set PduType
            TLV packet = new TLV(bas);

            MmsPduType pduId = (MmsPduType)BigEndianBitConverter.Big.ToInt8(packet.Tag.RawBytes,0);
            switch (pduId)
            {
                case MmsPduType.Unconfirmed:
                   Pdu = new UnconfirmedPdu(packet.Value.Bytes,packet);
                    break;
                case MmsPduType.ConfirmedRequest:
                    break;
                case MmsPduType.ConfirmedResponse:
                    break;
                default:
                    break;
            }
            this.payloadPacketOrData = new PacketOrByteArraySegment();
            this.payloadPacketOrData.TheByteArraySegment = bas;

        }
    }
}
