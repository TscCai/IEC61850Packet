using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Asn1
{
    public class BerIdentifier
    {
        public static byte Universal { get { return 0x00; } }
        public static byte Application { get { return 0x40; } }
        public static byte ContextSpecific { get { return 0x80; } }

        public static byte Primitive { get { return 0x00; } }
        public static byte Constructed { get { return 0x20; } }

        public static byte Boolean { get { return 1; } }
        public static byte Integer { get { return 2; } }
        public static byte BitString { get { return 3; } }
        public static byte OctetString { get { return 4; } }
        public static byte Null { get { return 5; } }
        public static byte ObjectIdentifier { get { return 6; } }
        public static byte Real { get { return 9; } }
        public static byte Enumerated { get { return 10; } }
        public static byte Utf8String { get { return 12; } }
        public static byte NumericString { get { return 18; } }
        public static byte PrintableString { get { return 19; } }
        public static byte TeletexString { get { return 20; } }
        public static byte VideotexString { get { return 21; } }
        public static byte Ia5String { get { return 22; } }
        public static byte GeneralizedTime { get { return 24; } }
        public static byte GraphicString { get { return 25; } }
        public static byte VisibleString { get { return 26; } }
        public static byte GeneralString { get { return 27; } }
        public static byte UniversalString { get { return 28; } }
        public static byte BmpString { get { return 30; } }

        static readonly byte MAX_SINGLE_OCTET_TAG_CODE =0x1F;
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

            result[0] = (byte)(type | structure | MAX_SINGLE_OCTET_TAG_CODE);
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
