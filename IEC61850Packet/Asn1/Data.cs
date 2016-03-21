using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Asn1.Types;
using TAsn1 = IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Asn1
{
    public class Data
    {
        public VariableType Type { get; private set; }
        public Data() { }
        public object Value { get; set; }

        public Data(TLV tlv)
        {

            Type = (VariableType)BigEndianBitConverter.Big.ToInt8(tlv.Tag.RawBytes, 0);
            switch (Type)
            {
                case VariableType.Array:
                    break;
                case VariableType.Structure:
                    Value = new Structure(tlv);
                    break;
                case VariableType.Boolean:
                    Value = new TAsn1.Boolean(tlv);
                    break;
                case VariableType.BitString:
                    Value = new BitString(tlv);
                    break;
                case VariableType.Integer:
                    Value = new Integer(tlv);
                    break;
                case VariableType.Unsigned:     // MMS Unsigned max value is System.Int32.MaxValue, not UInt32.MaxValue
                    Value = new Integer(tlv);
                    if (((Integer)Value).Value < 0) { throw new FormatException("Unsigned should be non-negative."); }
                    break;
                case VariableType.FloatPoint:
                    Value = new FloatPoint(tlv);
                    break;
                case VariableType.OctetString:
                    Value = new OctetString(tlv);
                    break;
                case VariableType.VisibleString:
                    Value = new VisibleString(tlv);
                    break;
                case VariableType.GeneralizedTime:
                    break;
                case VariableType.BinaryTime:   // AKA TimeOfDay
                    Value = new TimeOfDay(tlv);
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
                    Value = new UtcTime(tlv);
                    break;
                default:
                    break;
            }
        }

        public T GetValue<T>()
        {
            return (T)Value;
        }

    }
}
