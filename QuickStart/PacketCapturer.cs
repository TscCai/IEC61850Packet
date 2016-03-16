using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IEC61850Packet.Acsi;
using IEC61850Packet.Device;
using IEC61850Packet.Goose;
using IEC61850Packet.Mms;
using IEC61850Packet.Mms.Types;
using IEC61850Packet.Utils;
using Newtonsoft.Json;
using PacketDotNet;
using SharpPcap;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using TAsn1 = IEC61850Packet.Asn1.Types;


namespace QuickStart
{
	public class PacketCapturer
	{
		SharpPcap.ICaptureDevice dev;

		public PacketCapturer()
		{
		}

		public async Task CapturePacketsAsync(string deviceName)
		{
			Task task = Task.Run(() =>
			{

				dev = SharpPcap.CaptureDeviceList.Instance[2];
				// Instance[2] for wifi
				// Instance[5] for LAN
				int readTimeoutMilliseconds = 1000;
				
				dev.OnPacketArrival +=
			   new PacketArrivalEventHandler(dev_OnPacketArrival);

				if (dev is AirPcapDevice)
				{
					// NOTE: AirPcap devices cannot disable local capture
					var airPcap = dev as AirPcapDevice;

					airPcap.Open(SharpPcap.WinPcap.OpenFlags.DataTransferUdp, readTimeoutMilliseconds);
				}
				else if (dev is WinPcapDevice)
				{
					var winPcap = dev as WinPcapDevice;
					winPcap.Open(SharpPcap.WinPcap.OpenFlags.DataTransferUdp | SharpPcap.WinPcap.OpenFlags.NoCaptureLocal, readTimeoutMilliseconds);
				}
				else if (dev is LibPcapLiveDevice)
				{
					var livePcapDevice = dev as LibPcapLiveDevice;
					livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
				}
			
				dev.Capture();
			});
			await task;
		}

		void dev_OnPacketArrival(object sender, CaptureEventArgs e)
		{
			bool broadcast = false;
			//ResolveDevice rd = new ResolveDevice(null);
			Packet p = ResolveDevice.Resovle(e.Packet);

			StringWriter sw = new StringWriter();
			JsonWriter jw = new JsonTextWriter(sw);
			if (p.GetType() == typeof(MmsPacket))
			{
				MmsPacket mms = p as MmsPacket;
				if (mms.Pdu is UnconfirmedPdu)
				{
					AcsiMapping acsi = mms.Acsi;
					ReportedOptFldsType excpted = ReportedOptFldsType.DataSetName | ReportedOptFldsType.DataReference |
						ReportedOptFldsType.ReasonForInclusion;// | ReportedOptFldsType.ReportTimeStamp;
					bool test = (acsi.ReportedOptFlds & excpted) == excpted;
					//test = true;

					if (test)
					{
						string ip = mms.ParentPacket<IPv4Packet>().SourceAddress.ToString();
						pm_ied = Map.IED.SingleOrDefault(ied => ied.MMS.IP == ip);	// SingleOrDefault IP重复配置
						if (pm_ied == null)
						{
							// Consider Network B
							var tmp = ip.Split('.');
							ip = tmp[0] + ".120." + tmp[2] + "." + tmp[3];
							pm_ied = Map.IED.SingleOrDefault(ied => ied.MMS.IP == ip);
						}
						if (pm_ied != null)
						{
							string iedName = pm_ied.Name;
							string iedDesc = pm_ied.Desc;
							string rawDatSet = acsi.DatSet;
							string dsName = rawDatSet.Substring(rawDatSet.LastIndexOf('$') + 1);

							string ldInst = rawDatSet.Split('/')[0].Replace(iedName, "");
							var pm_ds = pm_ied.MMS.DataSet.SingleOrDefault(ds => ds.LdInst == ldInst && ds.Name == dsName);
							string dsDesc = pm_ds.Desc;

							// TODO: Search the related
							// warn.Related = {"",""};
							// warn.TimeofEntry = asci.TimeofEntry;

							jw.WriteStartObject();
							jw.WritePropertyName("ied"); jw.WriteValue(iedName);

							jw.WritePropertyName("desc"); jw.WriteValue(iedDesc);
							jw.WritePropertyName("dataSet");
							jw.WriteStartObject();
							jw.WritePropertyName("type"); jw.WriteValue("MMS");
							jw.WritePropertyName("name"); jw.WriteValue(dsName);
							jw.WritePropertyName("desc"); jw.WriteValue(dsDesc);
							jw.WritePropertyName("warnings"); jw.WriteStartArray();

					
							if (dsName.Contains("dsAlarm") || dsName.Contains("dsWarning")
								|| dsName.Contains("dsCommState") || dsName.Contains("dsTripInfo"))
							{
								for (int i = 0; i < acsi.DataRef.Count; i++)
								{
									var item = pm_ds.Item.SingleOrDefault(d => d.Ref == acsi.DataRef[i].Replace(iedName, ""));
									if (item.Alarm)
									{
										//  sb.AppendFormat("{0}: {1}, Reason: {2}<br/>", item.Value, acsi.Value[i].ToString(), acsi.ReasonCode[i]);
										jw.WriteStartObject();
										jw.WritePropertyName("ref"); jw.WriteValue(item.Ref);
										jw.WritePropertyName("desc"); jw.WriteValue(item.Value);
										jw.WritePropertyName("value");
										jw.WriteValue(acsi.Value[i].ToString());
										jw.WriteEndObject();
										broadcast = true;
									}
								}
							}
							jw.WriteEndArray();
							jw.WriteEndObject();
							jw.WriteEndObject();
						}

					}
				}
			}
			else if (p.GetType() == typeof(GoosePacket))
			{
				var goose = (GoosePacket)p;
				string gocbRef = goose.APDU.gocbRef.Value;
				string appId = goose.APPID.ToString("X4");
				TG.DataSet pm_ds = Map.IED.GetDataSetByAPPID(appId);
				pm_ied = Map.IED.GetIEDByAPPID(appId);
				//	List<string> related = pm_ied.GetRelatedIed();
				//sb.AppendFormat("gocbRef: {0} APPID: {1}<br/>", pm_ds.gocbRef, pm_ds.APPID);

				// Check if alarm
				if (pm_ied != null && pm_ds != null)
				{
					string iedName = pm_ied.Name;
					string iedDesc = pm_ied.Desc;

					jw.WriteStartObject();
					jw.WritePropertyName("ied"); jw.WriteValue(iedName);
					jw.WritePropertyName("desc"); jw.WriteValue(iedDesc);

					jw.WritePropertyName("dataSet");
					jw.WriteStartObject();
					jw.WritePropertyName("type"); jw.WriteValue("GOOSE");
					jw.WritePropertyName("desc"); jw.WriteValue(pm_ds.Desc);
					jw.WritePropertyName("gocbRef"); jw.WriteValue(pm_ds.gocbRef);
					jw.WritePropertyName("cbName"); jw.WriteValue(pm_ds.cbName);
					jw.WritePropertyName("VLAN"); jw.WriteValue(pm_ds.VLAN);
					jw.WritePropertyName("MAC"); jw.WriteValue(pm_ds.MAC);
					jw.WritePropertyName("APPID"); jw.WriteValue(pm_ds.APPID);
					jw.WritePropertyName("warnings"); jw.WriteStartArray();
					for (int i = 0; i < pm_ds.Item.Length; i++)
					{
						var type = goose.APDU.allData[i].Type;
						object value;
						switch (type)
						{
							case IEC61850Packet.Mms.Types.Data.VariableType.Boolean:
								value = goose.APDU.allData[i].GetValue<TAsn1.Boolean>().Value;
								break;
							case IEC61850Packet.Mms.Types.Data.VariableType.FloatPoint:
								value = goose.APDU.allData[i].GetValue<FloatPoint>().Value;
								break;
							default:
								value = null;
								break;
						}
						var item = pm_ds.Item[i];
						if (value is bool && item.Alarm)
						{
							// This packet is a warning packet and contains alarm
							//Console.ForegroundColor = ConsoleColor.Red;

							jw.WriteStartObject();
							jw.WritePropertyName("ref"); jw.WriteValue(item.Ref);
							jw.WritePropertyName("desc"); jw.WriteValue(item.Value);
							jw.WritePropertyName("value"); jw.WriteValue(value);
							if (item.To != null)
							{
								jw.WritePropertyName("to"); jw.WriteValue(item.To);
							}
							if (item.ExtAddr != null)
							{
								jw.WritePropertyName("extAddr"); jw.WriteValue(item.ExtAddr);
							}
							if (item.ExtDesc != null)
							{
								jw.WritePropertyName("extDesc"); jw.WriteValue(item.ExtDesc);
							}
							jw.WriteEndObject();


							//   sb.AppendFormat("{0}: isAlarm: {1}, isTrigged: {2}<br/>", pm_ds.Item[i].Value, pm_ds.Item[i].Alarm, value);
							broadcast = true;
							//Console.ResetColor();
						}

						//   sb.AppendFormat("{0}: isAlarm: {1}, Value: {2}<br/>", pm_ds.Item[i].Value, pm_ds.Item[i].Alarm, value);
					}
					jw.WriteEndArray();
					jw.WriteEndObject();
					jw.WriteEndObject();
				}
			}


			//    sb.Append("<hr/>");
			if (broadcast)
			{
				BroadcastHandler.Clients.Broadcast(sw.ToString());
				broadcast = false;
			}

			jw.Flush();

		}

	}
}