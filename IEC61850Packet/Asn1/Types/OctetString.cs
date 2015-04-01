using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;

namespace IEC61850Packet.Asn1.Types
{
    public class OctetString:BasicType
    {
        public virtual string Value { get; set; }
        public OctetString() { this.Identifier = BerIdentifier.Encode(BerIdentifier.Universal,BerIdentifier.Primitive,BerIdentifier.OctetString); }
        public OctetString(TLV tlv):this()
        {
            Value = BitConverter.ToString(tlv.Value.RawBytes, 0);
            this.Bytes = tlv.Bytes;
        }
    }
}
