using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Asn1;
using IEC61850Packet.Asn1.Types;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace IEC61850Packet.Sv.Types
{
	public enum ValidityType : byte
	{
		Good = 0x00,
		Invalid = 0x01,
		Reserved = 0x02,
		Questionable = 0x03
	}

	public enum SourceType : ushort
	{
		Process = 0x00,
		Substituted = 0x400
	}

	public enum DetailQualityType:ushort
	{
		Overflow = 0x04,
		OutOfRange = 0x08,
		BadReference = 0x10,
		Oscillatory = 0x20,
		Failure = 0x40,
		OldData = 0x80,
		Inconsisitent  = 0x100,
		Inaccurate = 0x200
	}

	public class DetailQuality
	{
		public bool overflow { get; private set; }
		public bool outOfRange { get; private set; }
		public bool badReference { get; private set; }
		public bool oscillatory { get; private set; }
		public bool failure { get; private set; }
		public bool oldData { get; private set; }
		public bool inconsisitent { get; private set; }
		public bool inaccurate { get; private set; }

		public DetailQuality(bool overflow, bool outOfRange, bool badReference, bool oscillatory,
			bool failure, bool oldData, bool inconsisitent, bool inaccurate
			)
		{
			this.overflow = overflow;
			this.outOfRange = outOfRange;
			this.badReference = badReference;
			this.oscillatory = oscillatory;
			this.failure = failure;
			this.oldData = oldData;
			this.inconsisitent = inconsisitent;
			this.inaccurate = inaccurate;
		}

		public DetailQuality(DetailQualityType dqType)
		{
			if (HasDetailQuality(dqType,DetailQualityType.Overflow))
			{
				overflow = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.OutOfRange))
			{
				outOfRange = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.BadReference))
			{
				badReference = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.Oscillatory))
			{
				oscillatory = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.Failure))
			{
				failure = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.OldData))
			{
				oldData = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.Inconsisitent))
			{
				inconsisitent = true;
			}
			if (HasDetailQuality(dqType, DetailQualityType.Inaccurate))
			{
				inaccurate = true;
			}
		}

		bool HasDetailQuality(DetailQualityType type,DetailQualityType flag)
		{
			return (type & flag) == flag;
		}
	}

	public class QualityFileds
	{
	//	public static byte ValidityLength = 2;
		public static byte ValidtyMask = 0x03;
	//	public static byte DetailQualLength = 8;
	//	public static byte SourceLength = 1;
		public static ushort SourceMask = 0x400;
	//	public static byte TestLength = 1;
		public static ushort TestMask = 0x800;
	//	public static byte OperatorBlockedLength = 1;
		public static ushort OperatorBlockedMask = 0x1000;
	}

	public class Quality
	{
		/// <summary>
		/// Bit 0 ~ Bit 1
		/// </summary>
		public ValidityType validity { get; private set; }
		//readonly byte validityLength = 2;
		//readonly byte validtyMask = 0x03;
		/// <summary>
		/// Bit 2 ~ Bit 9
		/// </summary>
		public DetailQuality detailQual { get; private set; }
		//readonly byte detailQualLength = 8;
		/// <summary>
		/// Bit 10
		/// </summary>
		public SourceType source { get; private set; }
		//readonly byte sourceLength = 1;
		//readonly ushort sourceMask = 0x400;
		/// <summary>
		/// Bit 11
		/// </summary>
		public bool test { get; private set; }
		//readonly byte testLength = 1;
		//readonly ushort testMask = 0x800;

		/// <summary>
		/// Bit 12
		/// </summary>
		public bool operatorBlocked { get; private set; }
		//readonly byte operatorBlockedLength = 1;
		//readonly ushort operatorBlockedMask = 0x1000;

		/// <summary>
		/// Bit 13 ~ Bit 31 are reserved.
		/// </summary>
		public Quality() { }

		public Quality(byte[] bytes)
		{
			uint q = BigEndianBitConverter.Big.ToUInt32(bytes,0);
			validity = (ValidityType)(q & QualityFileds.ValidtyMask);

			detailQual = new DetailQuality((DetailQualityType)q);

			source = (SourceType)(q & QualityFileds.SourceMask);

			test = (q & QualityFileds.TestMask) == 0 ? false : true;

			operatorBlocked = (q & QualityFileds.OperatorBlockedMask) == 0 ? false : true;


		}

		public Quality(ByteArraySegment bas):this(bas.ActualBytes())
		{

		}

	}
}
