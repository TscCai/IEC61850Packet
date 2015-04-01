using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Mms
{
    public class InformationReport : BasicType
    {
        public VariableAccessSpecification VariableAccessSpecification { get; set; }
        public List<AccessResult> ListOfAccessResult { get; set; }
        TLV tlv_listOfAccessResult;
        byte[] ListOfAccessResultId { get; set; }
        public InformationReport()
        {
            // According to MMS defination
            this.Identifier = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Constructed, 0);
            this.ListOfAccessResultId = BerIdentifier.Encode(BerIdentifier.ContextSpecific, BerIdentifier.Constructed, 0);
        }

        public InformationReport(ByteArraySegment bas)
            : this()
        {
            TLV ir = new TLV(bas);
            this.Bytes = bas;
            VariableAccessSpecification = new VariableAccessSpecification(ir.Bytes);

            // Decode ListofAccessResult
            ByteArraySegment list = new ByteArraySegment(bas.EncapsulatedBytes());
            tlv_listOfAccessResult = new TLV(list);
            int totalLen = tlv_listOfAccessResult.Length.Value;
            byte[] items = tlv_listOfAccessResult.Value.RawBytes;
            ByteArraySegment bas_items = new ByteArraySegment(items);
            int pos = 0;
            ListOfAccessResult = new List<AccessResult>();
            while (pos < totalLen)
            {
                
                AccessResult ar = new AccessResult(new TLV(bas_items));
                ListOfAccessResult.Add(ar);
                pos += ar.Bytes.Length;
                bas_items = bas_items.EncapsulatedBytes();
            }


        }
    }
}
