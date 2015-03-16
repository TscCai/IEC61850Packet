using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public class InformationReport:MmsService
    {
        public byte[] VariableAccessSpecification { get; set; }
        public AccessResult[] ListOfAccessResult { get; set; }
    }
}
