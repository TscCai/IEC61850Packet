using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;

namespace IEC61850Packet.Mms.Types
{
    public class TimeOfDay:OctetString
    {
        static readonly DateTime baseline = new DateTime(1984,1,1);

        public new DateTime Value { get; private set; }
        public TimeOfDay()
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific,BerIdentifier.Primitive,12);
        }

        public TimeOfDay(TLV tlv):this()
        {
            int len = tlv.Length.Value;
            byte[] ms= tlv.Value.RawBytes.Take(4).ToArray();
            if(len ==6)
            {
                Value = baseline.AddDays(BigEndianBitConverter.Big.ToUInt16(tlv.Value.RawBytes,4));
            }
            else if(len==4)
            {
                Value = DateTime.Today;
            }
            else
            {
                throw new FormatException("TimeOfDay should have a length of 4 or 6 bytes");
            }
            Value = Value.AddMilliseconds(BigEndianBitConverter.Big.ToUInt32(ms,0));
            this.Bytes = tlv.Bytes;
        }

        public override string ToString()
        {
            return Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
        }
    }
}
