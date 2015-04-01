using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace IEC61850Packet.Goose
{
    public class GoosePacket : Packet
    {

        public ushort APPID { get; private set; }

        public ushort Length { get; private set; }

        public ushort Reserved1
        {
            get { return 0; }
            private set
            {
                if(value!=0) 
                {
                    throw new ArgumentOutOfRangeException("Reserved1", "Attemp to set a non-zero value to Reserved1.");
                }
            }
        }
        public ushort Reserved2
        {
            get { return 0; }
            private set
            {
                if (value != 0)
                {
                    throw new ArgumentOutOfRangeException("Reserved2", "Attemp to set a non-zero value to Reserved2.");
                }
            }
        }

        public Apdu APDU { get;private set; }

        public GoosePacket(ByteArraySegment bas, Packet parent)
        {
            base.ParentPacket = parent;
            base.header = bas;

            // TODO: Decode header
            base.header.Length = GooseFileds.HeaderLength;
            byte[] ba_header=header.ActualBytes();
            int pos = 0;
            APPID = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
            pos += GooseFileds.APPIDLength;
            Length = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
            pos += GooseFileds.LengthLength;
            Reserved1 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
            pos += GooseFileds.Reserved1Length;
            Reserved2 = BigEndianBitConverter.Big.ToUInt16(ba_header, pos);
            pos += GooseFileds.Reserved2Length;
            APDU = new Apdu(header.EncapsulatedBytes());
            base.payloadPacketOrData.TheByteArraySegment = APDU.Bytes;
            
        }

        public GoosePacket(byte[] rawData, Packet parent)
            : this(new ByteArraySegment(rawData), parent)
        {

        }



        /*
        public override string ToString()
        {
            string result = "GOOSE Packet:{MAC Header:[";
            result += "DistAddress:" + BitConverter.ToString(Header.DistAddress) + ",";
            result += "SrcAddress:" + BitConverter.ToString(Header.SrcAddress) + "],";
            result += "Priority Flag:[TPID:" + BitConverter.ToString(Priority.TPID) + ",";
            result += "TCI:" + BitConverter.ToString(Priority.TCI) + "],";
            result += "Enther Type:" + EntherTypeMapping.GetType(EntherType) + ",";
            result += "APPID:" + BitConverter.ToString(APPID) + ",";
            result += "Length:" + (APDU.Raw.Length + 8) + ",";
            result += "Reversed1:" + BitConverter.ToString(Reversed1) + ",";
            result += "Reversed2:" + BitConverter.ToString(Reversed2) + ",";
            result += APDU.ToString();
            result = result.Substring(0, result.Length - 1);
            result += "}";
            return result;
        }
        */
    }

}
