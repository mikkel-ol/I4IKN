using System.Collections;

namespace Core
{
    public abstract class IHeader
    {
        public byte CS_HI { get; protected set; }
        public byte CS_LO { get; protected set; }
        public byte SEQ { get; protected set; }
        public byte TYPE { get; protected set; }

        // Assignment operator overloading
        public static implicit operator byte[](IHeader h)
        {
            var arr = new ArrayList();

            arr.Add(h.CS_HI);
            arr.Add(h.CS_LO);
            arr.Add(h.SEQ);
            arr.Add(h.TYPE);

            return (byte[])arr.ToArray(typeof(byte));
        }
    }

    public enum Type
    {
        DATA,
        ACK
    }
}