using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using MiscUtil.Conversion;

namespace IEC61850Packet.Asn1.Types
{
    public class FloatPoint:OctetString
    {
        public new double Value { get; set; }
        public FloatPoint() 
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Primitive, 12);
        }
        public FloatPoint(TLV tlv):this()
        {
            int len = tlv.Length.Value;
            int e = tlv.Value.RawBytes[0];
            if (len == 5 && e == 8)
            {
                // 32 bit Single
                Value = BigEndianBitConverter.Big.ToSingle(tlv.Value.RawBytes, 1);
            }
            else if(len ==9 && e==11)
            {
                // 64 bit Double
                Value = BigEndianBitConverter.Big.ToDouble(tlv.Value.RawBytes, 1);
            }
            this.Bytes = tlv.Bytes;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(string format)
        {
            return Value.ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        public string ToString(string format,IFormatProvider provider)
        {
            return Value.ToString(format,provider);
        }

    }
}
