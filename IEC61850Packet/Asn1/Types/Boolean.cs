using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;

namespace IEC61850Packet.Asn1.Types
{
    public class Boolean : BasicType
    {
        public bool Value { get; private set; }


        public Boolean() { this.Identifier = BerIdentifier.Encode(BerIdentifier.Universal, BerIdentifier.Primitive, BerIdentifier.Boolean); }
        public Boolean(TLV tlv):this()
        {
            int len = tlv.Length.Value;
            Value = BigEndianBitConverter.Big.ToBoolean(tlv.Value.RawBytes, 0);
            this.Bytes = tlv.Bytes;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
        
    }
}
