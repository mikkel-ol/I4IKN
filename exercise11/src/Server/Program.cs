using System;
using System.Text;

using Core;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // var msg = "Test message";
            // var bMsg = Encoding.UTF8.GetBytes(msg);

            // var chksum = new Checksum(bMsg);

            // byte chksumHigh = (byte) ((chksum.Value >> 8) & 0xFF);
            // byte chksumLow = (byte) (chksum.Value & 0xFF);
            // byte seq = 0b1;
            // var type = Core.Type.DATA;

            // var header = new Header(chksumHigh, chksumLow, seq, type);
            // var data = new Data(bMsg);

            // var packet = new Packet(header, data);

            // byte[] bPacket = packet;

            // var device = new Serial();
            // var transport = new Transport(device);
            // transport.Send(bPacket);
        }
    }
}
