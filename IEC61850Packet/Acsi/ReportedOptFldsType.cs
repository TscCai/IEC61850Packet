using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Acsi
{
    public enum ReportedOptFldsType:ushort
    {
        Reserved=0x8000,
        SequenceNumber=0x4000,
        ReportTimeStamp=0x2000,
        ReasonForInclusion=0x1000,
        DataSetName=0x0800,
        DataReference=0x0400,
        BufferOverflow=0x0200,
        EntryID=0x0100,
        ConfRev=0x0080,
        Segmentation=0x0040
    }
}
