namespace Core
{
    public interface ISLIP
    {
        bool Send(byte[] data, int length);
        int Receive(ref byte[] data, int length);
    }
}