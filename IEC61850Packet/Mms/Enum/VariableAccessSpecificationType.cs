using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Mms
{
    public enum VariableAccessSpecificationType:byte
    {
        ListOfVariable=0xA0,
        VariableListName=0xA1
    }
}
