using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public enum UnconfirmedServiceType:byte
    {
        InformationReport =0xA0,
        UnsolicitedStatus=0xA1,
        EventNotification=0xA2
    }
}
