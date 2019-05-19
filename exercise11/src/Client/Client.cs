using System;

using Core;

namespace Client
{
    public class Client
    {
        public Transport Transport { get; private set; }

        public Client(string port)
        {
            this.Transport = new Transport(port, 2);
        }

        public void Send(byte[] data)
        {
            this.Transport.Send(data);
        }

        public void Receive(ref byte[] buffer)
        {
            this.Transport.Receive(ref buffer);
        }
    }
}