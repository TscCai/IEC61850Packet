using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public class UnconfirmedService:MmsService
    {
        public UnconfirmedServiceType Type { get; set; }
        public MmsService Service { get; set; }

    }
}
