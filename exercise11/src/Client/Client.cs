using System;
using System.Collections;

using Core;

namespace Client
{
    public class Client
    {
        public Transport Transport { get; private set; }

        public Client(string port, int size)
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