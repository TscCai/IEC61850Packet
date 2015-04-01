using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Mms.Types
{
    public class ListOfAccessResult:BasicType
    {

        public ListOfAccessResult() { this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Constructed, 0); }

    }
}
