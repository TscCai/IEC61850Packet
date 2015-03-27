using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet
{
    public class CotpPacketBufferFactory
    {
        static Dictionary<string,CotpPacketBuffer> buffers;
        static CotpPacketBufferFactory()
        {
            buffers = new Dictionary<string, CotpPacketBuffer>();
        }
        internal CotpPacketBufferFactory() { }
        public static CotpPacketBuffer GetBuffer(string id)
        {
            CotpPacketBuffer result;
            if (buffers.ContainsKey(id))
            {
                result = buffers[id];
            }
            else
            {
                result = new CotpPacketBuffer();
                buffers.Add(id, result);
            }
            return result;
        }
    }
}
