using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;
using IEC61850Packet.Utils;

namespace IEC61850Packet
{
    public class CotpPacketBuffer//,IReassemble
    {
        public CotpPacket Reassembled
        {
            get 
            {
                IsReassembled = false; 
                return reassembled;
            }
            set { reassembled = value; } 
            //get;
            //private set;
        }
        public bool IsReassembled { get; private set; }
        List<CotpPacket> packetBuffer { get; set; }
        CotpPacket reassembled;
		public int Count
		{
			get { return packetBuffer.Count; }
		}
        public CotpPacketBuffer()
        {
            packetBuffer = new List<CotpPacket>();
        }

        public CotpPacketBuffer(CotpPacket packet)
            : this()
        {
            Add(packet);
        }

   

        public void Add(CotpPacket packet)
        {
            packetBuffer.Add(packet);
            if (packet.LastDataUnit)
            {
                Reasseble();
            }
        }

        /// <summary>
        /// Reaseemble the packets in Buffer, and flush it.
        /// </summary>
        private void Reasseble()
        {
            ReSort();
            List<byte> joint = new List<byte>();
            joint.AddRange(packetBuffer[packetBuffer.Count - 1].Header);
            foreach (Packet i in packetBuffer)
            {
                joint.AddRange(i.PayloadData);
            }

            Reassembled = new CotpPacket(new ByteArraySegment(joint.ToArray()), packetBuffer[packetBuffer.Count - 1].ParentPacket);
            joint.Clear();
            joint = null;
            IsReassembled = true;
            packetBuffer.Clear();

        }

        private void ReSort()
        {
            packetBuffer = packetBuffer.OrderBy(p => p.ParentPacket.ParentPacket<TcpPacket>().SequenceNumber).ToList();
        }

        public void Reset()
        {
            packetBuffer.Clear();
            Reassembled = null;
            IsReassembled = false;
        }
    }
}
