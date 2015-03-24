using IEC61850Packet.Asn1;
using IEC61850Packet.Mms.Types;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Mms
{
    public class VariableAccessSpecification : BasicType
    {
        public ObjectName VariableListName { get; set; }
        public VariableAccessSpecificationType AvaliableFiled{get;private set;}
        // VariableSpecification and AlternateAccess should have been here
        public VariableAccessSpecification() { }

        public VariableAccessSpecification(ByteArraySegment bas)
        {
            TLV vas = new TLV(bas);
            AvaliableFiled = (VariableAccessSpecificationType)BigEndianBitConverter.Big.ToInt8(vas.Tag.RawBytes, 0);
            switch (AvaliableFiled)
            {
                case VariableAccessSpecificationType.VariableListName:
                    VariableListName = new ObjectName(vas.Value.Bytes);
                    break;
                case VariableAccessSpecificationType.ListOfVariable:
                    break;
                default:
                    break;
            }
            this.Bytes = vas.Bytes;
        }
    }
}
