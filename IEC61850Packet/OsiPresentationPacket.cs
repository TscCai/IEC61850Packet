using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.SimpleTypes;
using IEC61850Packet.Mms;
using PacketDotNet;

namespace IEC61850Packet
{
    public class OsiPresentationPacket : Packet
    {
        public enum ContextIdentifier : byte { Mms = 0x03 };
        public ContextIdentifier PresentationContextIdentifier { get; private set; }
        public byte[] PresentationDataValues { get { return this.PayloadData; } }

        TLV UserData;
        TLV FullyEncodedData;
        TLV[] PdvList;
        public OsiPresentationPacket(ByteArraySegment bas, Packet parent)
        {

            ParentPacket = parent;
            UserData = new TLV(bas);
            FullyEncodedData = new TLV(UserData.Value.Bytes, UserData);
            PdvList = new TLV[2];
            PdvList[0] = new TLV(FullyEncodedData.Value.Bytes, FullyEncodedData);
            PresentationContextIdentifier = (ContextIdentifier)(PdvList[0].Value.RawBytes[0]);

            PdvList[1] = new TLV(FullyEncodedData.Value.Bytes.EncapsulatedBytes(), FullyEncodedData);
            //this.header = bas;
            //this.header.Length = UserData.Bytes.Length-PdvList[1].Bytes.Length;
            var payload = PdvList[1].Value.Bytes;
            payloadPacketOrData = new PacketOrByteArraySegment();

            switch (PresentationContextIdentifier)
            {
                case ContextIdentifier.Mms:
                    payloadPacketOrData.ThePacket = new MmsPacket(payload, this);
                    break;
                default:
                    payloadPacketOrData.TheByteArraySegment = payload;
                    break;
            }
        }
    }
}
