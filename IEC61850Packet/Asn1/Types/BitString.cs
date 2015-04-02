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
        static readonly ulong SYMBOL_MASK = 1UL << 63;
        //static readonly int SIZE_OF_LONG = sizeof(long);
        static readonly int SIZE_OF_ULONG = sizeof(ulong);
        public string Value { get; set; }
        public BitString() { this.Identifier = BerIdentifier.Encode(BerIdentifier.Universal, BerIdentifier.Primitive, BerIdentifier.BitString); }

        public BitString(TLV tlv)
            : this()
        {
            string str="";
            byte bit = tlv.Value.RawBytes[0];
            // BitString is not always 3bytes, 
            int len = 8 * (tlv.Length.Value - 1);
            if (len / 8 > SIZE_OF_ULONG)
            {
                int cnt = (int)Math.Ceiling(len / 8m / SIZE_OF_ULONG);
                for (int i = 0; i < cnt; i++)
                {
                    byte[] buffer = tlv.Value.RawBytes.Skip(i * SIZE_OF_ULONG + 1).Take(SIZE_OF_ULONG).ToArray();
                    ulong nmbr = BigEndianBitConverter.Big.ToUInt64(buffer, 0, true);
                    
                    if ((nmbr & SYMBOL_MASK) > 0)
                    {
                        str += "1";
                        str += Convert.ToString((long)(nmbr & ~SYMBOL_MASK), 2);
                    }
                    else
                    {
                        string tmp = Convert.ToString((long)nmbr, 2);
                        if (tmp.Length < 8*buffer.Length)
                        {
                            str += tmp.PadLeft(8 * buffer.Length, '0');
                        }
                        else
                        {
                            str += tmp;
                        }
                    }
                }
                // Bit right shift
                str = str.Substring(0, str.Length - bit);
            }
            else if(len/8>0)
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

        public override string ToString()
        {
            return Value;
        }
    }
}
