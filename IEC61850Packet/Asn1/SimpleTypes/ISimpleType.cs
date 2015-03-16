using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Asn1.SimpleTypes
{
    public interface ISimpleType
    {
        T Decode<T>(byte[] data);
    }
}
