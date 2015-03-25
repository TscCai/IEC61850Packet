using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;
using PacketDotNet;
using IEC61850Packet.Utils;

namespace IEC61850Packet
{
    public class CotpPacket : Packet
    {
        public enum TpduType : byte
        {
            ConnectioinRequest = 0xE0,
            ConnectionConfirm = 0xD0,
            DisconnectionRequest = 0x80,
            DataTransfer = 0xF0,
            ExpectedDataTransfer = 0x70

        }

        public TpduType Type { get; private set; }

        public int TpduNumber { get; private set; }

        public bool LastDataUnit { get; private set; }

        static readonly byte TPDU_NUM_MASK = 0x7F;
        static readonly byte LAST_DU_BIT= 7;

        public CotpPacket() { }

        public CotpPacket(ByteArraySegment bas, Packet parent)
        {
            this.ParentPacket = parent;
            header = bas;
            
            header.Length = BigEndianBitConverter.Big.ToInt8(new ByteArraySegment(bas.Bytes, bas.Offset, 1).ActualBytes(), 0) +
                CotpFileds.LengthLength;
            byte num_eot = header.ActualBytes()[CotpFileds.LengthLength + CotpFileds.PduTypeLength];

            this.Type = (TpduType)(BigEndianBitConverter.Big.ToInt8(header.ActualBytes(), 1));
            switch (Type)
            {
                case TpduType.ConnectioinRequest:
                    break;
                case TpduType.ConnectionConfirm:
                    break;
                case TpduType.DataTransfer:
                    TpduNumber = num_eot & TPDU_NUM_MASK;
                    LastDataUnit = Convert.ToBoolean(num_eot >> LAST_DU_BIT);
                    break;
                case TpduType.DisconnectionRequest:
                    break;
                case TpduType.ExpectedDataTransfer:
                    break;
                default:
                    break;
            }

            var payload = header.EncapsulatedBytes();
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = payload;

        }

    }
}
