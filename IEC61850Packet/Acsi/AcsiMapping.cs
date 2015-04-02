using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using IEC61850Packet.Utils;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Asn1.Types;
using TAsn1 = IEC61850Packet.Asn1.Types;
using IEC61850Packet.Mms;

namespace IEC61850Packet.Acsi
{
    public class AcsiMapping
    {

        public string RptID { get; private set; }   // always exists
        public ReportedOptFldsType ReportedOptFlds { get; private set; }  // always exists
        public int SeqNum { get; set; }    // if (OptFlds.sequence-number)
        public DateTime TimeofEntry { get; private set; }   // if(OptFlds.report-time-stamp)
        public string DatSet { get; private set; }  // if(OptFlds.data-set-name)
        public bool BufOvfl { get; private set; }   // if(OptFlds.buffer-overflow)
        public string EntryID { get; private set; } // if(OptFlds.entryID)
        public int SubSeqNum { get; private set; } // if(OptFlds.segmentation)
        public bool MoreSegmentFollow { get; private set; } // if(OptFlds.segmentation)
        public string Inclusion_Bitstring { get; private set; }     // always exists
        public List<string> DataRef { get; private set; }   // if(OptFlds.data-reference)
        public List<BasicType> Value { get; private set; }
        public List<ReasonCodeType> ReasonCode { get; private set; }    // if(OptFlds.reason-for-inclusion)

        List<AccessResult> ListOfAccessResult { get; set; }
        int AvaliableCnt { get; set; }
        int pos = 0;

        public AcsiMapping(List<AccessResult> listOfAccessResult)
        {
            ListOfAccessResult = listOfAccessResult;
            Resovle();

        }

        private void Resovle()
        {
            RptID = ListOfAccessResult[pos].Success.GetValue<VisibleString>().Value;
            pos++;  // 0 is RptID, 1 is OptFlds

            BitString bitstr = ListOfAccessResult[pos].Success.GetValue<BitString>();
            pos++;

            ReportedOptFldsType optFlds = (ReportedOptFldsType)BigEndianBitConverter.Big.ToUInt16(bitstr.Bytes.ActualBytes(), 3);
            ReportedOptFlds = optFlds; //new List<ReportedOptFldsType>();


            if ((optFlds & ReportedOptFldsType.SequenceNumber) >0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Sequence_Number);
                SeqNum = ListOfAccessResult[pos].Success.GetValue<Integer>().Value;
                pos++;
            }
            if ((optFlds & ReportedOptFldsType.ReportTimeStamp) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Report_Time_Stamp);
                TimeofEntry = ListOfAccessResult[pos].Success.GetValue<TimeOfDay>().Value;
                pos++;
            }
            if ((optFlds & ReportedOptFldsType.DataSetName) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Data_Set_Name);
                DatSet = ListOfAccessResult[pos].Success.GetValue<VisibleString>().Value;
                pos++;
            }
            if ((optFlds & ReportedOptFldsType.BufferOverflow) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Buffer_Overflow);
                BufOvfl = ListOfAccessResult[pos].Success.GetValue<TAsn1.Boolean>().Value;
                pos++;
            }
            if ((optFlds & ReportedOptFldsType.EntryID) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.EntryID);
                EntryID = ListOfAccessResult[pos].Success.GetValue<OctetString>().Value;
                pos++;
            }
            if ((optFlds & ReportedOptFldsType.Segmentation) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Segmentation);
                // Incorrect codes below, won't use recently.
                SubSeqNum = ListOfAccessResult[pos].Success.GetValue<Integer>().Value;
                pos++;
                MoreSegmentFollow = ListOfAccessResult[pos].Success.GetValue<TAsn1.Boolean>().Value;
                pos++;
            }

            // Inclusion-bitstring
            Inclusion_Bitstring = ListOfAccessResult[pos].Success.GetValue<BitString>().Value;
            pos++;

            if ((optFlds & ReportedOptFldsType.DataReference) > 0)
            {
                DataRef = new List<string>();
                //ReportedOptFlds.Add(ReportedOptFldsType.Data_Reference);
                AvaliableCnt = Inclusion_Bitstring.Count(ch => ch == '1');
                for (int i = 0; i < AvaliableCnt; i++)
                {
                    DataRef.Add(ListOfAccessResult[pos].Success.GetValue<VisibleString>().Value);
                    pos++;
                }
            }

            // Value
            Value = new List<BasicType>();
            for (int i = 0; i < AvaliableCnt; i++)
            {
                Value.Add(ListOfAccessResult[pos].Success.GetValue<BasicType>());
                pos++;
            }

            // This filed never show up in packet, but still a option in ReportedOptFlds
            if ((optFlds & ReportedOptFldsType.ConfRev) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Conf_Rev);
            }
            if ((optFlds & ReportedOptFldsType.ReasonForInclusion) > 0)
            {
                //ReportedOptFlds.Add(ReportedOptFldsType.Reason_For_Inclusion);
                ReasonCode = new List<ReasonCodeType>();
                for (int i = 0; i < AvaliableCnt; i++)
                {
                    ReasonCode.Add((ReasonCodeType)BigEndianBitConverter.Big.ToUInt32(
                        ListOfAccessResult[pos].Success.GetValue<BitString>().Bytes.ActualBytes(), 0
                        ));
                    pos++;
                }

            }

        }


    }
}
