using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Asn1.Types
{
    public class Integer:BasicType
    {
        public  int Value { get; set; }
        public Integer()
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.UNIVERSAL, BerIdentifier.PRIMITIVE, BerIdentifier.INTEGER);
        }

        public Integer(TLV tlv):this()
        {
            this.Bytes = tlv.Bytes;
            int len = tlv.Length.Value;
            // Maybe wrong if len !=4
            Value = BigEndianBitConverter.Big.ToInt32(tlv.Value.RawBytes, 0,true);
        }
       
    }
}
