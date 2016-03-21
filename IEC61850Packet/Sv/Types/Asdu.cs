using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;

using IEC61850Packet.Utils;
using IEC61850Packet.Asn1.Types;
using TAsn1=IEC61850Packet.Asn1.Types;
using PacketDotNet.Utils;

namespace IEC61850Packet.Sv.Types
{
	public class Asdu : BasicType
	{
		/// <summary>
		/// 0x80
		/// Refer to IEC 61850-9-2: 2004, Chapter 8.5, Tab. 13
		/// </summary>
		public VisibleString svID { get; set; }

		/// <summary>
		/// 0x81, Optional
		/// </summary>
		public VisibleString datset { get; set; }
		
		/// <summary>
		/// 0x82
		/// </summary>
		public Integer smpCnt { get; set; }
		
		/// <summary>
		/// 0x83
		/// </summary>
		public Integer confRev { get; set; }
		
		/// <summary>
		/// 0x84
		/// </summary>
		public UtcTime refrTm { get; set; }

		/// <summary>
		/// 0x85
		/// </summary>
		public TAsn1.Boolean smpSynch { get; set; }
		
		/// <summary>
		/// 0x86
		/// if smpSynch is true, this filed shouldn't appear
		/// </summary>
		public TAsn1.Boolean smpRate { get; set; }

		/// <summary>
		/// 0x87
		/// </summary>
		public List<Channel> sample { get; set; }

		public Asdu()
		{
			this.Identifier = new byte[] { 0x30 };	// Refer to 9-2 Fig.A.4
			sample = new List<Channel>();
		}
		public Asdu(ByteArraySegment bas)
			: this(new TLV(bas))
		{

		}
		public Asdu(TLV tlv)
			: this()
		{
			Bytes = tlv.Bytes;
			ByteArraySegment pdu = tlv.Value.Bytes;
			pdu.Length = 0;
			TLV tmp = new TLV(pdu.EncapsulatedBytes());

			svID = new VisibleString(tmp);
			pdu.Length += tmp.Bytes.Length;
			tmp = new TLV(pdu.EncapsulatedBytes());

			if (IsDatset(tmp)) 
			{
				datset = new VisibleString(tmp);
				pdu.Length += tmp.Bytes.Length;
				tmp = new TLV(pdu.EncapsulatedBytes());
			}

			smpCnt = new Integer(tmp);
			pdu.Length += tmp.Bytes.Length;
			tmp = new TLV(pdu.EncapsulatedBytes());

			confRev = new Integer(tmp);
			pdu.Length += tmp.Bytes.Length;
			tmp = new TLV(pdu.EncapsulatedBytes());

			if (IsRefrTm(tmp))
			{
				refrTm = new UtcTime(tmp);
				pdu.Length += tmp.Bytes.Length;
				tmp = new TLV(pdu.EncapsulatedBytes());
			}

			smpSynch = new TAsn1.Boolean(tmp);
			pdu.Length += tmp.Bytes.Length;
			tmp = new TLV(pdu.EncapsulatedBytes());

			if (!smpSynch.Value) 
			{
				smpRate = new TAsn1.Boolean(tmp);
				pdu.Length += tmp.Bytes.Length;
				tmp = new TLV(pdu.EncapsulatedBytes());
			}


			tmp = new TLV(pdu.EncapsulatedBytes());

			ByteArraySegment chn = tmp.Value.Bytes;
			chn.Length = Channel.ChannelLength;
			sample.Add(new Channel(chn));
			// Confirm the loop
			while (chn.Length + chn.Offset < chn.BytesLength)
			{
				chn = chn.EncapsulatedBytes();
				chn.Length = Channel.ChannelLength;
				sample.Add(new Channel(chn));
			}
		}

		public static bool IsAsduSeq(TLV tlv)
		{
			return tlv.Tag.RawBytes.IsSame(new byte[]{(byte)VariableType.Structure});
		}

		static bool IsDatset(TLV tlv)
		{
			return tlv.Tag.RawBytes.IsSame(new byte[]{0x81});
		}

		static bool IsRefrTm(TLV tlv)
		{
			return tlv.Tag.RawBytes.IsSame(new byte[]{0x84});
		}
	}
}
