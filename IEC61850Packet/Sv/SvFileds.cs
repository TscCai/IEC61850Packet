using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Sv
{
    public class SvFileds
    {
        public static byte APPIDLength { get { return 2; } }
        public static byte LengthLength { get { return 2; } }
        public static byte Reserved1Length { get { return 2; } }
        public static byte Reserved2Length { get { return 2; } }

        public static byte HeaderLength { get { return (byte)(APPIDLength + LengthLength + Reserved1Length + Reserved2Length); } }



    }
}
