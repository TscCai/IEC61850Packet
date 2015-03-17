using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet;
using IEC61850Packet.Utils;
using IEC61850Packet.Mms;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Asn1;
using System.Linq;

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

                        MmsPacket mms = (MmsPacket)packets[0].Extract(typeof(MmsPacket));
                        if (mms.Pdu is UnconfirmedPdu)
                        {
                            var pdu = mms.Pdu as UnconfirmedPdu;
                            string vmd_specific = pdu.Service.InformationReport.VariableAccessSpecification.VariableListName.Vmd_Specific;
                            Console.WriteLine(vmd_specific);
                        }

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

        [TestMethod]
        public void DataTest()
        {
            byte[] id = { 0x8A };
            byte[] len = { 0x0F };
            byte[] val = { 0x62, 0x72, 0x63, 0x62, 0x52, 0x65, 0x6C, 0x61, 0x79, 0x45, 0x6E, 0x61, 0x41, 0x30, 0x31 };
            byte[] raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);
            Data d = new Data(new TLV(new ByteArraySegment(raw)));

            if (d.Type == Data.VariableType.VisibleString)
            {
                Console.WriteLine(d.GetValue<VisibleString>().Value);
            }
            id[0] = 0x84;
            len[0] = 0x03;
            val = new byte[] { 0x06, 0x3E, 0x00 };

            raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);

            d = new Data(new TLV(new ByteArraySegment(raw)));
            if (d.Type == Data.VariableType.BitString)
            {
                Console.WriteLine(d.GetValue<BitString>().Value);
            }

        }

        public void FloatingPoint_Test()
        {
            float a = 45.0000000F;
            byte[] val = new byte[] { 0x42, 0x34, 0x00, 0x00 };
            a = BigEndianBitConverter.Big.ToSingle(val, 0);
            Assert.AreEqual(45.0f, a);
        }

        [TestMethod]
        public void TimeOfDay_Test()
        {
            DateTime date = new DateTime(2014, 8, 13, 7, 8, 31, 587);
            DateTime baseline = new DateTime(1984, 1, 1);
            byte[] id = { 0x8C };
            byte[] len = { 0x06};
            byte[] val = { 0x01, 0x88, 0x53, 0xE3, 0x2B, 0xAE };
            byte[] raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);

            TimeOfDay tod = new TimeOfDay(new TLV(new ByteArraySegment(raw)));
            DateTime result = tod.Value;
            Assert.AreEqual(date, result);

            
        }

    }
}
