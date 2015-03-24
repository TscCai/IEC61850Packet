using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Asn1;
using IEC61850Packet.Mms.Enum;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Mms
{
    public class ObjectName : BasicType
    {
        public ObjectNameType AvailableFiled { get; private set; }
        public string Vmd_Specific { get; set; }
        public Dictionary<string, string> Domain_Specific { get; set; }
        public string AA_Specific { get; set; }


        public ObjectName() { }
        public ObjectName(ByteArraySegment bas)
        {
            TLV on = new TLV(bas);
            this.Identifier = on.Tag.RawBytes;
            AvailableFiled = (ObjectNameType)BigEndianBitConverter.Big.ToInt8(this.Identifier, 0);
            switch (AvailableFiled)
            {
                case ObjectNameType.Vmd_Specific:
                    int len = on.Value.RawBytes.Length;
                    StringBuilder sb = new StringBuilder(len);
                    for (int i = 0; i < len; i++)
                    {
                        sb.Append((char)on.Value.RawBytes[i]);
                    }
                    Vmd_Specific = sb.ToString();
                    break;
                case ObjectNameType.Domain_Specific:
                    break;
                case ObjectNameType.AA_Specific:
                    break;
                default:
                    break;
            }

            this.Bytes = bas;

        }
    }
}
