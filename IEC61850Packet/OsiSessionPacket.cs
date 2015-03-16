using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;

namespace IEC61850Packet
{
    public class OsiSessionPacket:SessionPacket
    {
        public enum PduIdentifier : byte{GiveToken=0x01}
        public enum SpduIdentifier : byte { DataTransfer= 0x01}

        public PduIdentifier PduType { get; private set; }
        public int PduLength { get { return Pdu.Length; } }
        public OsiSessionPdu Pdu { get; private set; }

        public SpduIdentifier SpduType { get; private set; }
        public int SpduLength { get { return Spdu.Length; } }
        public OsiSessionPdu Spdu { get; private set; }

        public OsiSessionPacket(byte[] rawData, Packet parent)
        {
            this.ParentPacket = parent;
            int offset = 0;
            header = new ByteArraySegment(rawData);
            PduType = (PduIdentifier)rawData[ offset];
            offset += OsiSessionFileds.PiLength;
            Pdu = new OsiSessionPdu();
            Spdu = new OsiSessionPdu();

            Pdu.Length =(int) rawData[offset];
            offset+=OsiSessionFileds.PduLiLength;

            if (Pdu.Length > 0 )
            {
                Pdu.PI = rawData[offset];
                offset += 1;
                rawData.CopyTo(Pdu.Value,offset);
            }

            SpduType = (SpduIdentifier)rawData[offset];
            offset += OsiSessionFileds.SpiLength;

            Spdu.Length = (int)rawData[offset];
            offset += OsiSessionFileds.SpduLiLength;
            if (SpduLength > 0)
            {
                Spdu = new OsiSessionPdu();
                Spdu.PI = rawData[offset];
                offset += 1;
                rawData.CopyTo(Spdu.Value, offset);
            }

            header = new ByteArraySegment(rawData);
            header.Length = OsiSessionFileds.PiLength + OsiSessionFileds.PduLiLength + PduLength+
                                                OsiSessionFileds.SpduLiLength+OsiSessionFileds.SpiLength+SpduLength;
            var payload = header.EncapsulatedBytes();
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.ThePacket = new OsiPresentationPacket(payload,this);

        }


    }
}
