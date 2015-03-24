using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Asn1.Types
{
    public class BitString:BasicType
    {
        public string Value { get; set; }
        public BitString() { this.Identifier = BerIdentifier.Encode(BerIdentifier.UNIVERSAL, BerIdentifier.PRIMITIVE,BerIdentifier.BIT_STRING); }

        public BitString(TLV tlv) :this()
        {
            string str;
            byte bit = tlv.Value.RawBytes[0];
            // BitString is not always 3bytes, 
            int len = 8 * (tlv.Length.Value - 1);
            if(len/8>sizeof(long))
            {
                str = "";

                int cnt = (int)Math.Ceiling(len / 8m / sizeof(long));
                for(int i=0;i<cnt;i++)
                {
                    byte[] tmp = tlv.Value.RawBytes.Skip(i * sizeof(long) + 1).Take(sizeof(long)).ToArray();
                    long nmbr = BigEndianBitConverter.Big.ToInt64(tmp, 0, true);
                    str += Convert.ToString(nmbr, 2);
                }
                // Bit right shift
                str = str.Substring(0, str.Length - bit);
            }
            else
            {
                long nmbr = BigEndianBitConverter.Big.ToInt64(tlv.Value.RawBytes, 1, true) >> bit;
                str = Convert.ToString(nmbr, 2);
            }
            Value = new String('0', len - str.Length-bit) + str;
            this.Bytes = tlv.Bytes;
        }
    }
}
