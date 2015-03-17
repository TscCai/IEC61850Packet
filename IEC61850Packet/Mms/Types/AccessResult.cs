using IEC61850Packet.Mms.Types;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEC61850Packet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Asn1;
using IEC61850Packet.Mms.Enum;
using IEC61850Packet.Asn1.Types;
using TAsn1 = IEC61850Packet.Asn1.Types;

namespace IEC61850Packet.Mms
{
    public class AccessResult:BasicType
    {
        public AccessResultFileds AvaliableField { get; private set; }
        public Data Success { get; set; }
        public TAsn1.Integer Failure { get; set; }
        public AccessResult(TLV tlv)
        {
            AvaliableField = (AccessResultFileds)BigEndianBitConverter.Big.ToInt8(tlv.Tag.RawBytes, 0);
            if (AvaliableField != AccessResultFileds.Failiure)
            {
                AvaliableField = AccessResultFileds.Success;
                Success = new Data(tlv);
            }
            else
            {
                // decode as INTEGER, haven't been tested, maybe work incorrectly
                Failure = new TAsn1.Integer(tlv);
            }

            this.Bytes = tlv.Bytes;
        }
    }
}
