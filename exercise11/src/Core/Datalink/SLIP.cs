using System;
using System.Collections;

namespace Core 
{
    public class SLIP : ISLIP
    {
        public char Delimiter { get; set; } = 'A';
        public char FrameEscape { get; set; } = 'B';
        public char TransposedFrameEnd { get; set; } = 'C';
        public char TransposedFrameEscape { get; set; } = 'D';

        public ISerial Device { get; private set; }
        
        public SLIP(ISerial dev)
        {
            this.Device = dev;
        }

        public SLIP(string serialPath) 
            : this(new Serial(serialPath)) {}

        public SLIP() 
            : this(new Serial()) {}

        public bool Send(byte[] data, int length)
        {
            byte[] encoded = Encode(data);

            return this.Device.Send(encoded, encoded.Length);
        }

        public int Receive(ref byte[] data, int length)
        {
            byte[] buf = new byte[length*2+4]; // Should be able to handle wrost case

            int recv = this.Device.Receive(ref buf, buf.Length);

            if (recv == -1) return recv;

            recv = Decode(buf, ref data, recv);

            return recv;
        }

        private byte[] Encode(byte[] data)
        {
            // Need dynamic sized array
            var dataEndoded = new ArrayList();

            dataEndoded.Add((byte) this.Delimiter);

            foreach(byte c in data)
            {
                // TODO: Optimize if statements (else statement is most common)
                if (c == (byte) this.Delimiter)                           // 'A'
                {
                    dataEndoded.Add((byte) this.FrameEscape);             // 'B'
                    dataEndoded.Add((byte) this.TransposedFrameEnd);      // 'C'
                }
                else if (c == (byte) this.FrameEscape)                    // 'B'
                {
                    dataEndoded.Add((byte) this.FrameEscape);             // 'B'
                    dataEndoded.Add((byte) this.TransposedFrameEscape);   // 'D'
                }
                else
                {
                    dataEndoded.Add((byte) c);
                }
            }

            dataEndoded.Add((byte) this.Delimiter);

            return (byte[]) dataEndoded.ToArray(typeof(byte));
        }

        private int Decode(byte[] src, ref byte[] dst, int len)
        {
            // Need dynamic sized array
            var dataDecoded = new ArrayList();

            // Ignore first and last byte (delimiters)
            for (int i = 1; i < len - 1; i++)
            {
                byte current = src[i];

                if (current == (byte) this.FrameEscape)                     // 'B'
                {
                    byte next = src[i+1];

                    if (next == (byte) this.TransposedFrameEnd)             // 'C'
                    {
                        dataDecoded.Add((byte) this.Delimiter);             // 'A'
                    }
                    else if (next == (byte) this.TransposedFrameEscape)     // 'D'
                    {
                        dataDecoded.Add((byte) this.FrameEscape);           // 'B'
                    }
                }
                else
                {
                    dataDecoded.Add(current);
                }
            }

           var datArr = (byte[]) dataDecoded.ToArray(typeof(byte));
           Array.Copy(datArr, dst, datArr.Length);
           return datArr.Length;
        }
    }
}