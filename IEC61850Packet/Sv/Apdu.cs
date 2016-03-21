using System.Collections.Generic;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Utils;
using TAsn1 = IEC61850Packet.Asn1.Types;
using IEC61850Packet.Sv.Types;

namespace IEC61850Packet.Sv
{
    public class Apdu : BasicType
    {
		public SavPdu SavPdu { get; set; }
		
		public Apdu() { 
		}
		public Apdu(ByteArraySegment bas)      : this(new TLV(bas))
        {

        }
		public Apdu(TLV tlv)
            : this()
		{

		}



    }

}
