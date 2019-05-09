namespace Core
{
    public abstract class IData
    {
        public int Length { get; set; } = 1000;
        public byte[] Value { get; protected set; }

        // Assignment operator overloading
        public static implicit operator byte[](IData d)
        {
            return d.Value;
        }
    }
}