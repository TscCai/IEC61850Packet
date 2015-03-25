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
        List<TpktPacket> PacketBuffer { get; set; }
        public TpktPacketBuffer(): base()
        {
            IsReassembled = false;
            Reassembled = new List<TpktPacket>();
            PacketBuffer = new List<TpktPacket>();
        }

        public TpktPacketBuffer(TpktPacket packet)
            : this()
        {
            Add(packet);
        }

        public TpktPacket Last
        {
            get { return PacketBuffer.Last(); }
        }

        public void Reassemble()
        {
            List<TpktPacket> result = new List<TpktPacket>();

            IsReassembled = true;
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
                PacketBuffer.Add(packet);
                if(packet.LastSegment)
                {
                    Reassemble();
                }
            }
        }

        public void Reset()
        {
            IsReassembled = false;
            Reassembled.Clear();
            PacketBuffer.Clear();
        }

    }
}
