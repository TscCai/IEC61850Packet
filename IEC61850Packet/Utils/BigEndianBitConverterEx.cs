using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;

namespace IEC61850Packet.Utils
{
    public static class BigEndianBitConverterEx
    {
        public static sbyte ToInt8(this BigEndianBitConverter big, byte[] value, int startIndex)
        {
            return (sbyte)value[startIndex];
        }

        public static short ToInt16(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            if (value.Length - startIndex == 1 && padding)
            {
                byte[] newVal = new byte[2];
                newVal[1] = value[startIndex];
                return big.ToInt16(newVal, 0);
            }
            else
            {
                return big.ToInt16(value, startIndex);
            }

        }
        public static int ToInt32(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            int actualLength=value.Length - startIndex;
            if ( actualLength< 4 && actualLength>0 && padding)
            {
                byte[] newValue = new byte[4];
                int pos = 4 - actualLength;
                for(; pos<newValue.Length;pos++)
                {
                    newValue[pos] = value[startIndex];
                    startIndex++;
                }
                return big.ToInt32(newValue, 0);
            }
            else
            {
                return big.ToInt32(value, startIndex);
            }
        }
        public static long ToInt64(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            int actualLength = value.Length - startIndex;
            if (actualLength < 8 && actualLength > 0 && padding)
            {
                byte[] newValue = new byte[8];
                int pos = 8 - actualLength;
                for (; pos < newValue.Length; pos++)
                {
                    newValue[pos] = value[startIndex];
                    startIndex++;
                }
                return big.ToInt64(newValue, 0);
            }
            else
            {
                return big.ToInt64(value, startIndex);
            }
        }

        public static byte ToUInt8(this BigEndianBitConverter big, byte[] value, int startIndex)
        {
            return value[startIndex];
        }
        public static ushort ToUInt16(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            if (value.Length - startIndex == 1 && padding)
            {
                byte[] newVal = new byte[2];
                newVal[1] = value[startIndex];
                return big.ToUInt16(newVal, 0);
            }
            else
            {
                return big.ToUInt16(value, startIndex);
            }
        }
        public static uint ToUInt32(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            int actualLength = value.Length - startIndex;
            if (actualLength < 4 && actualLength > 0 && padding)
            {
                byte[] newValue = new byte[4];
                int pos = 4 - actualLength;
                for (; pos < newValue.Length; pos++)
                {
                    newValue[pos] = value[startIndex];
                    startIndex++;
                }
                return big.ToUInt32(newValue, 0);
            }
            else
            {
                return big.ToUInt32(value, startIndex);
            }
        }
        public static ulong ToUInt64(this BigEndianBitConverter big, byte[] value, int startIndex, bool padding)
        {
            int actualLength = value.Length - startIndex;
            if (actualLength < 8 && actualLength > 0 && padding)
            {
                byte[] newValue = new byte[8];
                int pos = 8 - actualLength;
                for (; pos < newValue.Length; pos++)
                {
                    newValue[pos] = value[startIndex];
                    startIndex++;
                }
                return big.ToUInt64(newValue, 0);
            }
            else
            {
                return big.ToUInt64(value, startIndex);
            }
        }

    }
}
