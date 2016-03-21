using System;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;
using IEC61850Packet.Device;
using IEC61850Packet.Goose;
using IEC61850Packet.Mms;
using IEC61850Packet.Utils;

using TAsn1 = IEC61850Packet.Asn1.Types;
using IEC61850Packet.Mms.Acsi;
using IEC61850Packet.Asn1.Types;
using IEC61850Packet.Asn1;
using IEC61850Packet.Sv;

namespace QuickStart
{
	public class PacketLoader
	{
		ResolveDevice dev;

		public string ScdFilename { get; private set; }
		public string MapFilename { get; private set; }
		string ConnFilename { get; set; }

		public PacketLoader() { }
		public PacketLoader(string scd, string map)
		{
			ScdFilename = scd;
			MapFilename = map;
			ConnFilename = @"..\config\connection.xml";

			LoadConfigure();
		}

		public void LoadConfigure()
		{/*
            if (!File.Exists(MapFilename))
            {
                // Generate map of ref and desc.
                Parser par = new Parser(ScdFilename);
                //par.ScdParsing+=new ScdParsingEventHandler(Parser_ScdParsing);

                par.ParseMap(MapFilename);
                par = null;
                GC.Collect(GC.MaxGeneration);
            }
            Map = Map.FromXml(MapFilename);
            //      Conn = Switches.FromXml(ConnFilename);
		  * */
		}

		public void LoadPackets(string filename)
		{
			dev = new ResolveDevice(filename);
			dev.OnOpened += Dev_OnOpened;
			//dev.OnOpening += Dev_OnOpening;
			dev.Open();

			dev.Close();
		}

		public async Task LoadPacketsAsync(string filename)
		{
			Task task = Task.Run(() =>
			{
				dev = new ResolveDevice(filename);
				dev.OnOpened += Dev_OnOpened;
				//dev.OnOpening += Dev_OnOpening;
				dev.Open();

				dev.Close();
			});
			await task;
		}

		void Dev_OnOpened(object sender, DeviceOpenedEventArgs e)
		{
			while (dev.HasNextPacket)
			{
				Packet p = dev.GetNextPacket();
				StringBuilder sb = new StringBuilder();
				//      StringWriter sw = new StringWriter();

				if (p.GetType() == typeof(MmsPacket))
				{
					MmsPacket mms = p as MmsPacket;
					if (mms.Pdu is UnconfirmedPdu)
					{
						AcsiMapping acsi = mms.Acsi;
						ReportedOptFldsType excpted = ReportedOptFldsType.DataSetName | ReportedOptFldsType.DataReference |
							ReportedOptFldsType.ReasonForInclusion | ReportedOptFldsType.ReportTimeStamp;
						bool test = (acsi.ReportedOptFlds & excpted) == excpted;


						if (test)
						{
							string ip = mms.ParentPacket<IPv4Packet>().SourceAddress.ToString();
							string rawDatSet = acsi.DatSet;
							string dsName = rawDatSet.Substring(rawDatSet.LastIndexOf('$') + 1);
							sb.AppendFormat("RptID: {0}\n", acsi.RptID);
							sb.AppendFormat("ReportedFileds: {0}\n", acsi.ReportedOptFlds);
							sb.AppendFormat("TimeofEntry: {0}\n", acsi.TimeofEntry);
							sb.AppendFormat("DatSet: {0},dsName(not a field of the packet): {1}\n", rawDatSet, dsName);
							sb.AppendFormat("EntryID: {0}\n", acsi.EntryID);
							sb.AppendFormat("Inclusion-bitstring: {0}\n", acsi.Inclusion_Bitstring);

							for (int i = 0; i < acsi.DataRef.Count; i++)
							{
								sb.AppendFormat("{0}: {1}, Reason: {2}\n", acsi.DataRef[i], acsi.Value[i].ToString(), acsi.ReasonCode[i]);

							}

						}

					}

				}
				else if (p.GetType() == typeof(GoosePacket))
				{
					var goose = (GoosePacket)p;
					string appId = goose.APPID.ToString("X4");
					sb.AppendFormat("gocbRef: {0} APPID: {1}\n", goose.APDU.gocbRef.Value, appId);

					// Check if alarm
					sb.AppendFormat("timeAllowedtoLive:{0}", goose.APDU.timeAllowedtoLive);
					sb.AppendFormat("datSet: {0}", goose.APDU.datSet);
					sb.AppendFormat("numDatSetEntries: {0}", goose.APDU.numDatSetEntries);
					for (int i = 0; i < goose.APDU.numDatSetEntries.Value; i++)
					{
						var type = goose.APDU.allData[i].Type;
						Type t = System.Reflection.Assembly.Load("IEC61850Packet").GetType("IEC61850Packet.Asn1.Types.UtcTime");
						object value;
						switch (type)
						{
							case VariableType.Boolean:
								value = goose.APDU.allData[i].GetValue<TAsn1.Boolean>().Value;
								sb.AppendFormat("No.{0}. Boolean: {1}\n", i + 1, value);
								break;
							case VariableType.FloatPoint:
								value = goose.APDU.allData[i].GetValue<FloatPoint>().Value;
								sb.AppendFormat("No.{0}. FloatPoint: {1}\n", i + 1, value);
								break;
							case VariableType.BitString:
								value = goose.APDU.allData[i].GetValue<BitString>().Value;
								sb.AppendFormat("No.{0}. BitString: {1}\n", i + 1, value);
								break;
							case VariableType.UtcTime:
								value = goose.APDU.allData[i].GetValue<UtcTime>().Value;
								sb.AppendFormat("No.{0}. UtcTime: {1}\n", i + 1, value);
								break;
							default:
								value = null;
								sb.AppendFormat("No.{0}. Unknown: {1}\n", i + 1, value);
								break;
						}
					}
					Console.WriteLine(sb.ToString());
				}
				else if (p.GetType() == typeof(SvPacket))
				{
					var sv = (SvPacket)p;
					Console.WriteLine("SV 9-2, APPID: {0}, svID: {1}", sv.APPID,sv.APDU.ASDU[0].svID);
				}
				Console.WriteLine(sb.ToString());
				sb.Clear();
			}
			Console.WriteLine("{0} packets have been resolved", dev.PacketCount);
		}

		void Dev_OnOpening(object sender, DeviceOpeningEventArgs e)
		{
			Console.Clear();
			Console.WriteLine("Processing: {0}", e.Progress.ToString("P2"));
		}
	}
}