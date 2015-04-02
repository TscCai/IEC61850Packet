using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using TAsn1 = IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using PacketDotNet.Utils;

namespace IEC61850Packet.Mms.Types
{
    public class Structure : BasicType
    {
        public List<object> Values { get; private set; }
        public List<Data.VariableType> Types { get; private set; }
        public Structure()
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Constructed, 2);
            Values = new List<object>();
            Types = new List<Data.VariableType>();
        }

        public Structure(TLV tlv)
            : this()
        {
            this.Bytes = tlv.Bytes;
            int totalLen = tlv.Length.Value;
            ByteArraySegment bas = new ByteArraySegment(tlv.Value.RawBytes);
            
            int pos = 0;
            while (pos < totalLen)
            {
                TLV tlv_item = new TLV(bas);
                var itemType = (Data.VariableType)BigEndianBitConverter.Big.ToInt8(tlv_item.Tag.RawBytes, 0);
                BasicType item = null;
                switch (itemType)
                {
                    case Data.VariableType.Array:
                        break;
                    case Data.VariableType.Structure:
                        item = new Structure(tlv_item);
                        break;
                    case Data.VariableType.Boolean:
                        item = new TAsn1.Boolean(tlv_item);
                        break;
                    case Data.VariableType.BitString:
                        item = new BitString(tlv_item);
                        break;
                    case Data.VariableType.Integer:
                        item = new Integer(tlv_item);
                        break;
                    case Data.VariableType.Unsigned:     // MMS Unsigned max value is System.Int32.MaxValue, not UInt32.MaxValue
                        item = new Integer(tlv_item);
                        if (((Integer)item).Value < 0) { throw new FormatException("Unsigned should be non-negative."); }
                        break;
                    case Data.VariableType.FloatPoint:
                        item = new FloatPoint(tlv_item);
                        break;
                    case Data.VariableType.OctetString:
                        item = new OctetString(tlv_item);
                        break;
                    case Data.VariableType.VisibleString:
                        item = new VisibleString(tlv_item);
                        break;
                    case Data.VariableType.GeneralizedTime:
                        break;
                    case Data.VariableType.BinaryTime:   // AKA TimeOfDay
                        item = new TimeOfDay(tlv_item);
                        break;
                    case Data.VariableType.Bcd:
                        break;
                    case Data.VariableType.BooleanArray:
                        break;
                    case Data.VariableType.ObjId:
                        break;
                    case Data.VariableType.MmsString:
                        break;
                    case Data.VariableType.UtcTime:
                        item = new UtcTime(tlv_item);
                        break;
                    default:
                        break;

                }
                pos += item.Bytes.Length;
                bas.Length = item.Bytes.Length;
                bas = bas.EncapsulatedBytes();
                Values.Add(item);
                Types.Add(itemType);
            }

        }

        public override string ToString()
        {
            string result = "";
            for(int i=0;i<Values.Count;i++)
            {
                switch (Types[i])
                {
                    case Data.VariableType.Array:
                        break;
                    case Data.VariableType.Bcd:
                        break;
                    case Data.VariableType.BinaryTime:
                        break;
                    case Data.VariableType.BitString:
                        result += ((BitString)Values[i]).ToString();
                        break;
                    case Data.VariableType.Boolean:
                        result += (Values[i] as TAsn1.Boolean).ToString();
                        break;
                    case Data.VariableType.BooleanArray:
                        break;
                    case Data.VariableType.FloatPoint:
                        result += (Values[i] as FloatPoint).ToString();
                        break;
                    case Data.VariableType.GeneralizedTime:
                        
                        break;
                    case Data.VariableType.Integer:
                        result += (Values[i] as Integer).ToString();
                        break;
                    case Data.VariableType.MmsString:
                        
                        break;
                    case Data.VariableType.ObjId:
                        result += (Values[i] as ObjectIdentifier).ToString();
                        break;
                    case Data.VariableType.OctetString:
                        result += (Values[i] as OctetString).ToString();
                        break;
                    case Data.VariableType.Structure:
                        result += (Values[i] as Structure).ToString();
                        break;
                    case Data.VariableType.Unsigned:
                        result += (Values[i] as Integer).ToString();
                        break;
                    case Data.VariableType.UtcTime:
                        result += (Values[i] as UtcTime).ToString();
                        break;
                    case Data.VariableType.VisibleString:
                        result += (Values[i] as VisibleString).ToString();
                        break;
                }
                result += "\n";
            }
            return result;
        }

    }
}
