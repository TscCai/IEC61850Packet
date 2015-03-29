using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Asn1.Types
{
    public class BitString : BasicType
    {
        static ulong SYMBOL_MASK = 1UL << 63;
        public string Value { get; set; }
        public BitString() { this.Identifier = BerIdentifier.Encode(BerIdentifier.UNIVERSAL, BerIdentifier.PRIMITIVE, BerIdentifier.BIT_STRING); }

        public BitString(TLV tlv)
            : this()
        {
            string str;
            byte bit = tlv.Value.RawBytes[0];
            // BitString is not always 3bytes, 
            int len = 8 * (tlv.Length.Value - 1);
            if (len / 8 > sizeof(ulong))
            {
                str = "";

                int cnt = (int)Math.Ceiling(len / 8m / sizeof(ulong));
                for (int i = 0; i < cnt; i++)
                {
                    byte[] tmp = tlv.Value.RawBytes.Skip(i * sizeof(ulong) + 1).Take(sizeof(ulong)).ToArray();
                    ulong nmbr = BigEndianBitConverter.Big.ToUInt64(tmp, 0, true);
                    
                    if ((nmbr & SYMBOL_MASK) > 0)
                    {
                        str += "1";
                        str += Convert.ToString((long)(nmbr & ~SYMBOL_MASK), 2);
                    }
                    else
                    {
                        str += Convert.ToString((long)nmbr, 2);
                    }
                }
                // Bit right shift
                str = str.Substring(0, str.Length - bit);
            }
            else
            {
                ulong nmbr = BigEndianBitConverter.Big.ToUInt64(tlv.Value.RawBytes, 1, true) >> bit;
                if ((nmbr & SYMBOL_MASK) > 0)
                {
                    str = "1";
                    str += Convert.ToString((long)(nmbr & ~SYMBOL_MASK), 2);
                }
                else
                {
                    str = Convert.ToString((long)nmbr, 2);
                }
            }
            Value = new String('0', len - str.Length - bit) + str;
            this.Bytes = tlv.Bytes;
        }
    }
}
