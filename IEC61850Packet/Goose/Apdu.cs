using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Utils;
using TAsn1 = IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Goose
{
    public class Apdu : BasicType
    {
        public VisibleString gocbRef { get; set; }
        public Integer timeAllowedtoLive { get; set; }
        public VisibleString datSet { get; set; }
        public VisibleString goID { get; set; }
        public UtcTime t { get; set; }
        public Integer stNum { get; set; }
        public Integer sqNum { get; set; }
        public TAsn1.Boolean test { get; set; }
        public Integer confRev { get; set; }
        public TAsn1.Boolean ndsCom { get; set; }
        public Integer numDatSetEntries { get; set; }
        public List<Data> allData { get; set; }
        public BasicType security { get; set; }

        private Apdu()
        {
            allData = new List<Data>();
            security = null;
        }
        public Apdu(ByteArraySegment bas)
            : this(new TLV(bas))
        {

        }
        public Apdu(TLV tlv)
            : this()
        {
            base.Identifier = BerIdentifier.Encode(BerIdentifier.Application, BerIdentifier.Constructed, 1);
            base.Bytes = tlv.Bytes;
            ByteArraySegment pdu = tlv.Value.Bytes;

            pdu.Length = 0;
            TLV tmp = new TLV(pdu.EncapsulatedBytes());
            gocbRef = new VisibleString(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            timeAllowedtoLive = new Integer(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            datSet = new VisibleString(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            if(IsGoIDTag(tmp.Tag.RawBytes))
            {
                goID = new VisibleString(tmp);
            }

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            t = new UtcTime(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            stNum = new Integer(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            sqNum = new Integer(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            test = new TAsn1.Boolean(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            confRev = new Integer(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            ndsCom = new TAsn1.Boolean(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            numDatSetEntries = new Integer(tmp);

            pdu.Length += tmp.Bytes.Length;
            tmp = new TLV(pdu.EncapsulatedBytes());
            int pos = 0;
            int len = tmp.Value.Bytes.Length;
            while(pos<len)
            {
                TLV data = new TLV(tmp.Value.Bytes);
                allData.Add(new Data(data));
                pos += data.Bytes.Length;
            }

        }

        private static bool IsGoIDTag(byte[] tag)
        {
            return tag.IsSame(BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Primitive, 3));
        }

    }

}
