using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEC61850Packet.Asn1;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Mms.Acsi;


namespace IEC61850Packet.Mms
{
    public class MmsPacket : ApplicationPacket
    {
        //public MmsPduType PduType { get; set; }
        public MmsPdu Pdu { get; set; }
        public AcsiMapping Acsi { get; private set; }

        public MmsPacket(ByteArraySegment bas, Packet parent)
        {
            this.ParentPacket = parent;

            // Set PduType
            TLV pdu = new TLV(bas);
            this.header = bas;
            this.header.Length = pdu.Tag.RawBytes.Length + pdu.Length.RawBytes.Length;
            this.payloadPacketOrData = new PacketOrByteArraySegment();
            this.payloadPacketOrData.TheByteArraySegment = this.header.EncapsulatedBytes();

            MmsPduType pduId = (MmsPduType)BigEndianBitConverter.Big.ToInt8(pdu.Tag.RawBytes, 0);
            switch (pduId)
            {
                case MmsPduType.Unconfirmed:
                    Pdu = new UnconfirmedPdu(pdu.Value.Bytes, pdu);
                    var list = ((UnconfirmedPdu)Pdu).Service.InformationReport.ListOfAccessResult;
                    Acsi = new AcsiMapping(list);
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
