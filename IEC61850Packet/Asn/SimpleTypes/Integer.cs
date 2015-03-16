using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Asn.SimpleTypes
{
    public class Integer:ISimpleType
    {

        public Integer Decode(byte[] data)
        {
            throw new NotImplementedException();
        }

        public T Decode<T>(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
