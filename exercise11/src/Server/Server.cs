using System;

using Core;

namespace Server
{
    public class Server
    {
        public Transport Transport { get; private set; }

        public Server(string port)
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