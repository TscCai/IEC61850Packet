using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC61850Packet.Mms
{
    public enum MmsPduType : byte
    {
        ConfirmedRequest = 0xA0,
        ConfirmedResponse = 0xA1,
        ConfirmError = 0xA2,
        Unconfirmed = 0xA3,
        Reject = 0xA4,
        CancelRequest = 0xA5,
        CancelResponse = 0xA6,
        CancelError = 0xA7,
        InitiateRequest = 0xA8,
        InitiateResponse = 0xA9,
        InitiateError = 0xAA,
        ConcludeRequest = 0xAB,
        ConcludeResponse = 0xAC,
        ConcludeError = 0xAD
    }
}
