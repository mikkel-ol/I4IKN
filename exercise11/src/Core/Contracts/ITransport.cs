namespace Core
{
    public interface ITransport
    {
        int MaxSize { get; }
        ISLIP Slip { get; }
        ITimeout Timeout { get; }

        void Send(byte[] data);
        void Receive(ref byte[] buffer);
    }
}