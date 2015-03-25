using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace IEC61850Packet
{
    public class TpktPacket : Packet
    {
        public int Version { get; private set; }
        public byte[] Reserved { get; private set; }
        public List<ByteArraySegment> TcpSegments { get; private set; }
        public bool HasSegments { get; private set; }
        public bool LastSegment { get; private set; }
        public int NextFrameSegmentLength { get; private set; }
        //public TpktPacket PreviousPacket { get; private set; }
        //public TpktPacket NextPacket { get; private set; }

        /// <summary>
        /// TPKT packet length, including header. Decoded form the Length filed from header
        /// </summary>
        public int Length { get; private set; }

        public TpktPacket(byte[] rawData, Packet parent, TpktFileds fileds)
        {
            ParentPacket = parent;
            //PreviousPacket = previous;
            //PreviousPacket.NextPacket = this;

            this.header = new ByteArraySegment(rawData);
            if (fileds.LeadWithSegment)
            {
                this.header.Length = fileds.LeadingSegmentLength;
            }
            else
            {
            }
            this.header.Length = TpktFileds.TpktHeaderLength;
            TcpPacketType type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(this.header.ActualBytes(), this.header.Offset));
            TcpSegments = new List<ByteArraySegment>();
            switch (type)
            {
                case TcpPacketType.Tpkt:
                    Length = BigEndianBitConverter.Big.ToInt16(rawData,
               TpktFileds.TpktHeaderVersionLength + TpktFileds.TpktHeaderReservedLength
               );
                    this.payloadPacketOrData = ParseEncapsulatedBytes(this.header);
                    break;
                default:    // The payload of this packet is begin with a TPKT segment of previous one.
                    int len =0;// PreviousPacket.NextFrameSegmentLength;
                    TcpSegments.Add(new ByteArraySegment(rawData, 0, len));
                    break;
            }
         
        }

        /// <summary>
        /// Parse to COTP
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment header)
        {
            ByteArraySegment payload = header.EncapsulatedBytes();
            PacketOrByteArraySegment payloadPacketOrData = new PacketOrByteArraySegment();
            if (payload.Length!=Length-TpktFileds.TpktHeaderLength)
            {
                BuildSegments(payload, Math.Min(Length - TpktFileds.TpktHeaderLength, payload.Length));
                payloadPacketOrData.TheByteArraySegment = payload;
            }
            else
            {
                HasSegments = false;
                NextFrameSegmentLength = 0;
                payloadPacketOrData.ThePacket = new CotpPacket(payload, this);
            }
            return payloadPacketOrData;
        }

        private void BuildSegments(ByteArraySegment payload, int length)
        {
            HasSegments = true;

            int segLen = payload.Length - length;
            payload.Length = length;
            ByteArraySegment segs = payload.EncapsulatedBytes();
            var potentialHeader = new ByteArraySegment(segs);
            potentialHeader.Length = TpktFileds.TpktHeaderLength;
            // segs may contain multiple TPKT segments
            byte[] rawPotentialHeader = potentialHeader.ActualBytes();
            TcpPacketType type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(rawPotentialHeader, 0));
            switch (type)
            {
                case TcpPacketType.Tpkt:
                    LastSegment = false;
                    int tpktLen = BigEndianBitConverter.Big.ToInt16(rawPotentialHeader, TpktFileds.TpktHeaderReservedLength + TpktFileds.TpktHeaderVersionLength);
                    if (tpktLen > segLen)
                    {
                        // The begin of next frame packet tcp payload is a successor of THIS segment.
                        NextFrameSegmentLength = tpktLen - segLen;
                        LastSegment = false;
                        potentialHeader.Length = segLen;
                        TcpSegments.Add(potentialHeader);
                    }
                    else if (tpktLen == segLen)
                    {
                        // This packet contains the last segment, it's ready for reasseble
                        NextFrameSegmentLength = 0;
                        LastSegment = true;     // The last segment of this frame is the last part of an integreted tpkt packet.
                        potentialHeader.Length = segLen;
                        TcpSegments.Add(potentialHeader);
                    }
                    else // if (tpktLen < segLen)
                    {
                        // Means there is another one or more segment(s) succeed
                        potentialHeader.Length = tpktLen;
                        TcpSegments.Add(potentialHeader);
                        BuildSegments(potentialHeader.EncapsulatedBytes(),1);
                    }
                    break;
                default:
                    break;
            }
        }


    }
}
