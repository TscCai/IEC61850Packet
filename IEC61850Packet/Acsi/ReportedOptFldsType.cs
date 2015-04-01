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
        Sequence_Number=0x4000,
        Report_Time_Stamp=0x2000,
        Reason_For_Inclusion=0x1000,
        Data_Set_Name=0x0800,
        Data_Reference=0x0400,
        Buffer_Overflow=0x0200,
        EntryID=0x0100,
        Conf_Rev=0x0080,
        Segmentation=0x0040
    }
}
