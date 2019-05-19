namespace Core
{
    public abstract class IData
    {
        public int Length { get; set; } = 1000;
        public byte[] Value { get; protected set; }

        // Assignment operator overloading
        public static implicit operator byte[](IData d)
        {
            if (d == null) return new byte[0];
            return d.Value;
        }
    }
}