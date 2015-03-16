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
        public static byte ToInt8(this BigEndianBitConverter big ,byte[] value, int startIndex)
        {
            return value[startIndex];
        }
    }
}
