using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using IEC61850Packet;
using System.Collections.Generic;

namespace Test
{
    [TestClass]
    public class MmsPacketTest
    {
        [TestMethod]
        public void ParseFromFile()
        {
            var dev = new CaptureFileReaderDevice(@"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap");
            dev.Filter = "ip src 198.121.0.92 and tcp";
            dev.Open();
            RawCapture rawCapture;
            List<Packet> packets = new List<Packet>();
            List<Packet> buffer = new List<Packet>();
            rawCapture = dev.GetNextPacket();
            while (rawCapture != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                TcpPacket tcp = (TcpPacket)p.Extract(typeof(TcpPacket));

                TpktPacket tpkt = new TpktPacket(tcp.PayloadData, tcp);
                CotpPacket cotp = (CotpPacket)tpkt.Extract(typeof(CotpPacket));
                if (cotp.Type == CotpPacket.TpduType.DataTransfer)
                {
                    buffer.Add(cotp);
                    if (cotp.LastDataUnit)
                    {
                        // Reassemble packet
                        Packet reass = ReassemblePacket(buffer);
                        packets.Add(new OsiSessionPacket(reass.PayloadData, reass));
                        buffer.Clear();
                    }

                }


                rawCapture = dev.GetNextPacket();
            }

            dev.Close();


        }

        private Packet ReassemblePacket(List<Packet> buffer)
        {

            List<byte> joint = new List<byte>();
            joint.AddRange(buffer[buffer.Count - 1].Header);
            foreach (Packet i in buffer)
            {
                joint.AddRange(i.PayloadData);
            }

            CotpPacket reaseembled = new CotpPacket(new ByteArraySegment(joint.ToArray()), null);

            joint.Clear();
            joint = null;

            return reaseembled;
        }


    }
}
