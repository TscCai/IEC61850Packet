using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using IEC61850Packet;
using IEC61850Packet.Utils;
using IEC61850Packet.Device;

namespace Test
{
     [TestClass]
     public  class MmsPacketTest
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
                     TpktFileds tf = new TpktFileds();
                     if (tpktBuff.Count > 0 && !tpktBuff.IsReassembled)
                     {
                         tf.LeadWithSegment = true;
                         tf.LeadingSegmentLength = tpktBuff.Last.NextFrameSegmentLength;
                     }
                     TpktPacket tpkt = new TpktPacket(tcp.PayloadData, tcp, tf);
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
#if DEBUG && SHOW_DETAILS
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
         public void ResovleDevice_Test()
         {
             string captureFilename = @"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap";
             // string captureFilename = @"..\..\CapturedFiles\20140725-210910_00036E3E-20+20_RcdD05.pcap";
             //string captureFilename = @"..\..\CapturedFiles\20140826-113450-10+20_RcdD05_.pcap";
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
