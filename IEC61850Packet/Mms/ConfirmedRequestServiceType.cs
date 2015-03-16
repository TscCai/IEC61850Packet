using PacketDotNet.MiscUtil.Asn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public enum ConfirmedRequestServiceType : byte
    {
        GetNameList = 0xA1,
        Read=0xA4,
        Write=0xA5,
        GetVariableAccessAttributes=0xA6,
        DefineNamedVariableList=0xAB,
        GetNamedVariableListAttributes=0xAC,
        DeleteNamedVariableList=0xAD
    }
}
