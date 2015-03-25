using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;

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
        List<CotpPacket> PacketBuffer { get; set; }
        CotpPacket reassembled;
        public CotpPacketBuffer()
        {
            PacketBuffer = new List<CotpPacket>();
        }

        public CotpPacketBuffer(CotpPacket packet)
            : this()
        {
            Add(packet);
        }

   

        public void Add(CotpPacket packet)
        {
            PacketBuffer.Add(packet);
            if (packet.LastDataUnit)
            {
                Reasseble();
            }
        }
        /*
        public List<T> Reassemble<T>()
        {
            throw new NotImplementedException("IEC61850Packet.CotpPacketBuffer can't reassemble packets as serveral ones.");
        }
        */
        /// <summary>
        /// Reaseemble the packets in Buffer, and flush it.
        /// </summary>
        public void Reasseble()
        {
            List<byte> joint = new List<byte>();
            joint.AddRange(PacketBuffer[PacketBuffer.Count - 1].Header);
            foreach (Packet i in PacketBuffer)
            {
                joint.AddRange(i.PayloadData);
            }

            Reassembled = new CotpPacket(new ByteArraySegment(joint.ToArray()), null);
            joint.Clear();
            joint = null;
            IsReassembled = true;
            PacketBuffer.Clear();

        }

        public void Reset()
        {
            PacketBuffer.Clear();
            Reassembled = null;
            IsReassembled = false;
        }
    }
}
