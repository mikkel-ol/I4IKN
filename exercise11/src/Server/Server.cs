using System;
using System.Collections;

using Core;

namespace Server
{
    public class Server
    {
        public Transport Transport { get; private set; }

        public Server(string port, int size)
        {
            this.Transport = new Transport(port, size);
        }

        public void Send(byte[] data)
        {
            this.Transport.Send(data);
        }

        public void Receive(ref ArrayList buffer)
        {
            this.Transport.Receive(ref buffer);
        }
    }
}