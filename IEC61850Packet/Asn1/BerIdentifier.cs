using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Asn1
{
    public class BerIdentifier
    {
        public static readonly byte UNIVERSAL = 0x00;
        public static readonly byte APPLICATION = 0x40;
        public static readonly byte CONTEXT_SPECIFIC = 0x80;

        public static readonly byte PRIMITIVE = 0x00;
        public static readonly byte CONSTRUCTED = 0x20;

        public static readonly byte BOOLEAN = 1;
        public static readonly byte INTEGER = 2;
        public static readonly byte BIT_STRING = 3;
        public static readonly byte OCTET_STRING = 4;
        public static readonly byte NULL = 5;
        public static readonly byte OBJECT_IDENTIFIER = 6;
        public static readonly byte REAL = 9;
        public static readonly byte ENUMERATED = 10;
        public static readonly byte UTF8_STRING = 12;
        public static readonly byte NUMERIC_STRING = 18;
        public static readonly byte PRINTABLE_STRING = 19;
        public static readonly byte TELETEX_STRING = 20;
        public static readonly byte VIDEOTEX_STRING = 21;
        public static readonly byte IA5_STRING = 22;
        public static readonly byte GENERALIZED_TIME = 24;
        public static readonly byte GRAPHIC_STRING = 25;
        public static readonly byte VISIBLE_STRING = 26;
        public static readonly byte GENERAL_STRING = 27;
        public static readonly byte UNIVERSAL_STRING = 28;
        public static readonly byte BMP_STRING = 30;

        static readonly byte MAX_SINGLE_OCTET_TAG_CODE = 0x1F;
        static readonly byte VALID_SUCCEED_OCTET_BIT_CNT = 7;

        /// <summary>
        ///  Get the encoded Identifier in byte array
        /// </summary>
        /// <param name="type">Identifier type, choose from pre-defined static variables inside this class</param>
        /// <param name="structure">Structure type, choose from pre-defined static variables inside this class</param>
        /// <param name="code">An non-negtive value, choose from pre-defined static variables or elsewhere</param>
        /// <returns></returns>
        public static byte[] Encode(byte type, byte structure, int code)
        {
            byte[] result;
            if (code < 0) { throw new ArgumentOutOfRangeException("code", "Attempt to set a negtive value."); }
            if (code <= 30)
            {
                result = new byte[1];
                result[0] = (byte)(type | structure | (byte)code);
            }
            else
            {
                int len = 2;  // When mutiple Tag bytes, first byte doesn't indicate the tag code.
                int cnt = 0;
                cnt = (int)Math.Ceiling(Math.Log(code, 2) / 7);
                len += cnt;
                result = new byte[len];

            }

            result[0] = (byte)(type | structure|MAX_SINGLE_OCTET_TAG_CODE);
            result[result.Length - 1] = (byte)(code & ~(1 << VALID_SUCCEED_OCTET_BIT_CNT));
            code >>= VALID_SUCCEED_OCTET_BIT_CNT;

            for (int i = result.Length - 2; i >= 0; i--)
            {
                result[i] = (byte)(0x80 | (code & ~(1 << VALID_SUCCEED_OCTET_BIT_CNT)));
                code >>= VALID_SUCCEED_OCTET_BIT_CNT;
            }
            return result;

        }
    }
}
