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
            //string captureFilename = @"..\..\..\Test\CapturedFiles\20140725-210910_00036E3E-20+20_RcdD05.pcap";
            //string captureFilename = @"..\..\..\Test\CapturedFiles\20140826-113450-10+20_RcdD05_.pcap";
            string captureFilename = @"..\..\..\Test\CapturedFiles\[220kV B网]20140820-093320_0006D1FF-10+10.pcap";
			//string captureFilename = @"..\..\..\Test\CapturedFiles\SV.pcap";
			PacketLoader loader = new PacketLoader();
			DateTime start = DateTime.Now;
			loader.LoadPackets(captureFilename);
			// var task = loader.LoadPacketsAsync(captureFilename);	// Won't work in CMD program.


			DateTime end = DateTime.Now;
			Console.WriteLine("Time eclapsed: {0}", (end - start).TotalSeconds);
			Console.ReadLine();
        }

    }
}
