using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;

namespace IEC61850Packet.Asn1.Types
{
    public class VisibleString:BasicType
    {
        public string Value { get; private set; }
        public VisibleString() { this.Identifier = BerIdentifier.Encode(BerIdentifier.UNIVERSAL,BerIdentifier.PRIMITIVE,BerIdentifier.VISIBLE_STRING); }

        public VisibleString(TLV tlv):this()
        {
            int len = tlv.Length.Value;
            StringBuilder sb = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                sb.Append((char)tlv.Value.RawBytes[i]);
            }
            Value = sb.ToString();
            this.Bytes = tlv.Bytes;
        }

    }
}
