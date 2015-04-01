using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Goose
{
    public class GooseFileds
    {
        public static int APPIDLength { get { return 2; } }
        public static int LengthLength { get { return 2; } }
        public static int Reserved1Length { get { return 2; } }
        public static int Reserved2Length { get { return 2; } }

        public static int HeaderLength { get { return APPIDLength + LengthLength + Reserved1Length + Reserved2Length; } }

    }
}
