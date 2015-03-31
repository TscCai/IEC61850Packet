using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet
{
    public class TpktPacket : Packet
    {
        public int Version { get; private set; }
        public byte[] Reserved { get; private set; }
        public List<TpktSegment> TpktSegments { get; private set; }
        public bool HasSegments { get; private set; }
        public bool LastSegment { get; private set; }
        public int NextFrameSegmentLength { get; private set; }
        TpktFileds fileds;

        /// <summary>
        /// TPKT packet length, including header. Decoded form the Length filed from header
        /// </summary>
        public int Length { get; private set; }

        public TpktPacket(byte[] rawData, Packet parent, TpktFileds tf)
        {
            ParentPacket = parent;
            fileds = tf;
            TcpPacketType type;
            this.header = new ByteArraySegment(rawData);
            TpktSegments = new List<TpktSegment>();
            if (fileds.LeadWithSegment)
            {
                BuildSegments(this.header, true);
                this.header.Length = 0;
                this.payloadPacketOrData.TheByteArraySegment = this.header.EncapsulatedBytes();
            }
            else
            {
                this.header.Length = TpktFileds.HeaderLength;
                type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(this.header.ActualBytes(), 0));
                switch (type)
                {
                    case TcpPacketType.Tpkt:
                        Length = BigEndianBitConverter.Big.ToInt16(this.header.ActualBytes(),
                   TpktFileds.VersionLength + TpktFileds.ReservedLength
                   );
                        ParseEncapsulatedBytes();
                        break;
                    default:    //// The payload of this packet is begin with a TPKT segment of previous one.
                        //int len = 0;// PreviousPacket.NextFrameSegmentLength;
                        //TcpSegments.Add(new ByteArraySegment(rawData, 0, len));

                        break;
                }
            }

        }

        public TpktPacket(byte[] rawData, Packet parent)
            : this(rawData, parent, new TpktFileds())
        {
        }

        /// <summary>
        /// Parse to COTP
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private void ParseEncapsulatedBytes()
        {
            ByteArraySegment payload;
            payload = this.header.EncapsulatedBytes();
            this.payloadPacketOrData = new PacketOrByteArraySegment();
            if (payload.Length > Length - TpktFileds.HeaderLength)
            {
                BuildSegments(payload, Math.Min(Length - TpktFileds.HeaderLength, payload.Length));
                this.payloadPacketOrData.TheByteArraySegment = payload;
            }
            else if (payload.Length == Length - TpktFileds.HeaderLength)
            {
                HasSegments = false;
                NextFrameSegmentLength = 0;
                // TODO: Check if payload is COTP
                byte[] header = payload.ActualBytes().Take(CotpFileds.HeaderLength).ToArray();
                if (CotpPacket.IsCotp(header))
                {
                    this.payloadPacketOrData.ThePacket = new CotpPacket(payload, this);
                }
                else
                {
                    this.payloadPacketOrData.TheByteArraySegment = payload;
                }

            }
            else
            {
                this.header.Length = 0;
                this.payloadPacketOrData.TheByteArraySegment = this.header.EncapsulatedBytes();
                payload = this.payloadPacketOrData.TheByteArraySegment;
                BuildSegments(payload, payload.Length);
                this.Length = 0;
            }
        }

        /// <summary>
        ///  For a frame with normal TPKT start and segments appended.
        /// </summary>
        /// <param name="payload">Actual payload of this frame</param>
        /// <param name="length">The length that TPKT header indicate(header itself is exclude)</param>
        private void BuildSegments(ByteArraySegment payload, int length)
        {
            HasSegments = true;

            // Let the length of original TKPT payload without segments
            // be input param length which indicate the TPKT's payload length.
            int segLen = payload.Length - length;
            payload.Length = length;

            if (segLen > 0)
            {
                ByteArraySegment segs = payload.EncapsulatedBytes();
                // The rest segments may be the next TPKT beginning which includes TPKT header.
                //  var potentialHeader = segs;// new ByteArraySegment(segs);
                //segs.Length = TpktFileds.TpktHeaderLength;
                // segs may contain multiple TPKT segments
                byte[] headerRawBytes = segs.ActualBytes().Take(TpktFileds.HeaderLength).ToArray();

                TcpPacketType type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(headerRawBytes, 0));
                switch (type)
                {
                    case TcpPacketType.Tpkt:
                        // LastSegment = false;
                        int tpktLen = BigEndianBitConverter.Big.ToInt16(headerRawBytes,
                            TpktFileds.ReservedLength + TpktFileds.VersionLength
                            );
                        BuildSegments(segs, tpktLen, segLen);
                        break;
                    default:
                        break;
                }

            }
            else
            {
                ByteArraySegment segs = new ByteArraySegment(payload.Bytes);
                BuildSegments(segs, this.Length, segs.Length);
            }
        }

        /// <summary>
        /// For a frame with leading segment.
        /// </summary>
        /// <param name="segs">The segment without the leading segments</param>
        private void BuildSegments(ByteArraySegment segs, bool referFileds)
        {
            HasSegments = true;
            TcpPacketType type;
            int segLen = segs.Length;
            if (referFileds)
            {
                segs.Length = fileds.LeadingSegmentLength;
                TpktSegments.Add(new TpktSegment(this.header.ActualBytes(), false));   // possible some refactoring here
                segs = segs.EncapsulatedBytes();
                // refresh the length value
                segLen = segs.Length;   // When succssor's payload has only one segment, it will crash. see X.X.0.115 No.7322, 7325
            }
            if (segLen > 0)
            {
                byte[] potentialHeader = segs.ActualBytes().Take(TpktFileds.HeaderLength).ToArray();
                type = (TcpPacketType)(BigEndianBitConverter.Big.ToInt16(potentialHeader, 0));
                switch (type)
                {
                    case TcpPacketType.Tpkt:
                        int tpktLen = BigEndianBitConverter.Big.ToInt16(potentialHeader,
                            TpktFileds.ReservedLength + TpktFileds.VersionLength
                            );
                        // refresh the length value
                        //    segs.Length = segLen;
                        BuildSegments(segs, tpktLen, segLen);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                LastSegment = true;
            }
        }


        /// <summary>
        /// Should always be invoked by other overload method.
        /// </summary>
        /// <param name="segs"></param>
        /// <param name="tpktLen"></param>
        /// <param name="segLen"></param>
        private void BuildSegments(ByteArraySegment segs, int tpktLen, int segLen)
        {
            if (tpktLen > segLen)
            {
                // The begin of next frame packet tcp payload is a successor of THIS segment.
                NextFrameSegmentLength = tpktLen - segLen;
                LastSegment = false;
                segs.Length = segLen;
                TpktSegments.Add(new TpktSegment(segs, true));
            }
            else if (tpktLen == segLen)
            {
                // This packet contains the last segment, it's ready for reasseble
                NextFrameSegmentLength = 0;
                LastSegment = true;     // The last segment of this frame is the last part of an integreted tpkt packet.
                segs.Length = segLen;
                TpktSegments.Add(new TpktSegment(segs, true));
            }
            else // if (tpktLen < segLen)
            {
                // Contains a integral TPKT and another one or more segment(s) succeed
                segs.Length = tpktLen;
                TpktSegments.Add(new TpktSegment(segs, true));
                BuildSegments(segs.EncapsulatedBytes(), false);
            }
        }

        public static bool IsTpkt(byte[] header)
        {
            bool result = false;
            int pos = 0;
            if (Enum.IsDefined(typeof(TcpPacketType), BigEndianBitConverter.Big.ToUInt16(header, pos)))
            {
                pos += TpktFileds.VersionLength + TpktFileds.ReservedLength;
                int len = BigEndianBitConverter.Big.ToUInt16(header, 2);
                if (len <= TpktFileds.MaxLength && len>0)
                {
                    result = true;
                }

            }
            return result;
        }

    }
}
