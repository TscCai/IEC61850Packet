using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Mms
{
    public class Domain_Specific:ObjectName
    {
        public string DomainId { get; set; }
        public string ItemId { get; set; }
    }
}
