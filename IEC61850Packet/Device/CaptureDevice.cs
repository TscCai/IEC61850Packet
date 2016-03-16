using System;
using System.Collections.Generic;
using System.Linq;
using IEC61850Packet.Goose;
using IEC61850Packet.Mms;
using IEC61850Packet.Utils;
using PacketDotNet;
using SharpPcap;
using SharpPcap.AirPcap;

namespace IEC61850Packet.Device
{
	public class CaptureDevice
	{
		#region Propoerties
        public event DeviceOpenedEventHandler OnOpened;
        public event DeviceOpeningEventHandler OnOpening;
		
		string _filter;
		public string Filter { get { return _filter; } set { device.Filter = value; _filter = value; } }
        #endregion

        #region Private members

		ICaptureDevice device = null;
		Stack<Packet> packets = new Stack<Packet>();
        //List<Packet> packets = new List<Packet>();
        //List<Type> packetTypes = new List<Type>();
        TpktPacketBuffer tpktBuff;
        CotpPacketBuffer cotpBuff;
        int currentPacketIndex = 1;
        long filePosition = 0;
        private delegate void RaiseEventHandler(object sender,int length);
        private event RaiseEventHandler OnRaising;
        #endregion

		public CaptureDevice() { }

        public CaptureDevice(ICaptureDevice device)
            : base()
        {
			this.device = device;
        }

		public CaptureDevice(string name)
		{
			// Wire Network Adaptor
			var list = CaptureDeviceList.Instance;
			var dev = list.SingleOrDefault(d => d.Name == name);

			// Wireless Network Adaptor
			if (dev == null)
			{
				var wl_list = AirPcapDeviceList.Instance;
				dev = wl_list.SingleOrDefault(d => d.Description == name);
			}

			this.device = dev;
		}

        public void Open()
        {
            device.Open();
        }

		public Packet GetNextPacket(RawCapture raw)
		{

			RawCapture rawCapture;
			Packet result = null;
			rawCapture = raw;
			if (rawCapture != null)
			{
				Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
				result = p;
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
#if DEBUG
                    Console.WriteLine("No. {0}: {1}\nTPKT buffer count: {2}.", currentPacketIndex, ex.Message, tpktBuff.Reassembled.Count);

                    Console.WriteLine(ex.StackTrace);
#endif
				}


			}
			if (packets.Count > 0)
			{
				result = packets.Pop();
			}

			return result;

		}


        public Packet GetNextPacket()
        {
		
			RawCapture rawCapture;
			Packet result = null;
			rawCapture = device.GetNextPacket();
			if (rawCapture != null)
			{
				Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
				result = p;
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
#if DEBUG
                    Console.WriteLine("No. {0}: {1}\nTPKT buffer count: {2}.", currentPacketIndex, ex.Message, tpktBuff.Reassembled.Count);

                    Console.WriteLine(ex.StackTrace);
#endif
				}


			}
			if (packets.Count > 0) { 
			result = packets.Pop();
			}
			
			return result;

        }

        public void Close()
        {

            tpktBuff.Reset();
            tpktBuff = null;
            cotpBuff.Reset();
            cotpBuff = null;

            currentPacketIndex = 0;
            device.Close();
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
                                    var session = new OsiSessionPacket(reassCotp.PayloadData, reassCotp);
                                    //packets.Add();
                                    var mms = session.Extract<MmsPacket>();
                                    if (mms != null)
                                    {
                                        packets.Push(mms);
                                    }
                                    else
                                    {
                                        packets.Push(session);
                                    }

                                    cotpBuff.Reset();

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
            switch (ether.Type)
            {
                case EthernetPacketType.Goose:
                    ether.PayloadPacket = new GoosePacket(ether.PayloadData, ether);
                    int len = ether.PayloadPacket.Extract<GoosePacket>().APDU.Bytes.Length;
                    packets.Push(ether.PayloadPacket);
                  //  packetTypes.Add(typeof(GoosePacket));
                    break;
                case EthernetPacketType.Sv:
                    // UNDONE: SV construct
                    // packets.Add(ether);
                    break;
                case EthernetPacketType.Gse:
                    break;
                default:
                    // Unknown packet
                    break;
            }

        }

	}
}
