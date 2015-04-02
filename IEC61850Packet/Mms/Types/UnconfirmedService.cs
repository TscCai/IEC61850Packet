using IEC61850Packet.Asn1;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;

namespace IEC61850Packet.Mms
{
    public class UnconfirmedService : MmsService
    {
        public UnconfirmedServiceType AvaliableFiled { get; set; }
        public InformationReport InformationReport { get; set; }
        // unsolicitedStatus and eventNotification 
        // should have been two properties as informatnReport

        public UnconfirmedService() { }
        public UnconfirmedService(byte[] identifier, ByteArraySegment bas)
        {
            this.Identifier = identifier;
            AvaliableFiled = (UnconfirmedServiceType)BigEndianBitConverter.Big.ToInt8(this.Identifier, 0);
            //TLV service = new TLV(bas);
            switch (AvaliableFiled)
            {
                case UnconfirmedServiceType.InformationReport:
                    InformationReport = new InformationReport(bas);
                    break;
                case UnconfirmedServiceType.UnsolicitedStatus:
                    break;
                case UnconfirmedServiceType.EventNotification:
                    break;
                default:
                    break;
            }

            this.Bytes = bas;

            // us.Tag.RawBytes
        }
    }
}
