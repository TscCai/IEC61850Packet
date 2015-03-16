using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEC61850Packet;

namespace IEC61850Packet
{
    public class OsiSessionPdu
    {
        public byte PI { get; set; }
        public int Length { get; set; }

        public byte[] Value { get; set; } 
    }
}
