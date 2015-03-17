using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Asn1
{
    public class LengthFiled
    {

        public enum LengthType {Short, Long, Uncertain }

        public byte[] RawBytes { get; private set; }

        public LengthType Type { get; private set; }

        public int Value { get; private set; }

        public LengthFiled(ByteArraySegment bas)
        {
            byte[] len = bas.EncapsulatedBytes(1).ActualBytes();    // try first byte to see if single-byte-length
            // ASN.1 long or uncertain form
            if ((len[0] & SINGLE_OCTET_FLAG_MASK) > 0)
            {
                int len_byteCnt = len[0] & VALID_VALUE_MASK;
                if (len_byteCnt > 0 && len_byteCnt<VALID_VALUE_MASK)
                {
                    Type = LengthType.Long;
                    //bas.Length += 1;    // skip the first leading byte
                    len=bas.EncapsulatedBytes(len_byteCnt+1).ActualBytes();
                    switch(len_byteCnt)
                    {
                        case 1:
                            Value = BigEndianBitConverter.Big.ToUInt8(len,1);
                            break;
                        case 2:
                            Value = BigEndianBitConverter.Big.ToUInt16(len,1);
                            break;
                        default:
                            Value = 0;
                            break;
                    }
                }
                else if(len_byteCnt==0)
                {
                    Type = LengthType.Uncertain;
                    // Uncertain, confirm by the 0x00 0x00 in ValueFiled
                    Value = -1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value","Bad argument when reading value of length");
                }
            }
            else
            {
                Type = LengthType.Short;              
                Value = len[0] & VALID_VALUE_MASK;
            }
            RawBytes = len;
        }

        public LengthFiled(byte[] rawBytes):this(new ByteArraySegment(rawBytes))
        {
        
        }

        public LengthFiled(LengthType type, int lengthValue)
        {
            Type = type;
            switch (Type)
            {
                case LengthType.Short:
                    if(lengthValue>0 && lengthValue<VALID_VALUE_MASK)
                    {
                        RawBytes=new byte[] {(byte)lengthValue};
                    }
                    else
                    {
                        throw new ArgumentException("LengthType doesn't match with lengthValue.");
                    }
                    break;
                case LengthType.Long:
                    int byteCnt = 1;    // 1 for leading byte
                    int tmp = lengthValue;

                    byteCnt += (int)Math.Ceiling(Math.Log(lengthValue, 2) / 8);
              
                    RawBytes = new byte[byteCnt];
                    RawBytes[0] = (byte)(LONG_TYPE_LEADING | byteCnt);
                    for (int i = byteCnt - 1; i >= 0; i--)
                    {
                        RawBytes[i] =(byte)(lengthValue & 0xFF);
                        lengthValue >>= 8;
                    }
                        break;
                case LengthType.Uncertain:
                    break;
                default:
                    break;
            }

        }

        public static readonly byte SINGLE_OCTET_FLAG_MASK = 0x80;
        public static readonly byte LONG_TYPE_LEADING = 0x80;
        public static readonly byte VALID_VALUE_MASK = 0x7F;    // ~0x80

    }
}
