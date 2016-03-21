using System.Collections.Generic;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Utils;
using TAsn1 = IEC61850Packet.Asn1.Types;
using System;

namespace IEC61850Packet.Sv.Types
{
	public class SavPdu : BasicType
	{
		public Integer noASDU { get; set; }
		public BasicType security { get; set; }
		public List<Asdu> ASDU { get; set; }

		private SavPdu()
		{
			this.Identifier = new byte[] { 0x60 };

			ASDU = new List<Asdu>();
			security = null;
		}
		public SavPdu(ByteArraySegment bas)
			: this(new TLV(bas))
		{

		}
		public SavPdu(TLV tlv)
			: this()
		{
			this.Bytes = tlv.Bytes;
			ByteArraySegment pdu = tlv.Value.Bytes;
			pdu.Length = 0;

			TLV tmp = new TLV(pdu.EncapsulatedBytes());
			pdu.Length += tmp.Bytes.Length;
			noASDU = new NoAsdu(tmp);

			tmp = new TLV(pdu.EncapsulatedBytes());
			if (IsSecurity(tmp))
			{
				security = null;

				pdu.Length += tmp.Bytes.Length;
				tmp = new TLV(pdu.EncapsulatedBytes());
			}

			//ASDU = new Asdu()
			if (Asdu.IsAsduSeq(tmp) && noASDU.Value > 0)
			{
				int cnt = noASDU.Value;
				tmp = new TLV(pdu.EncapsulatedBytes());
				for (int i = 0; i < cnt; i++)
				{
					//tmp = new TLV(pdu.EncapsulatedBytes());
					Asdu asdu = new Asdu(tmp.Value.Bytes);
					pdu.Length += tmp.Bytes.Length;
					
					ASDU.Add(asdu);
				}
			}
			else
			{
				throw new Exception("Bad SV packet");
			}


		}

		bool IsSecurity(TLV tlv)
		{
			return tlv.Tag.RawBytes.IsSame(new byte[] { 0x81 });
		}

	}

}
