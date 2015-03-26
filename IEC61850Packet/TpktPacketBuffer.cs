using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace IEC61850Packet
{
    public class TpktPacketBuffer
    {
        public List<TpktPacket> Reassembled { get; private set; }
        public bool IsReassembled { get; private set; }

        List<byte> segBuffer;
        List<TpktPacket> packetBuffer;
        public TpktPacketBuffer()
        {
            IsReassembled = false;
            Reassembled = new List<TpktPacket>();
            packetBuffer = new List<TpktPacket>();
            segBuffer = new List<byte>();
        }

        public TpktPacketBuffer(TpktPacket packet)
            : this()
        {
            Add(packet);
        }

        public TpktPacket Last
        {
            get { return packetBuffer.Last(); }
        }

        public int Count
        {
            get { return packetBuffer.Count; }
        }

        public void Reassemble()
        {
            Reassembled = new List<TpktPacket>();
            segBuffer.AddRange(packetBuffer[0].Header);
            segBuffer.AddRange(packetBuffer[0].PayloadData);
            Reassembled.Add(new TpktPacket(segBuffer.ToArray(), packetBuffer[0].ParentPacket));
            segBuffer.Clear();
            Reassemble(packetBuffer[0].TpktSegments);

            for (int i = 1; i < Count; i++)
            {
                TpktPacket p = packetBuffer[i];
                Reassemble(p.TpktSegments);
            }
            if (segBuffer.Count > 0)
            {
                Reassembled.Add(new TpktPacket(segBuffer.ToArray(), packetBuffer[Count - 1].ParentPacket));
                segBuffer.Clear();
            }
            IsReassembled = true;
        }

        private void Reassemble(List<TpktSegment> segments)
        {
            foreach (var i in segments)
            {
                if (i.StartWithHeader && segBuffer.Count > 0)
                {
                    Reassembled.Add(new TpktPacket(segBuffer.ToArray(), packetBuffer[0].ParentPacket));
                    segBuffer.Clear();
                }
                segBuffer.AddRange(i.Segment.ActualBytes());
            }
        }

        /// <summary>
        /// If the given TPKT packet doesn't contain segments, it will be added into <see cref="IEC61850Packet.TpktPacketBuffer.Reassembled"/>,
        /// </summary>
        /// <param name="packet"></param>
        public void Add(TpktPacket packet)
        {
            if (!packet.HasSegments)
            {
                Reassembled.Add(packet);
                IsReassembled = true;
            }
            else
            {
                packetBuffer.Add(packet);
                if (packet.LastSegment)
                {
                    Reassemble();
                }
            }
        }

        public void Reset()
        {
            IsReassembled = false;
            Reassembled.Clear();
            packetBuffer.Clear();
        }

    }
}
