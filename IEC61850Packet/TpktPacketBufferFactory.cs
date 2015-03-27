using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet
{
    public class TpktPacketBufferFactory
    {
        static Dictionary<string,TpktPacketBuffer> buffers;
        internal TpktPacketBufferFactory() { }
        static TpktPacketBufferFactory()
        {
            buffers = new Dictionary<string, TpktPacketBuffer>();
        }
        public static TpktPacketBuffer GetBuffer(string id)
        {
            TpktPacketBuffer result;
            if (buffers.ContainsKey(id))
            {
                result = buffers[id];
            }
            else
            {
                result = new TpktPacketBuffer();
                buffers.Add(id, result);
            }
            return result;
        }
    }
}
