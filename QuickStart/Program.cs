using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEC61850Packet.Device;

namespace QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            //string captureFilename = @"..\..\..\Test\CapturedFiles\20140813-150920_0005ED9B-50+60_MMS.pcap";
            // string captureFilename = @"..\..\..\Test\CapturedFiles\20140725-210910_00036E3E-20+20_RcdD05.pcap";
            string captureFilename = @"..\..\..\Test\CapturedFiles\20140826-113450-10+20_RcdD05_.pcap";
            ResolveDevice dev = new ResolveDevice(captureFilename);
            DateTime start = DateTime.Now;
            dev.Open();
            // some works here
            dev.Close();
            DateTime end = DateTime.Now;
            Console.WriteLine("Time eclapsed: {0}", (end - start).TotalSeconds);
            Console.WriteLine("Done.");
        }
    }
}
