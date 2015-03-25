using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace IEC61850Packet.Utils
{
    public static class PacketEx
    {
        /// <summary>
        /// An improved method for <see cref="PacketDotNet.Packet.Extract(System.Type)"/>,
        /// which is easier to use. Return null if this packet doesn't contain the given type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The inner packet type which will be extracted out</typeparam>
        /// <param name="packet">The original packet</param>
        /// <returns>Extracted inner packet</returns>
        public static T Extract<T>(this Packet packet)
        {
            object p =  packet.Extract(typeof(T));
            return (T)p;
        }
    }
}
