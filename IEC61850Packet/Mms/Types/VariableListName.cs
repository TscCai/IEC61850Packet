using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Mms.Types
{
    public class VariableListName:VariableAccessSpecification
    {
        public VariableListName() 
        {
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Primitive, 0);
            //this.Bytes=
        }
        public VariableListName(ByteArraySegment bas)
        {
            TLV vln = new TLV(bas);

            
        }

        
    }
}
