namespace Core
{
    public interface ISerial
    {
        bool Open();
        bool Send(byte[] data, int length);
        int Receive(ref byte[] buffer, int length = 4096);
    }

    public enum Parity
    {
        None,
        Odd,
        Even,
        Mark,
        Space
    }

    public enum StopBits
    {
        None,
        One,
        Two,
        OnePointFive
    }
}