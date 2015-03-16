using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Mms.Types;

namespace IEC61850Packet.Mms
{
    public class InformationReport : BasicType
    {
        public VariableAccessSpecification VariableAccessSpecification { get; set; }
        public AccessResult[] ListOfAccessResult { get; set; }

        public InformationReport()
        {
            // According to MMS defination
            this.Identifier = BerIdentifier.Encode(BerIdentifier.CONTEXT_SPECIFIC, BerIdentifier.CONSTRUCTED, 0);
        }

        public InformationReport( ByteArraySegment bas)
            : this()
        {
            VariableAccessSpecification = new VariableAccessSpecification(bas);
            //TLV ir = new TLV(bas);
            //VariableAccessSpecificationType type = (VariableAccessSpecificationType)BigEndianBitConverter.Big.ToInt8(ir.Tag.RawBytes,0);
            //switch (type)
            //{
            //    case VariableAccessSpecificationType.VariableListName:
            //        VariableAccessSpecification = new VariableListName(ir.Value.Bytes);
            //        break;
            //    case VariableAccessSpecificationType.ListOfVariable:
            //        break;
            //    default:
            //        break;
            //}
            // Decode ListofAccessResult
        }
    }
}
