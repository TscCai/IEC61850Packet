using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace IEC61850Packet.Sv
{
	public class SvPacket:Packet
	{
		public SvPacket(ByteArraySegment bas, Packet parent)
        {
            base.ParentPacket = parent;
            base.header = bas;

            // TODO: Decode header
           // base.header.Length = GooseFileds.HeaderLength;
            byte[] ba_header=header.ActualBytes();
            int pos = 0;
			//APPID = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			//pos += GooseFileds.APPIDLength;
			//Length = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			//pos += GooseFileds.LengthLength;
			//Reserved1 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			//pos += GooseFileds.Reserved1Length;
			//Reserved2 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			//pos += GooseFileds.Reserved2Length;
			//APDU = new Apdu(header.EncapsulatedBytes());
			//base.payloadPacketOrData.TheByteArraySegment = APDU.Bytes;
            
        }

        public SvPacket(byte[] rawData, Packet parent)
            : this(new ByteArraySegment(rawData), parent)
        {

        }
	}
}
