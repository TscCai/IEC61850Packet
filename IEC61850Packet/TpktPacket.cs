using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace IEC61850Packet
{
    public class TpktPacket:Packet
    {

        public int Version { get; private set; }
        public byte[] Reserved { get; private set; }

        /// <summary>
        /// TPKT packet length, including header. Decoded form the Length filed from header
        /// </summary>
        public int Length { get; private set; }

        public TpktPacket(byte[] rawData, Packet parent)
        {
            this.ParentPacket = parent;

            this.header = new ByteArraySegment(rawData);
            this.header.Length = TpktFileds.TpktHeaderLength;

            try 
            {
                TcpPacketType type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(header.ActualBytes(), header.Offset));
            }
            catch(Exception ex)
            {
                throw new TypeInitializationException(this.GetType().FullName, ex);
            }

            Length = BigEndianBitConverter.Big.ToInt16(rawData,
                TpktFileds.TpktHeaderVersionLength+TpktFileds.TpktHeaderReservedLength
                );

            this.payloadPacketOrData = ParseEncapsulatedBytes(header);

         
        }

        /// <summary>
        /// Parse to COTP
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        internal PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment header)
        {
            ByteArraySegment payload = header.EncapsulatedBytes();
            PacketOrByteArraySegment payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.ThePacket = new CotpPacket(payload, this);
            return payloadPacketOrData;
        }


    }
}
