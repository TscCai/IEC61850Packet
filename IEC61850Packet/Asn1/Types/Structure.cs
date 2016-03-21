using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using PacketDotNet.Utils;

namespace IEC61850Packet.Asn1.Types
{
    public class Structure : BasicType
    {
        public List<object> Values { get; private set; }
        public List<VariableType> Types { get; private set; }
        public Structure()
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Constructed, 2);
            Values = new List<object>();
            Types = new List<VariableType>();
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
                var itemType = (VariableType)BigEndianBitConverter.Big.ToInt8(tlv_item.Tag.RawBytes, 0);
                BasicType item = null;
                switch (itemType)
                {
                    case VariableType.Array:
                        break;
                    case VariableType.Structure:
                        item = new Structure(tlv_item);
                        break;
                    case VariableType.Boolean:
						item = new IEC61850Packet.Asn1.Types.Boolean(tlv_item);
                        break;
                    case VariableType.BitString:
                        item = new BitString(tlv_item);
                        break;
                    case VariableType.Integer:
                        item = new Integer(tlv_item);
                        break;
                    case VariableType.Unsigned:     // MMS Unsigned max value is System.Int32.MaxValue, not UInt32.MaxValue
                        item = new Integer(tlv_item);
                        if (((Integer)item).Value < 0) { throw new FormatException("Unsigned should be non-negative."); }
                        break;
                    case VariableType.FloatPoint:
                        item = new FloatPoint(tlv_item);
                        break;
                    case VariableType.OctetString:
                        item = new OctetString(tlv_item);
                        break;
                    case VariableType.VisibleString:
                        item = new VisibleString(tlv_item);
                        break;
                    case VariableType.GeneralizedTime:
                        break;
                    case VariableType.BinaryTime:   // AKA TimeOfDay
                        item = new TimeOfDay(tlv_item);
                        break;
                    case VariableType.Bcd:
                        break;
                    case VariableType.BooleanArray:
                        break;
                    case VariableType.ObjId:
                        break;
                    case VariableType.MmsString:
                        break;
                    case VariableType.UtcTime:
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
                    case VariableType.Array:
                        break;
                    case VariableType.Bcd:
                        break;
                    case VariableType.BinaryTime:
                        break;
                    case VariableType.BitString:
                        result += ((BitString)Values[i]).ToString();
                        break;
                    case VariableType.Boolean:
						result += (Values[i] as IEC61850Packet.Asn1.Types.Boolean).ToString();
                        break;
                    case VariableType.BooleanArray:
                        break;
                    case VariableType.FloatPoint:
                        result += (Values[i] as FloatPoint).ToString();
                        break;
                    case VariableType.GeneralizedTime:
                        
                        break;
                    case VariableType.Integer:
                        result += (Values[i] as Integer).ToString();
                        break;
                    case VariableType.MmsString:
                        
                        break;
                    case VariableType.ObjId:
                        result += (Values[i] as ObjectIdentifier).ToString();
                        break;
                    case VariableType.OctetString:
                        result += (Values[i] as OctetString).ToString();
                        break;
                    case VariableType.Structure:
                        result += (Values[i] as Structure).ToString();
                        break;
                    case VariableType.Unsigned:
                        result += (Values[i] as Integer).ToString();
                        break;
                    case VariableType.UtcTime:
                        result += (Values[i] as UtcTime).ToString();
                        break;
                    case VariableType.VisibleString:
                        result += (Values[i] as VisibleString).ToString();
                        break;
                }
                result += ",";
            }
            result = result.Remove(result.Length-1);
            return result;
        }

    }
}
