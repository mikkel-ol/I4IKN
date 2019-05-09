namespace Core
{
    public class Data : IData
    {
        public Data(byte[] data)
        {
            this.Value = data;
        }

        // Assignment operator overloading
        public static implicit operator Data(byte[] b)
        {
            return new Data(b);
        }
    }
}