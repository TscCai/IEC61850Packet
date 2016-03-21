//#define SHOW_DETAILS
using System;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Asn1;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Sv.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiscUtil.Conversion;
using PacketDotNet.Utils;
using TAsn1 = IEC61850Packet.Asn1.Types;

namespace Test
{

    [TestClass]
    public class DataTypeTest
    {
    

        [TestMethod]
        public void DataTest()
        {
            byte[] id = { 0x8A };
            byte[] len = { 0x0F };
            byte[] val = { 0x62, 0x72, 0x63, 0x62, 0x52, 0x65, 0x6C, 0x61, 0x79, 0x45, 0x6E, 0x61, 0x41, 0x30, 0x31 };
            byte[] raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);
            Data d = new Data(new TLV(new ByteArraySegment(raw)));

            if (d.Type == VariableType.VisibleString)
            {
                Console.WriteLine(d.GetValue<VisibleString>().Value);
            }
            id[0] = 0x84;
            len[0] = 0x0E;
            val = new byte[] { 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xF0 };

            raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);

            d = new Data(new TLV(new ByteArraySegment(raw)));
            if (d.Type == VariableType.BitString)
            {
                Console.WriteLine(d.GetValue<BitString>().Value);
            }

        }

        [TestMethod]
        public void FloatingPoint_Test()
        {
            float a = 45.0000000F;
            byte[] val = new byte[] { 0x42, 0x34, 0x00, 0x00 };
            a = BigEndianBitConverter.Big.ToSingle(val, 0);
            Assert.AreEqual(45.0f, a);
        }

        [TestMethod]
        public void TimeOfDay_Test()
        {
            DateTime date = new DateTime(2014, 8, 13, 7, 8, 31, 587);
            DateTime baseline = new DateTime(1984, 1, 1);
            byte[] id = { 0x8C };
            byte[] len = { 0x06 };
            byte[] val = { 0x01, 0x88, 0x53, 0xE3, 0x2B, 0xAE };
            byte[] raw = new byte[id.Length + len.Length + val.Length];
            id.CopyTo(raw, 0);
            len.CopyTo(raw, id.Length);
            val.CopyTo(raw, id.Length + len.Length);

            TimeOfDay tod = new TimeOfDay(new TLV(new ByteArraySegment(raw)));
            DateTime result = tod.Value;
            Assert.AreEqual(date, result);


        }

        [TestMethod]
        public void Structure_Test()
        {
            byte[] raw = { 0xa2,0x12, 0x83, 0x01, 0x00, 0x84, 0x03,0x03,0x00,0x00,0x91,0x08,0x53,0xe9,0xc5,0x9f,
                         0xaa,0x7e,0xef,0x2a};
            Data d = new Data(new TLV(new ByteArraySegment(raw)));
            Structure s = d.GetValue<Structure>();
            for (int i = 0; i < s.Values.Count; i++)
            {
                switch (s.Types[i])
                {
                    case VariableType.Boolean:
                        Console.WriteLine(((TAsn1.Boolean)s.Values[i]).Value);
                        break;
                    case VariableType.BitString:
                        Console.WriteLine(((BitString)s.Values[i]).Value);
                        break;
                    case VariableType.UtcTime:
                        Console.WriteLine(((UtcTime)s.Values[i]).ToString());
                        break;

                }
            }

        }

        [TestMethod]
        public void BitString_Test()
        {
            byte[] raw = { 0x84, 0x0B, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF ,0xE0};
            BitString bs = new BitString(new TLV(new ByteArraySegment(raw)));
            Console.WriteLine(bs.Value);
        }

		[TestMethod]
		public void NoASDU_Test()
		{
			byte[] raw = {0x80,0x01,0x01 };
			NoAsdu na = new NoAsdu(new TLV(new ByteArraySegment(raw)));
			Console.WriteLine(na.Value);
		}
     
		[TestMethod]
		public void CDC_Quality_Test()
		{
			Quality q = new Quality(new byte[]{0x00,0x00,0x1F,0xFF});
			Console.WriteLine(q.detailQual);
		}
    }
}
