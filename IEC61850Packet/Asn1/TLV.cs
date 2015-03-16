using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace IEC61850Packet.Asn1
{
    /// <summary>
    /// a Tag-Length-Value object, not same as PacketDotNet.LLDP.TLV
    /// </summary>
    public class TLV
    {
        public TLV() { }

        public TLV(ByteArraySegment bas)
        {
            
            Tag = new TagFiled(bas);
            bas.Length = Tag.RawBytes.Length;
            Length = new LengthFiled(bas);
            bas.Length += Length.RawBytes.Length;
            Value = new ValueFiled(bas, Length.Value);
        //    if (Tag.RawBytes.Length == 1)
        //    {
        ////        ValueType = Asn_1.DataTypeMap[Tag.RawBytes[0]];
        //    }
            bas.Length = Tag.RawBytes.Length + Length.RawBytes.Length + Value.Bytes.Length;
            Bytes = bas;
        }

        public TLV(ByteArraySegment bas, TLV parent)
            : this(bas)
        {
            this.Parent = parent;

        }


        public ByteArraySegment Bytes { get; private set; }

      //  public Type ValueType { get; private set; }
        public TagFiled Tag { get; private set; }
        public LengthFiled Length { get; private set; }
        public ValueFiled Value { get; private set; }

      //  public TLV[] NewValue { get; set; }

        public TLV Parent { get; private set; }
        // public TLV Children { get; private set; }



    }
}
