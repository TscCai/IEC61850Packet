//#define SHOW_DETAILS
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
using IEC61850Packet.Goose;
#endif

namespace IEC61850Packet.Device
{
    public class ResolveDevice : CaptureFileReaderDevice
    {
        public int PacketCount { get; private set; }
        public bool HasNextPacket
        {
            get
            {
                bool result = false;
                if (pos < PacketCount)
                {
                    result = true;
                }
                return result;
            }
        }
        public ResolveDevice(string captureFilename) : base(captureFilename) { }
        List<Packet> packets = new List<Packet>();

        TpktPacketBuffer tpktBuff;
        CotpPacketBuffer cotpBuff;
        int pos = 1;
        public override void Open()
        {
            base.Open();


            // var dev = new CaptureFileReaderDevice(@"..\..\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap");
            // dev.Filter = "ip src 198.121.0.92 and tcp"; // 92 or 115

            RawCapture rawCapture;


            rawCapture = base.GetNextPacket();

            while (rawCapture != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                try
                {
                    TcpPacket tcp = p.Extract<TcpPacket>();
                    if (tcp != null && tcp.PayloadData.Length > 0)
                    {
                        ExtractUpperPacket(tcp);
                    }
                    else
                    {
                        // UNDONE: For GOOSE and SV or null TCP
                        EthernetPacket ether = p.Extract<EthernetPacket>();
                        ExtractEthernetPacket(ether);
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("No. {0}: {1}\nTPKT buffer count: {2}.", pos, ex.Message, tpktBuff.Reassembled.Count);
#if DEBUG
                    Console.WriteLine(ex.StackTrace);
#endif
                }
                finally
                {
                    rawCapture = base.GetNextPacket();
                    pos++;
                }

            }

            // TODO: Reset the pos, raise the Opened Event
            pos = 0;
            PacketCount = packets.Count;
        }

        private void ExtractUpperPacket(TcpPacket tcp)
        {

            TpktFileds tf = new TpktFileds();
            string srcIp = tcp.ParentPacket<IPv4Packet>().SourceAddress.ToString();
            tpktBuff = TpktPacketBufferFactory.GetBuffer(srcIp);
            if (tpktBuff.Count > 0 && !tpktBuff.IsReassembled)
            {
                tf.LeadWithSegment = true;
                tf.LeadingSegmentLength = tpktBuff.Last.NextFrameSegmentLength;
            }

            byte[] header = tcp.PayloadData.Take(TpktFileds.HeaderLength).ToArray();
            if (tf.LeadWithSegment || TpktPacket.IsTpkt(header))
            {
                TpktPacket tpkt = new TpktPacket(tcp.PayloadData, tcp, tf);
                if (tpkt.PayloadPacket != null || tpkt.PayloadData != null)
                {
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

                                    packets.Add(new OsiSessionPacket(reassCotp.PayloadData, reassCotp));

                                    cotpBuff.Reset();

                                    #region For debug
#if DEBUG &&  SHOW_DETAILS
                                    MmsPacket mms = (MmsPacket)packets.Last().Extract(typeof(MmsPacket));
                                    if (mms.Pdu is UnconfirmedPdu)
                                    {
                                        var pdu = mms.Pdu as UnconfirmedPdu;
                                        string dsRef = pdu.Service.InformationReport.ListOfAccessResult[3].Success.GetValue<IEC61850Packet.Asn1.Types.VisibleString>().Value;
                                        Console.WriteLine("No. {0}: {1}", pcnt, dsRef);
                                    }
#endif
                                    #endregion
                                }

                            }
                        }
                        tpktBuff.Reset();
                    }
                }
            }

        }

        private void ExtractEthernetPacket(EthernetPacket ether)
        {
            if(ether.Type==EthernetPacketType.None)
            {
                EthernetPacketTypeEx type = (EthernetPacketTypeEx)ether.Type;
                switch (type)
                {
                    case EthernetPacketTypeEx.Goose:
                        ether.PayloadPacket = new GoosePacket(ether.PayloadData,ether);
                        break;
                    case EthernetPacketTypeEx.Sv:
                        // UNDONE: SV construct
                        break;
                    case EthernetPacketTypeEx.Gse:
                        break;
                    default:
                        // Unknown packet
                        break;
                }
            }
            
        }

        public new Packet GetNextPacket()
        {
            if (pos <= PacketCount)
            {
                return packets[pos++];
            }
            else
            {
                throw new InvalidOperationException("No more packets.");
            }
        }
    }
}
