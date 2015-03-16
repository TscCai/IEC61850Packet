using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using IEC61850Packet.Asn.SimpleTypes;
using IEC61850Packet.Asn.ConstructedTypes;

namespace IEC61850Packet.Asn
{
    public class Asn_1
    {
   
        public static Dictionary<byte, Type> DataTypeMap { get; private set; }

        static Asn_1()
        {
            // For Universal tag data type.
            DataTypeMap = new Dictionary<byte, Type>();
            DataTypeMap.Add(0x01, typeof(Boolean));
            DataTypeMap.Add(0x02, typeof(Int32));
            DataTypeMap.Add(0x03, typeof(BitString));
            DataTypeMap.Add(0x04, typeof(OctetString));
            DataTypeMap.Add(0x05, null);
            DataTypeMap.Add(0x06, typeof(ObjectIdentifier));
            DataTypeMap.Add(0x09, typeof(Double));
            DataTypeMap.Add(0x10, typeof(Enum));

            // For Presentation header tag
            DataTypeMap.Add(0x61, typeof(Sequence));
            DataTypeMap.Add(0x30, typeof(Sequence));
            DataTypeMap.Add(0xA0, typeof(Sequence));


            // For Context specific tag data type(like GOOSE or MMS)
            DataTypeMap.Add(0x81, typeof(Array));
            DataTypeMap.Add(0x82, typeof(Sequence));
            DataTypeMap.Add(0x83, typeof(Boolean));
            DataTypeMap.Add(0x84, typeof(BitString));
            DataTypeMap.Add(0x85, typeof(Int32));
            DataTypeMap.Add(0x86, typeof(UInt32));
            DataTypeMap.Add(0x87, typeof(Double));
            // 0x88, Unknown
            DataTypeMap.Add(0x89, typeof(OctetString));
            DataTypeMap.Add(0x8A, typeof(String));
            DataTypeMap.Add(0x8C, typeof(TimeOfDay));
            DataTypeMap.Add(0x8D, typeof(Bcd));

            DataTypeMap.Add(0x91, typeof(DateTime));
            // 0x8E, BooleanArray
        }


    }
}
