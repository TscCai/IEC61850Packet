using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;

namespace IEC61850Packet.Mms.Types
{
    public class FloatPoint:OctetString
    {
        public new double Value { get; set; }
        public FloatPoint() 
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.CONTEXT_SPECIFIC, BerIdentifier.PRIMITIVE, 12);
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
    }
}
