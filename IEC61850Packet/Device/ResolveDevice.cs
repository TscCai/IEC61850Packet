using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using IEC61850Packet.Utils;
#if DEBUG
using IEC61850Packet.Mms;
#endif

namespace IEC61850Packet.Device
{
    public class ResolveDevice : CaptureFileReaderDevice
    {
        public int PacketCount { get; private set; }
        public ResolveDevice(string captureFilename) : base(captureFilename) { }
        List<Packet> packets;
        public override void Open()
        {
            base.Open();

            int pcnt = 1;
            // var dev = new CaptureFileReaderDevice(@"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap");
            // dev.Filter = "ip src 198.121.0.92 and tcp"; // 92 or 115

            RawCapture rawCapture;
            packets = new List<Packet>();
            TpktPacketBuffer tpktBuff;
            CotpPacketBuffer cotpBuff;
            rawCapture = base.GetNextPacket();

            while (rawCapture != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                TcpPacket tcp = p.Extract<TcpPacket>();
                if (tcp != null && tcp.PayloadData.Length > 0)
                {
                    TpktFileds tf = new TpktFileds();
                    string srcIp = tcp.ParentPacket<IPv4Packet>().SourceAddress.ToString();
                    tpktBuff = TpktPacketBufferFactory.GetBuffer(srcIp);
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
                                cotpBuff = CotpPacketBufferFactory.GetBuffer(srcIp);
                                cotpBuff.Add(cotp);
                                if (cotpBuff.IsReassembled)
                                {
                                    CotpPacket reassCotp = cotpBuff.Reassembled;
#if DEBUG
                                    try
                                    {
#endif
                                    packets.Add(new OsiSessionPacket(reassCotp.PayloadData, reassCotp));
#if DEBUG
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Packet count: {0}",pcnt);
                                        Console.WriteLine(ex.StackTrace);
                                        throw ex;
                                    }
#endif
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
                else
                {
                    // For GOOSE and SV

                }
                rawCapture = base.GetNextPacket();
                pcnt++;
            }


        }
        public new Packet GetNextPacket()
        {
            return null;
        }
    }
}
