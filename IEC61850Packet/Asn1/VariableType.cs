using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Asn1
{
	/// <summary>
	/// Tag identifiers for MMS, GOOSE and SV. All tag identifiers are Context Specific.
	/// All identifiers are extracted from Industrial automation systems-
	/// Manufacturing message specification - Part 2: Protocol specification (ISO 9506-2:2003)
	/// Chapter 14.4.2
	/// </summary>
	public enum VariableType : byte
	{
		// Primitive
		Boolean = 0x83,   //
		BitString = 0x84,
		Integer = 0x85,
		Unsigned = 0x86,  // UInt32
		FloatPoint = 0x87,
		// 0x88 is reserved
		OctetString = 0x89,
		VisibleString = 0x8A,
		GeneralizedTime = 0x8B,
		BinaryTime = 0x8C,    // TimeOfDay
		Bcd = 0x8D,   //Integer, non-negative
		BooleanArray = 0x8E,  // Bitstring
		ObjId = 0x8F, // Object Identifier
		MmsString = 0x90,  // MMSString
		UtcTime = 0x91,

		// Constructed
		Array = 0xA1, // Sequence of Data
		Structure = 0xA2 //Sequence of Data
		
	}

}
