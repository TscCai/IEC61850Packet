﻿using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Asn
{
    public class ValueFiled
    {
        public ByteArraySegment Bytes { get; private set; }
        public byte[] RawBytes
        {
            get
            {
                return Bytes.ActualBytes();
            }
        }
        public ValueFiled(ByteArraySegment bas,int length)
        {
            Bytes = bas.EncapsulatedBytes(length);
            Bytes.Length = length;
        }
      
    }
}
