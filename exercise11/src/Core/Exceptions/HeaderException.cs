namespace Core
{
    [System.Serializable]
    public class HeaderException : System.Exception
    {
        public HeaderException() { }
        public HeaderException(string message) : base(message) { }
        public HeaderException(string message, System.Exception inner) : base(message, inner) { }
        protected HeaderException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}