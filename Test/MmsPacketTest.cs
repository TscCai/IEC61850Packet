using System;
using System.Collections.Generic;
using System.Linq;
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
using TAsn1 = IEC61850Packet.Asn1.Types;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Asn1;
using IEC61850Packet.Device;


namespace Test
{
    [TestClass]
    public class MmsPacketTest
    {
        [TestMethod]
        public void ParseFromFile()
        {
            int pcnt = 1;
            var dev = new CaptureFileReaderDevice(@"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap");
            dev.Filter = "ip src 198.121.0.115 and tcp"; // 92 or 115
            // won't work correctly if not give the ip source ( The buffer will take all the packets from different source)
            dev.Open();
            RawCapture rawCapture;
            List<Packet> packets = new List<Packet>();
            TpktPacketBuffer tpktBuff = new TpktPacketBuffer();
            CotpPacketBuffer cotpBuff = new CotpPacketBuffer();
            rawCapture = dev.GetNextPacket();
            
            
            while (rawCapture != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);            
                TcpPacket tcp = p.Extract<TcpPacket>();
                if (tcp.PayloadData.Length > 0)
                {
                    TpktFileds tf=new TpktFileds();
                    if (tpktBuff.Count>0 && !tpktBuff.IsReassembled)
                    {
                        tf.LeadWithSegment = true;
                        tf.LeadingSegmentLength = tpktBuff.Last.NextFrameSegmentLength;
                    }
                    TpktPacket tpkt = new TpktPacket(tcp.PayloadData, tcp,tf);
                    tpktBuff.Add(tpkt);
                    if (tpktBuff.IsReassembled)
                    {
                        foreach (TpktPacket reassTpkt in tpktBuff.Reassembled)
                        {
                            CotpPacket cotp = reassTpkt.Extract<CotpPacket>();
                            if (cotp.Type == CotpPacket.TpduType.DataTransfer)
                            {
                                cotpBuff.Add(cotp);
                                if (cotpBuff.IsReassembled)
                                {
                                    CotpPacket reassCotp = cotpBuff.Reassembled;
                                    packets.Add(new OsiSessionPacket(reassCotp.PayloadData, reassCotp));
                                    cotpBuff.Reset();

                                    #region For debug
#if DEBUG
                                    MmsPacket mms = (MmsPacket)packets.Last().Extract(typeof(MmsPacket));
                                    if (mms.Pdu is UnconfirmedPdu)
                                    {
                                        var pdu = mms.Pdu as UnconfirmedPdu;
                                        string dsRef = pdu.Service.InformationReport.ListOfAccessResult[3].Success.GetValue<IEC61850Packet.Asn1.Types.VisibleString>().Value;
                                        Console.WriteLine(dsRef);
                                    }
#endif
                                    #endregion
                                }

                            }
                        }
                        tpktBuff.Reset();
                    }
                }
                rawCapture = dev.GetNextPacket();
                pcnt++;
            }

            dev.Close();


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
            len[0] = 0x0E;
            val = new byte[] { 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xF0 };

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

        [TestMethod]
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
            byte[] len = { 0x06 };
            byte[] val = { 0x01, 0x88, 0x53, 0xE3, 0x2B, 0xAE };
            byte[] raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);

            TimeOfDay tod = new TimeOfDay(new TLV(new ByteArraySegment(raw)));
            DateTime result = tod.Value;
            Assert.AreEqual(date, result);


        }

        [TestMethod]
        public void Structure_Test()
        {
            byte[] raw = { 0xa2,0x12, 0x83, 0x01, 0x00, 0x84, 0x03,0x03,0x00,0x00,0x91,0x08,0x53,0xe9,0xc5,0x9f,
                         0xaa,0x7e,0xef,0x2a};
            Data d = new Data(new TLV(new ByteArraySegment(raw)));
            Structure s = d.GetValue<Structure>();
            for (int i = 0; i < s.Values.Count; i++)
            {
                switch (s.Types[i])
                {
                    case Data.VariableType.Boolean:
                        Console.WriteLine(((TAsn1.Boolean)s.Values[i]).Value);
                        break;
                    case Data.VariableType.BitString:
                        Console.WriteLine(((BitString)s.Values[i]).Value);
                        break;
                    case Data.VariableType.UtcTime:
                        Console.WriteLine(((UtcTime)s.Values[i]).ToString());
                        break;

                }
            }

        }

        [TestMethod]
        public void BitString_Test()
        {
            byte[] raw = { 0x84, 0x0B, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF ,0xE0};
            BitString bs = new BitString(new TLV(new ByteArraySegment(raw)));
            Console.WriteLine(bs.Value);
        }

        [TestMethod]
        public void ResovleDevice_Test()
        {
          //  string captureFilename = @"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap";
            // string captureFilename = @"..\..\CapturedFiles\20140725-210910_00036E3E-20+20_RcdD05.pcap";
            string captureFilename = @"..\..\CapturedFiles\20140826-113450-10+20_RcdD05_.pcap";
            ResolveDevice dev = new ResolveDevice(captureFilename);
            DateTime start = DateTime.Now;
            dev.Open();
            // some works here
            dev.Close();
            DateTime end = DateTime.Now;
            Console.WriteLine("Time eclapsed: {0}", (end - start).TotalSeconds);
            Console.WriteLine("Done.");
        }

    }
}
