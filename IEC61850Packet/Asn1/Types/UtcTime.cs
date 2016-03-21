using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Asn1.Types
{
    public class UtcTime:BasicType
    {
        public DateTime Value { get; private set; }
        public byte Quality { get; private set; }
        static readonly DateTime baseline = new DateTime(1970, 1, 1);
        public UtcTime()
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific,BerIdentifier.Primitive,17);
        }

        public UtcTime(TLV tlv)
            : this()
        {
            this.Bytes = tlv.Bytes;
            byte[] seconds = tlv.Value.RawBytes.Take(4).ToArray();
            byte[] fraction = tlv.Value.RawBytes.Skip(4).Take(3).ToArray();
            Quality = tlv.Value.RawBytes.Last();
            Value = baseline.AddSeconds(BigEndianBitConverter.Big.ToInt32(seconds,0));

            Value = Value.AddSeconds(BigEndianBitConverter.Big.ToInt32(fraction,0,true)/Math.Pow(2,24));

        }

        public override string ToString()
        {
            return Value.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "q" + Quality.ToString("X");
        }
    }
}
