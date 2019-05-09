namespace Core
{
    public class Packet : IPacket
    {
        public Packet(IHeader header, IData data)
        {
            this.Header = header;
            this.Data = data;
        }
    }
}