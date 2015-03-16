using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public class UnconfirmedPdu:MmsPdu
    {
        public UnconfirmedService Service { get; set; }
        public UnconfirmedDetail Detail { get; set; }
    }
}
