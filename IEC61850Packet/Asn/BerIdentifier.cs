using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet.MiscUtil.Asn
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
    }
}
