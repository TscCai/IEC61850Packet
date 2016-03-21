using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;
using PacketDotNet.Utils;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Sv.Types
{
	/// <summary>
	/// 8 bytes in total
	/// </summary>
	public class Channel
	{
		public static int ChannelLength { get { return 8; } }

		/// <summary>
		/// First 4 bytes, BigEndian
		/// </summary>
		public int value { get; private set; }
		readonly int valueLength = 4;

		/// <summary>
		/// The rest 4 bytes
		/// </summary>
		public Quality quality { get; private set; }
		readonly int qualityLength = 4;

		public Channel()
		{
		}

		public Channel(ByteArraySegment bas)
		{
			bas.Length = valueLength;
			value = BigEndianBitConverter.Big.ToInt32(bas.ActualBytes(), 0, true);

			quality = new Quality(bas.EncapsulatedBytes(qualityLength));
			bas.Length += qualityLength;
		}

	}
}
