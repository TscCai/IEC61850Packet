using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace IEC61850Packet.Asn1
{
    public class TagFiled
    {
        public enum TagType : byte
        {
            Universal = 0x00,
            Application = 0x40,
            Context_Specific = 0x80
        }


        public TagFiled(TagType type, bool isConstructed, int code)
        {
            Type = type;
            IsConstructed = IsConstructed;
            if (code < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            Code = code;
            RawBytes = EncodeTag();
        }

        public TagFiled(ByteArraySegment bas)
        {
            // set Length not ready
            Code = -1;
            int len = 1;
            bas.Length = len;
            byte tag = bas.ActualBytes()[0];

            Type = (TagType)(tag & TAG_TYPE_MASK);
            IsConstructed = Convert.ToBoolean(tag & CONSTRUCTED_MASK);

            Code = tag & TAG_CODE_MASK;
            /* 
             * When Code == TAG_CODE_MASK, the tag is more
             * than 1 byte, and the first byte change to a leading 
             * byte.
            */
            if (Code == MAX_SINGLE_OCTET_TAG_CODE)
            {
                int code = 0;
                byte succeedOctet;
                bool hasNextOctet = false;
                do
                {
                    code <<= 7;
                    bas.Length++;
                    succeedOctet = bas.EncapsulatedBytes(bas.Length - 1).ActualBytes()[0];
                    code |= succeedOctet;
                    // If the highest bit == 0, it's the last succeed octet
                    hasNextOctet = Convert.ToBoolean(succeedOctet >> LAST_SUCCEED_OCTET_BIT);
                } while (hasNextOctet);
                Code = code;
            }

            bas.Length = this.BytesCount;
            RawBytes = bas.ActualBytes();
            
        }

        static readonly byte TAG_TYPE_MASK = 0xC0;
        static readonly byte CONSTRUCTED_MASK = 0x20;
        static readonly byte TAG_CODE_MASK = 0x1F;
        static readonly byte MAX_SINGLE_OCTET_TAG_CODE = 0x1F;
        //static readonly byte SUCCEED_TAG_CODE_MASK = 0x7F;
        static readonly byte LAST_SUCCEED_OCTET_BIT = 7;
        static readonly byte VALID_SUCCEED_OCTET_BIT_CNT = 7;
        static readonly byte CONSTRUCTED_FLAG_BIT = 5;

        /// <summary>
        /// Bytes that the Tag take, availiable after Code is set.
        /// </summary>
        int BytesCount
        {
            get
            {
                int len=1;
                if(Code<0)
                {
                    throw new InvalidOperationException("Length is not ready since Code is not set yet.");
                }
                else if (Code >= MAX_SINGLE_OCTET_TAG_CODE)
                {
                    len += 1;   // When mutiple Tag bytes, first byte doesn't indicate the tag code.
                    int cnt = 0;
                    cnt=(int)Math.Ceiling(Math.Log(Code, 2) / 7);
                    len += cnt;
                }
                return len;
            }
        }

        public TagType Type { get; private set; }
        public bool IsConstructed { get; private set; }
        public int Code { get; private set; }
        public byte[] RawBytes { get; private set; }

        private byte[] EncodeTag()
        {
            byte[] result = new byte[BytesCount];
            int code = Code;
            result[0] = (byte)((byte)Type | (byte)((Convert.ToInt16(IsConstructed)) << CONSTRUCTED_FLAG_BIT));
            if (BytesCount == 1)
            {
                result[0] |= (byte)Code;
            }
            else if (BytesCount > 1)
            {
                result[0] |= MAX_SINGLE_OCTET_TAG_CODE;
                result[result.Length-1]=(byte)(code & ~(1<<VALID_SUCCEED_OCTET_BIT_CNT));
                code >>= VALID_SUCCEED_OCTET_BIT_CNT;

                for (int i = result.Length-2; i >=0; i--)
                {
                    result[i] = (byte)(0x80|(code & ~(1 << VALID_SUCCEED_OCTET_BIT_CNT)));
                    code >>= VALID_SUCCEED_OCTET_BIT_CNT;
                }
            }
            return result;
        }

    }
}
