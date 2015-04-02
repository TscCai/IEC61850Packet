using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC61850Packet.Device
{
    public class DeviceOpeningEventArgs : EventArgs
    {
        public double Progress { get; set; }
        public DeviceOpeningEventArgs() { }
        public DeviceOpeningEventArgs(double progress)
        {
            Progress = progress;
        }
    }
}
