using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Sv.Types;

namespace IEC61850Packet.Sv
{
	public class SvPacket:Packet
	{
		public ushort APPID { get; private set; }

		public ushort Length { get; private set; }

		public ushort Reserved1
		{
			get { return 0; }
			private set
			{
				if (value != 0)
				{
					throw new ArgumentOutOfRangeException("Reserved1", "Attemp to set a non-zero value to Reserved1.");
				}
			}
		}
		public ushort Reserved2
		{
			get { return 0; }
			private set
			{
				if (value != 0)
				{
					throw new ArgumentOutOfRangeException("Reserved2", "Attemp to set a non-zero value to Reserved2.");
				}
			}
		}
		public SavPdu APDU { get; private set; }

		public SvPacket(ByteArraySegment bas, Packet parent)
        {
            base.ParentPacket = parent;
            base.header = bas;

            // TODO: Decode header
            base.header.Length = SvFileds.HeaderLength;
            byte[] ba_header=header.ActualBytes();
            int pos = 0;
			APPID = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			pos += SvFileds.APPIDLength;
			Length = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			pos += SvFileds.LengthLength;
			Reserved1 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			pos += SvFileds.Reserved1Length;
			Reserved2 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
			pos += SvFileds.Reserved2Length;

			APDU = new SavPdu(header.EncapsulatedBytes());
			base.payloadPacketOrData.TheByteArraySegment = APDU.Bytes;
        }

        public SvPacket(byte[] rawData, Packet parent)
            : this(new ByteArraySegment(rawData), parent)
        {

        }
	}
}
