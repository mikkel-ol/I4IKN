using System.Collections;

namespace Core
{
    public abstract class IPacket
    {
        public IHeader Header { get; protected set; }
        public IData Data { get; protected set; }

        // Assignment operator overloading
        public static implicit operator byte[] (IPacket p)
        {
            var arr = new ArrayList();

            byte[] header = p.Header;
            byte[] data = p.Data;

            foreach(byte b in header)
            {
                arr.Add(b);
            }

            foreach(byte b in data)
            {
                arr.Add(b);
            }

            return (byte[]) arr.ToArray(typeof(byte));
        }
    }
}