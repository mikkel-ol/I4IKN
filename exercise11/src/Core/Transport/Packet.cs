using System.Collections;

namespace Core
{
    public class Packet : IPacket
    {
        public Packet(IHeader header, IData data)
        {
            this.Header = header;
            this.Data = data;
        }

        // Assignment operator overloading
        public static implicit operator Packet (byte[] b)
        {
            var header = new Header(b[0], b[1], b[2], (Core.Type) b[3]);
            
            var dataList = new ArrayList();

            for (int i = 4; i < b.Length; i++)
            {
                dataList.Add(b[i]);
            }

            var dataArray = (byte[]) dataList.ToArray(typeof(byte));

            var data = new Data(dataArray);

            return new Packet(header, data);
        }
    }
}