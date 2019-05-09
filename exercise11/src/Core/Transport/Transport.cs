using System;

namespace Core
{
    public class Transport : ITransport
    {
        public SLIP Slip { get; private set; }

        public Transport(ISerial device)
        {
            this.Slip = new SLIP(device);
        }

        public void Send(byte[] packet)
        {
            this.Slip.Send(packet, packet.Length);
        }
    }
}