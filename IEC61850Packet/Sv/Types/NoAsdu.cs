using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using PacketDotNet.Utils;

namespace IEC61850Packet.Sv.Types
{
	/// <summary>
	/// The filed stand for number of ASDU
	/// </summary>
	public class NoAsdu : Integer
	{
		public NoAsdu()
		{
			this.Identifier = new byte[] { 0x80 };	// Refer to 9-2 Fig.A.4
		}
		public NoAsdu(ByteArraySegment bas)
			: this(new TLV(bas))
		{

		}
		public NoAsdu(TLV tlv)
			: base(tlv)
		{
			this.Identifier = new byte[] { 0x80 };	// Refer to 9-2 Fig.A.4
			
		}
	}
}
