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
            TLV pdu = new TLV(bas);

            MmsPduType pduId = (MmsPduType)BigEndianBitConverter.Big.ToInt8(pdu.Tag.RawBytes,0);
            switch (pduId)
            {
                case MmsPduType.Unconfirmed:
                   Pdu = new UnconfirmedPdu(pdu.Value.Bytes,pdu);
                    break;
                case MmsPduType.ConfirmedRequest:
                    break;
                case MmsPduType.ConfirmedResponse:
                    break;
                default:
                    break;
            }

        }
    }
}
